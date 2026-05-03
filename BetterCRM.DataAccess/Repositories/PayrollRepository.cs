using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
namespace BetterCRM.DataAccess.Repositories
{
    public class PayrollRepository : Repository<PayrollRecord, PayrollRecordEntity>, IPayrollRepository
    {
        public PayrollRepository(ApplicationDbContext context) : base(context) { }
        protected override PayrollRecord MapToDomain(PayrollRecordEntity db) => DomainMapper.ToPayrollDomain(db);
        protected override PayrollRecordEntity MapToDb(PayrollRecord domain, PayrollRecordEntity? existing = null) => DomainMapper.ToPayrollDb(domain, existing);

        public async Task<PayrollRecord?> GetByUserAndPeriodAsync(Guid userId, DateTime periodStart, DateTime periodEnd)
        {
            var db = await _dbSet.FirstOrDefaultAsync(pr => pr.UserId == userId && pr.PeriodStart == periodStart && pr.PeriodEnd == periodEnd);
            return db != null ? MapToDomain(db) : null;
        }

        public async Task<List<PayrollRecord>> GetByDepartmentAsync(Guid departmentId, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var list = await _dbSet.Where(pr => pr.User.DepartmentId == departmentId && pr.PeriodStart == start).ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task UpdateStatusAsync(Guid recordId, string status)
        {
            var db = await _dbSet.FirstAsync(pr => pr.Id == recordId);
            var domain = MapToDomain(db);
            domain.ChangeStatus(status);
            MapToDb(domain, db);
            await _context.SaveChangesAsync();
        }
    }
}
