using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface IShiftBreakRepository : IRepository<ShiftBreak>
    {
        Task<List<ShiftBreak>> GetByShiftAsync(Guid shiftId);
    }
}
