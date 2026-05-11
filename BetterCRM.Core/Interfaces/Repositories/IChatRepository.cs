using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface IChatRepository
    {
        Task SaveMessageAsync(ChatMessage message);

        Task<IEnumerable<ChatMessage>> GetPrivateMessagesAsync(
            Guid userId, Guid withUserId, int take, DateTime before);

        Task<IEnumerable<ChatMessage>> GetDepartmentMessagesAsync(
            Guid departmentId, int take, DateTime before);

        Task MarkAsReadAsync(Guid messageId, Guid readerId);
        Task<bool> UserBelongsToDepartmentAsync(Guid userId, Guid departmentId);
        Task<Guid?> GetUserDepartmentIdAsync(Guid userId);
    }
}

