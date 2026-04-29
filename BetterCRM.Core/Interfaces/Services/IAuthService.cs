namespace BetterCRM.Core.Interfaces.Services
{
    public record LoginCommand(string Email, string Password);
    public record RegisterCommand(string Email, string Password, string FullName, string Role, Guid OrganizationId, Guid PositionId, Guid? DepartmentId);
    public record CurrentUserInfo(Guid Id, string Email, string FullName, string Role, Guid OrganizationId, Guid? DepartmentId, bool IsMainDirector);
    public record AuthResult(CurrentUserInfo User, string Token);
    public interface IAuthService
    {
        Task<AuthResult?> LoginAsync(LoginCommand command);
        Task<CurrentUserInfo> RegisterAsync(RegisterCommand command);
        Task<CurrentUserInfo?> ValidateTokenAsync(string token);
    }
}
