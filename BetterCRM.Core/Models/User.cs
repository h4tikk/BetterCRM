using System.Security.Cryptography;
using System.Text;

namespace BetterCRM.Core.Models
{
    public class User : TenantEntity
    {
        public string Email { get; internal set; } = string.Empty;
        public string PasswordHash { get; internal set; } = string.Empty;
        public string FullName { get; internal set; } = string.Empty;
        public string Role { get; internal set; } = string.Empty;
        public Guid? DepartmentId { get; internal set; }
        public Guid PositionId { get; internal set; }
        public DateTime HireDate { get; internal set; }
        public bool IsActive { get; internal set; } = true;

        public Department? Department { get; internal set; }
        public Position Position { get; internal set; } = null!;
        public ICollection<Ticket> CreatedTickets { get; internal set; } = new List<Ticket>();
        public ICollection<Ticket> AssignedTickets { get; internal set; } = new List<Ticket>();
        public ICollection<WorkSession> WorkSessions { get; internal set; } = new List<WorkSession>();
        public ICollection<TicketParticipant> TicketParticipations { get; internal set; } = new List<TicketParticipant>();
        public ICollection<PayrollRecord> PayrollRecords { get; internal set; } = new List<PayrollRecord>();
        public ICollection<Shift> Shifts { get; internal set; } = new List<Shift>();

        public const int MinEmailLength = 5;
        public const int MaxEmailLength = 256;
        public const int MinPasswordLength = 6;
        public const int MinFullNameLength = 2;
        public const int MaxFullNameLength = 150;
        public static readonly string[] ValidRoles = { "Admin", "DepartmentHead", "Employee" };

        private User() { }

        public static (User? user, string? error) Create(
            Guid organizationId,
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
                OrganizationId = organizationId,
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
        internal static string HashPassword(string password)
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
    }
}
