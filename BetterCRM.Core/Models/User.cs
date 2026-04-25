using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;

namespace BetterCRM.Core.Models
{
    public class User : BaseEntity
    {
        public string Email { get; private set; } = string .Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string FullName { get; private set; } = string.Empty;
        public string Role { get; private set; } = string.Empty; 
        public Guid? DepartmentId { get; private set; }
        public Guid PositionId { get; private set; }
        public DateTime HireDate { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Department? Department { get; private set; }
        public Position Position { get; private set; } = null!;
        public ICollection<Ticket> CreatedTickets { get; private set; } = new List<Ticket>();
        public ICollection<Ticket> AssignedTickets { get; private set; } = new List<Ticket>();
        public ICollection<WorkSession> WorkSessions { get; private set; } = new List<WorkSession>();
        public ICollection<TicketParticipant> TicketParticipations { get; private set; } = new List<TicketParticipant>();
        public ICollection<PayrollRecord> PayrollRecords { get; private set; } = new List<PayrollRecord>();

        public const int MinEmailLength = 5;
        public const int MaxEmailLength = 256;
        public const int MinPasswordLength = 6;
        public const int MinFullNameLength = 2;
        public const int MaxFullNameLength = 150;
        public static readonly string[] ValidRoles = { "Admin", "DepartmentHead", "Employee" };

        private User() { }

        public static (User? user, string? error) Create(
            string email,
            string password,
            string fullName,
            string role,
            Guid positionId,
            Guid? departmentId = null,
            DateTime? hireDate = null)
        {
            email = email.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(email) || email.Length < MinEmailLength || email.Length > MaxEmailLength)
                return (null, $"Некорректный email (от {MinEmailLength} до {MaxEmailLength} символов)");

            if (!email.Contains("@") || !email.Contains("."))
                return (null, "Некорректный формат email");

            if (string.IsNullOrWhiteSpace(password) || password.Length < MinPasswordLength)
                return (null, $"Пароль должен содержать минимум {MinPasswordLength} символов");

            fullName = fullName.Trim();
            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length < MinFullNameLength || fullName.Length > MaxFullNameLength)
                return (null, $"ФИО должно содержать от {MinFullNameLength} до {MaxFullNameLength} символов");

            if (!ValidRoles.Contains(role))
                return (null, $"Недопустимая роль. Доступные: {string.Join(", ", ValidRoles)}");

            var passwordHash = HashPassword(password);

            return (new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = passwordHash,
                FullName = fullName,
                Role = role,
                PositionId = positionId,
                DepartmentId = departmentId,
                HireDate = hireDate ?? DateTime.UtcNow.Date,
                IsActive = true
            }, null);
        }
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + "CourseProjectSalt2026");
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string password)
        {
            return PasswordHash == HashPassword(password);
        }

        public void Activate() { IsActive = true; MarkAsUpdated(); }
        public void Deactivate() { IsActive = false; MarkAsUpdated(); }

        public void UpdateEmail(string newEmail)
        {
            var (_, error) = Create(newEmail, "temp", FullName, Role, PositionId, DepartmentId, HireDate);
            if (error != null) throw new InvalidOperationException(error);

            Email = newEmail.Trim().ToLower();
            MarkAsUpdated();
        }

        ublic void UpdatePassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < MinPasswordLength)
                throw new InvalidOperationException($"Пароль должен содержать минимум {MinPasswordLength} символов");

            PasswordHash = HashPassword(newPassword);
            MarkAsUpdated();
        }

        public void UpdateFullName(string newFullName)
        {
            newFullName = newFullName.Trim();
            if (string.IsNullOrWhiteSpace(newFullName) || newFullName.Length < MinFullNameLength || newFullName.Length > MaxFullNameLength)
                throw new InvalidOperationException($"Некорректное ФИО");

            FullName = newFullName;
            MarkAsUpdated();
        }

        public void AssignToDepartment(Guid? departmentId)
        {
            DepartmentId = departmentId;
            MarkAsUpdated();
        }

        public void ChangeRole(string newRole)
        {
            if (!ValidRoles.Contains(newRole))
                throw new InvalidOperationException($"Недопустимая роль: {newRole}");

            Role = newRole;
            MarkAsUpdated();
        }
    }
}
