using System;
using System.Collections.Generic;
using System.Text;

namespace BetterCRM.Core.Models
{
    public class WorkSession : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid? TicketId { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime? EndedAt { get; private set; }
        public string? Description { get; private set; }

        public User User { get; private set; } = null!;
        public Ticket? Ticket { get; private set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public decimal DurationHours => EndedAt.HasValue
            ? (decimal)(EndedAt.Value - StartedAt).TotalHours
            : 0m;

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public bool IsActive => !EndedAt.HasValue;

        public const int MaxDescriptionLength = 300;
        public const int MaxSessionHours = 24;

        private WorkSession() { }

        public static (WorkSession? session, string? error) Start(Guid userId, Guid? ticketId = null)
        {
            if (userId == Guid.Empty)
                return (null, "Некорректный ID пользователя");

            return (new WorkSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TicketId = ticketId,
                StartedAt = DateTime.UtcNow,
                EndedAt = null,
                Description = null
            }, null);
        }

        public (bool success, string? error) Stop(string? description = null)
        {
            if (EndedAt.HasValue)
                return (false, "Сессия уже завершена");

            if (!string.IsNullOrWhiteSpace(description) && description.Length > MaxDescriptionLength)
                return (false, $"Описание не может превышать {MaxDescriptionLength} символов");

            var endedAt = DateTime.UtcNow;
            var duration = (endedAt - StartedAt).TotalHours;

            if (duration > MaxSessionHours)
                return (false, $"Сессия не может длиться более {MaxSessionHours} часов");

            EndedAt = endedAt;
            Description = description?.Trim();
            MarkAsUpdated();

            return (true, null);
        }

        public void UpdateDescription(string newDescription)
        {
            if (!string.IsNullOrWhiteSpace(newDescription) && newDescription.Length > MaxDescriptionLength)
                throw new InvalidOperationException($"Описание не может превышать {MaxDescriptionLength} символов");

            Description = newDescription?.Trim();
            MarkAsUpdated();
        }

        public bool IsForTicket(Guid ticketId) => TicketId == ticketId;
        public bool IsForUser(Guid userId) => UserId == userId;
        public bool OverlapsWith(DateTime start, DateTime end) =>
            StartedAt < end && (!EndedAt.HasValue || EndedAt.Value > start);
    }
}
