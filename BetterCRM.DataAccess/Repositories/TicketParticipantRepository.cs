using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BetterCRM.DataAccess.Repositories
{
    public class TicketParticipantRepository : Repository<TicketParticipant>, ITicketParticipantRepository
    {
        public TicketParticipantRepository(ApplicationDbContext context) : base(context) { }

        public async Task<bool> IsParticipantAsync(Guid ticketId, Guid userId) => 
            await _dbSet.AnyAsync(tp => tp.TicketId == ticketId && tp.UserId == userId);

        public async Task<List<TicketParticipant>> GetByTicketAsync(Guid ticketId) =>
            await _dbSet.Where(tp => tp.TicketId == ticketId).Include(tp => tp.User).ToListAsync();

        public async Task RemoveByTicketAndUserAsync(Guid ticketId, Guid userId) 
        { 
            var p = await _dbSet.FirstOrDefaultAsync(tp => tp.TicketId == ticketId && tp.UserId == userId); 
            if (p != null) 
            {
                _dbSet.Remove(p); 
                await _context.SaveChangesAsync(); 
            } 
        }
    }
}

