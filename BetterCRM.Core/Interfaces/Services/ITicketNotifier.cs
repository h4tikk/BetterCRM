namespace BetterCRM.Core.Interfaces.Services
{
    public interface ITicketNotifier
    {
        Task SendNotificationAsync(object payload, Guid recipientId);
        Task UpdateUnreadCountAsync(Guid userId, int count);
    }
}
