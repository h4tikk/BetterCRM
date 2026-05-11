using System;
using System.Collections.Generic;
using System.Text;

namespace BetterCRM.DataAccess.Entities
{
    public class NotificationEntity : TenantDbEntity
    {
        public Guid UserId { get; set; }
        public Guid? TicketId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public UserEntity User { get; set; } = null!;
        public TicketEntity? Ticket { get; set; }
    }
}
