using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterCRM.DataAccess.Repositories
{
    public class TicketParticipantRepository : Repository<TicketParticipant, TicketParticipantEntity>, ITicketParticipantRepository
    {
        public TicketParticipantRepository(ApplicationDbContext context) : base(context) { }
        protected override TicketParticipant MapToDomain(TicketParticipantEntity db) => DomainMapper.ToParticipantDomain(db);
        protected override TicketParticipantEntity MapToDb(TicketParticipant domain, TicketParticipantEntity? existing = null) => DomainMapper.ToParticipantDb(domain, existing);

        public async Task<bool> IsParticipantAsync(Guid ticketId, Guid userId) =>
            await _dbSet.AsNoTracking().AnyAsync(tp => tp.TicketId == ticketId && tp.UserId == userId);

        public async Task<List<TicketParticipant>> GetByTicketAsync(Guid ticketId)
        {
            var list = await _dbSet.Where(tp => tp.TicketId == ticketId).Include(tp => tp.User).ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task RemoveByTicketAndUserAsync(Guid ticketId, Guid userId)
        {
            var db = await _dbSet.FirstOrDefaultAsync(tp => tp.TicketId == ticketId && tp.UserId == userId);
            if (db != null)
            {
                _dbSet.Remove(db);
                await _context.SaveChangesAsync();
            }
        }
    }
}

