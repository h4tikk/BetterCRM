using BetterCRM.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetterCRM.Core.Interfaces.Services
{
    public record CreateUserCommand(
        string FullName,
        string Email,
        string Password,
        string Role,
        Guid PositionId,
        Guid? DepartmentId
    );
    public interface IUserManagementService
    {
        Task<User> CreateUserAsync(CurrentUserInfo creator, CreateUserCommand command);
    }
}
