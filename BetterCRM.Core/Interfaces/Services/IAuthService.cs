namespace BetterCRM.Core.Interfaces.Services
{
    public record LoginCommand(string Email, string Password);
    public record RegisterCommand(string OrganizationName, string FullName, string Email, string Password, string PositionTitle);
    public record CurrentUserInfo(Guid Id, string Email, string FullName, string Role, Guid OrganizationId, Guid? DepartmentId, bool IsMainDirector);
    public record AuthResult(CurrentUserInfo User, string Token);
    public interface IAuthService
    {
        Task<AuthResult?> LoginAsync(LoginCommand command);
        Task<AuthResult> RegisterAsync(RegisterCommand command);
    }
}
