using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using Microsoft.EntityFrameworkCore;
namespace BetterCRM.DataAccess.Repositories
{
    public class PayrollRepository : Repository<PayrollRecord>, IPayrollRepository
    {
        public PayrollRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PayrollRecord?> GetByUserAndPeriodAsync(Guid userId, DateTime periodStart, DateTime periodEnd) =>
             await _dbSet.FirstOrDefaultAsync(pr => pr.UserId ==  userId && pr.PeriodStart == periodStart && pr.PeriodEnd == periodEnd);

        public async Task<List<PayrollRecord>> GetByDepartmentAsync(Guid departmentId, int year, int month)
        {
            var start = new DateTime(year, month , 1);
            return await _dbSet.Where(pr => pr.User.DepartmentId == departmentId && pr.PeriodStart == start).ToListAsync();
        }
        public async Task UpdateStatusAsync(Guid recordId, string status)
        {
            var r = await _dbSet.FirstAsync(pr => pr.Id == recordId);
            r.ChangeStatus(status);
            await _context.SaveChangesAsync();
        }
    }
}
