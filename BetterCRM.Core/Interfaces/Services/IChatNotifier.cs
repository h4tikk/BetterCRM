namespace BetterCRM.Core.Interfaces.Services
{
    public interface IChatNotifier
    {
        Task SendPrivateMessageAsync(object payload, Guid recipientId);
        Task SendDepartmentMessageAsync(object payload, Guid departmentId);
    }
}
