using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterCRM.DataAccess.Configurations
{
    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessageEntity>
    {
        public void Configure(EntityTypeBuilder<ChatMessageEntity> builder)
        {
            builder.ToTable("chat_messages");

            builder.Property(m => m.Text)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(m => m.SentAt)
                .IsRequired()
                .HasColumnType("timestamptz");

            builder.Property(m => m.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(m => m.MessageType)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("text");

            builder.Property(m => m.AttachmentObjectName)
                .HasMaxLength(1000);

            builder.Property(m => m.AttachmentName)
                .HasMaxLength(500);

            builder.Property(m => m.AttachmentMime)
                .HasMaxLength(100);

            builder.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Recipient)
                .WithMany()
                .HasForeignKey(m => m.RecipientId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(m => new { m.SenderId, m.RecipientId, m.SentAt })
                .HasDatabaseName("IX_chat_messages_private");

            builder.HasIndex(m => new { m.ChatRoomId, m.SentAt })
                .HasDatabaseName("IX_chat_messages_room")
                .HasFilter("\"ChatRoomId\" IS NOT NULL");

            builder.HasIndex(m => new { m.RecipientId, m.IsRead })
                .HasDatabaseName("IX_chat_messages_unread")
                .HasFilter("\"IsRead\" = false");
        }
    }
}
