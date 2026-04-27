using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Services
{
    public record AuthResult(User User, string Token);
    public record RegisterCommand(string Email, string Password, string FullName, string Role, Guid PositionId, Guid? DepartmentId);
    public record LoginCommand(string Email, string Password);
    public interface IAuthService
    {
        Task<AuthResult?> LoginAsync(LoginCommand command);
        Task<User> RegisterAsync(RegisterCommand command);
        Task<bool> ValidateTokenAsync(string token);
        Task<User?> GetUserByTokenAsync(string token);
    }
}
