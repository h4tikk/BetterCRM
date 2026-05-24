using BetterCRM.Business.Exceptions;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.Core.Models;
using BetterCRM.Core.Constants;
using static BetterCRM.Business.Exceptions.DomainException;

namespace BetterCRM.Business.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly ITicketParticipantRepository _participantRepo;
        private readonly IUserRepository _userRepo;

        public TicketService(
            ITicketRepository ticketRepo,
            ITicketParticipantRepository participantRepo,
            IUserRepository userRepo)
        {
            _ticketRepo = ticketRepo;
            _participantRepo = participantRepo;
            _userRepo = userRepo;
        }

        public async Task<Ticket> CreateAsync(CreateTicketCommand cmd)
        {
            var creator = await _userRepo.GetByIdAsync(cmd.CreatorId)
                ?? throw new NotFoundException("Создатель не найден");

            var (ticket, err) = Ticket.Create(
                creator.OrganizationId,
                cmd.Title,
                cmd.Description,
                cmd.Priority,
                cmd.CreatorId,
                cmd.DepartmentId,
                cmd.AssigneeId);
            if (ticket == null) throw new DomainException(err!);

            return await _ticketRepo.AddAsync(ticket);
        }

        public async Task<Ticket?> GetByIdAsync(Guid id) =>
            await _ticketRepo.GetByIdAsync(id);

        public async Task<List<Ticket>> GetFilteredAsync(Guid userId, string role, Guid? departmentId) =>
            await _ticketRepo.GetForUsersAsync(userId, role, departmentId);

        public async Task ApproveAsync(Guid ticketId, Guid approverId)
        {
            var approver = await _userRepo.GetByIdAsync(approverId)
                ?? throw new NotFoundException("Пользователь не найден");

            if (approver.Role == Roles.Employee)
                throw new UnauthorizedOperationException("Недостаточно прав для одобрения тикета");

            var ticket = await _ticketRepo.GetByIdAsync(ticketId)
                ?? throw new NotFoundException("Тикет не найден");

            ticket.Approve();
            await _ticketRepo.UpdateAsync(ticket);
        }

        public async Task RejectAsync(Guid ticketId, Guid rejecterId)
        {
            var rejecter = await _userRepo.GetByIdAsync(rejecterId)
                ?? throw new NotFoundException("Пользователь не найден");

            if (rejecter.Role == Roles.Employee)
                throw new UnauthorizedOperationException("Недостаточно прав для отклонения тикета");

            var ticket = await _ticketRepo.GetByIdAsync(ticketId)
                ?? throw new NotFoundException("Тикет не найден");

            ticket.Reject();
            await _ticketRepo.UpdateAsync(ticket);
        }

        public async Task ResolveAsync(Guid ticketId, Guid resolverId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId)
                ?? throw new NotFoundException("Тикет не найден");

            var isAssignee = ticket.AssigneeId == resolverId;
            var isParticipant = await _participantRepo.IsParticipantAsync(ticketId, resolverId);
            var resolver = await _userRepo.GetByIdAsync(resolverId)
                ?? throw new NotFoundException("Пользователь не найден");
            var isManager = resolver.Role is "Admin" or "OrganizationHead" or "DepartmentHead";

            if (!isAssignee && !isParticipant && !isManager)
                throw new UnauthorizedOperationException("Только исполнитель или участник может закрыть тикет");

            ticket.Resolve();
            await _ticketRepo.UpdateAsync(ticket);
        }

        public async Task CloseAsync(Guid ticketId, Guid closerId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId)
                ?? throw new NotFoundException("Тикет не найден");

            var closer = await _userRepo.GetByIdAsync(closerId)
                ?? throw new NotFoundException("Пользователь не найден");
            var isCreator = ticket.CreatorId == closerId;
            var isManager = closer.Role is Roles.OrganizationHead or Roles.DepartmentHead;

            if (!isCreator && !isManager)
                throw new UnauthorizedOperationException("Только создатель или менеджер может закрыть тикет");

            ticket.Close();
            await _ticketRepo.UpdateAsync(ticket);
        }

        public async Task AddParticipantAsync(Guid ticketId, Guid userId, string role, Guid requesterId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId)
                ?? throw new NotFoundException("Тикет не найден");

            var requester = await _userRepo.GetByIdAsync(requesterId)
                ?? throw new NotFoundException("Запрашивающий не найден");

            var isAssignee = ticket.AssigneeId == requesterId;
            var isManager = requester.Role is "OrganizationHead" or "DepartmentHead";

            if (!isAssignee && !isManager)
                throw new UnauthorizedOperationException("Только исполнитель или менеджер может добавлять участников");

            if (ticket.CreatorId == userId || ticket.AssigneeId == userId)
                throw new ConflictException("Пользователь уже связан с тикетом в роли создателя или исполнителя");

            if (await _participantRepo.IsParticipantAsync(ticketId, userId))
                throw new ConflictException("Участник уже добавлен");


            var (p, err) = TicketParticipant.Create(ticket.OrganizationId, ticketId, userId, role);
            if (p == null) throw new DomainException(err!);

            await _participantRepo.AddAsync(p);
        }

        public async Task RemoveParticipantAsync(Guid ticketId, Guid userId, Guid requesterId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId)
                ?? throw new NotFoundException("Тикет не найден");

            var requester = await _userRepo.GetByIdAsync(requesterId)
                ?? throw new NotFoundException("Запрашивающий не найден");

            var isSelf = requesterId == userId;
            var isAssignee = ticket.AssigneeId == requesterId;
            var isManager = requester.Role is Roles.OrganizationHead or Roles.DepartmentHead;

            if (!isSelf && !isAssignee && !isManager)
                throw new UnauthorizedOperationException("Недостаточно прав для удаления участника");

            await _participantRepo.RemoveByTicketAndUserAsync(ticketId, userId);
        }

        public async Task<List<TicketParticipant>> GetParticipantsAsync(Guid ticketId) =>
            await _participantRepo.GetByTicketAsync(ticketId);

        public async Task<int> CheckAndMarkOverdueAsync()
        {
            var overdue = await _ticketRepo.GetOverdueAsync();
            var processed = 0;
            foreach (var t in overdue)
            {
                try
                {
                    t.CheckSLA();
                    if (t.IsSLABreached && t.OverduePenaltyHours == 0)
                        t.ApplyOverduePenalty();
                    await _ticketRepo.UpdateAsync(t);
                    processed++;
                }
                catch (InvalidOperationException) { }
            }
            return overdue.Count;
        }
    }
}