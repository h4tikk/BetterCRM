using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;


namespace BetterCRM.DataAccess.Repositories
{
    public class DepartmentRepository : Repository<Department, DepartmentEntity>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context) { }
        protected override Department MapToDomain(DepartmentEntity db) => DomainMapper.ToDepartmentDomain(db);
        protected override DepartmentEntity MapToDb(Department domain, DepartmentEntity? existing = null) => DomainMapper.ToDepartmentDb(domain, existing);

        public Task<Department?> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<bool> NameExistsAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<List<Department>> GetWithUsersCountAsync()
        {
            throw new NotImplementedException();
        }
    }
}
