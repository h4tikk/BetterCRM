using BetterCRM.Business.Exceptions;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.Core.Models;
using static BetterCRM.Business.Exceptions.DomainException;

namespace BetterCRM.Business.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly ITicketParticipantRepository _participantRepo;
        private readonly IUserRepository _userRepo;

        public TicketService(ITicketRepository ticketRepo, ITicketParticipantRepository participantRepo, IUserRepository userRepo)
            => (_ticketRepo, _participantRepo, _userRepo) = (ticketRepo, participantRepo, userRepo);

        public async Task<Ticket> CreateAsync(CreateTicketCommand cmd)
        {
            var creator = await _userRepo.GetByIdAsync(cmd.CreatorId) ?? throw new NotFoundException("Создатель не найден");
            var (ticket, err) = Ticket.Create(creator.OrganizationId, cmd.Title, cmd.Description, cmd.Priority, cmd.CreatorId, cmd.AssigneeId);
            if (err != null) throw new DomainException(err);
            return await _ticketRepo.AddAsync(ticket);
        }

        public async Task<Ticket?> GetByIdAsync(Guid id) => await _ticketRepo.GetByIdAsync(id);

        public async Task<List<Ticket>> GetFilteredAsync(Guid userId, string role, Guid? departmentId) =>
            await _ticketRepo.GetForUsersAsync(userId, role, departmentId);

        public async Task ResolveAsync(Guid ticketId)
        {
            var t = await _ticketRepo.GetByIdAsync(ticketId) ?? throw new NotFoundException("Тикет не найден");
            t.Resolve();
            await _ticketRepo.UpdateAsync(t);
        }

        public async Task AddParticipantAsync(Guid ticketId, Guid userId, string role)
        {
            if (await _participantRepo.IsParticipantAsync(ticketId, userId))
                throw new ConflictException("Участник уже добавлен");
            var t = await _ticketRepo.GetByIdAsync(ticketId) ?? throw new NotFoundException("Тикет не найден");
            var (p, err) = TicketParticipant.Create(t.OrganizationId, ticketId, userId, role);
            if (err != null) throw new DomainException(err);
            await _participantRepo.AddAsync(p);
        }

        public async Task RemoveParticipantAsync(Guid ticketId, Guid userId) =>
            await _participantRepo.RemoveByTicketAndUserAsync(ticketId, userId);

        public async Task<List<TicketParticipant>> GetParticipantsAsync(Guid ticketId) =>
            await _participantRepo.GetByTicketAsync(ticketId);

        public async Task<int> CheckAndMarkOverdueAsync()
        {
            var overdue = await _ticketRepo.GetOverdueAsync();
            foreach (var t in overdue) { t.CheckSLA(); await _ticketRepo.UpdateAsync(t); }
            return overdue.Count;
        }
    }
}
