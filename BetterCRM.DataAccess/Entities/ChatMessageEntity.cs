using System;
using System.Collections.Generic;
using System.Text;

namespace BetterCRM.DataAccess.Entities
{
    public class ChatMessageEntity : TenantDbEntity
    {
        public Guid SenderId { get; set; }
        public UserEntity Sender { get; set; } = null!;

        public Guid? RecipientId { get; set; }   // null для группового
        public UserEntity? Recipient { get; set; }

        public Guid? ChatRoomId { get; set; }   // = departmentId

        public string Text { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
