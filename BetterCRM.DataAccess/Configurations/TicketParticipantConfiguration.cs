using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BetterCRM.Core.Models;

namespace BetterCRM.DataAccess.Configurations
{
    public class TicketParticipantConfiguration : IEntityTypeConfiguration<TicketParticipant>
    {
        public void Configure(EntityTypeBuilder<TicketParticipant> builder)
        {
            builder.ToTable
        }
    }
}
