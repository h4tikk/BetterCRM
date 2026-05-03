namespace BetterCRM.Core.Models
{
    public class Organization : BaseEntity
    {
        public string Name { get; internal set; } = string.Empty;
        public string Slug { get; internal set; } = string.Empty;
        public bool IsActive { get; internal set; } = true;
        public Guid? MainDirectorId { get; internal set; }
        public User? MainDirector { get; internal set; }

        public const int MaxNameLength = 100;
        public const int MinNameLength = 2;

        private Organization() { }

        public static (Organization? org, string? error) Create(string name)
        {
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(name) || name.Length < MinNameLength || name.Length > MaxNameLength)
                return (null, $"Название от {MinNameLength} до {MaxNameLength} символов");

            return (new Organization
            {
                Id = Guid.NewGuid(),
                Name = name,
                Slug = GenerateSlug(name),
                IsActive = true
            }, null);
        }

        public void AssignMainDirector(Guid userId) => MainDirectorId = userId;
        public bool IsMainDirector(Guid userId) => MainDirectorId == userId;

        internal static string GenerateSlug(string name) =>
            new string(name.ToLower().Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());
    }
}
