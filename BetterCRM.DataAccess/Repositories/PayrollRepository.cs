using BetterCRM.Core.Extensions;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using static BetterCRM.Business.Exceptions.DomainException;

namespace BetterCRM.DataAccess.Repositories
{
    public class PayrollRepository : Repository<PayrollRecord, PayrollRecordEntity>, IPayrollRepository
    {
        public PayrollRepository(ApplicationDbContext context) : base(context) { }

        protected override PayrollRecord MapToDomain(PayrollRecordEntity db) => DomainMapper.ToPayrollDomain(db);
        protected override PayrollRecordEntity MapToDb(PayrollRecord domain, PayrollRecordEntity? existing = null) => DomainMapper.ToPayrollDb(domain, existing);

        public async Task<PayrollRecord?> GetByUserAndPeriodAsync(Guid userId, DateTime periodStart, DateTime periodEnd)
        {
            var db = await _dbSet.FirstOrDefaultAsync(pr =>
                pr.UserId == userId &&
                pr.PeriodStart == periodStart.Date &&
                pr.PeriodEnd == periodEnd.Date);
            return db != null ? MapToDomain(db) : null;
        }

        public async Task<List<PayrollRecord>> GetByDepartmentAsync(Guid departmentId, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var list = await _dbSet
                .Include(pr => pr.User)
                .Where(pr => pr.User.DepartmentId == departmentId && pr.PeriodStart == start)
                .ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task UpdateStatusAsync(Guid recordId, PayrollStatus status)
        {
            var db = await _dbSet.FirstOrDefaultAsync(pr => pr.Id == recordId)
                ?? throw new InvalidOperationException($"PayrollRecord {recordId} не найден");
            var domain = MapToDomain(db);
            domain.ChangeStatus(status);
            MapToDb(domain, db);
            await _context.SaveChangesAsync();
        }
        public async Task<decimal> GetTicketPenaltyHoursAsync(Guid userId, DateTime from, DateTime to) =>
            await _context.Tickets
                .Where(t =>
                    (t.AssigneeId == userId || t.CreatorId == userId) &&
                    t.IsSLABreached &&
                    t.OverduePenaltyHours > 0 &&
                    t.CreatedAt >= from && t.CreatedAt <= to)
                .SumAsync(t => t.OverduePenaltyHours);

        public async Task UpdateStatusAsync(Guid recordId, string status)
        {
            var allowed = new[] { "Calculated", "Approved", "Paid" };
            if (!allowed.Contains(status))
                throw new ArgumentException($"Неизвестный статус: '{status}'. Допустимые: {string.Join(", ", allowed)}");

            var db = await _dbSet.FirstOrDefaultAsync(pr => pr.Id == recordId)
                ?? throw new NotFoundException($"PayrollRecord {recordId} не найден");

            var domain = MapToDomain(db);
            domain.ChangeStatus(status);
            MapToDb(domain, db);

            await _context.SaveChangesAsync(); ;
        }

        public async Task UpsertAsync(PayrollRecord record)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var existing = await _dbSet.FirstOrDefaultAsync(p =>
                    p.UserId == record.UserId &&
                    p.PeriodStart == record.PeriodStart.Date);

                if (existing != null)
                    _dbSet.Remove(existing);

                var entity = MapToDb(record);
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

        }
    }
}