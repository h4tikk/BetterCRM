namespace BetterCRM.Core.Models
{
    public class Department : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public ICollection<User> Users { get; private set; } = new List<User>();

        public const int MinNameLength = 2;
        public const int MaxNameLength = 100;

        private Department() { }

        public static (Department? department, string? error) Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return (null, "Название отдела не может быть пустым");

            name = name.Trim();
            if (name.Length < MinNameLength || name.Length > MaxNameLength) 
                return (null, $"Название должно содержать от {MinNameLength} до  {MaxNameLength} символов");
            return (new Department { Id = Guid.NewGuid(), Name = name }, null);
        }

        public void UpdateName(string name)
        {
            var (updated, error) = Create(name);
            if(error != null) throw new InvalidOperationException(error);
            Name = updated!.Name;
            MarkAsUpdated();
        }
    }
}
