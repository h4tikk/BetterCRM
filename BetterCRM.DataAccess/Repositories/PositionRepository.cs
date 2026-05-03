using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;


namespace BetterCRM.DataAccess.Repositories
{
    public class PositionRepository : Repository<Position, PositionEntity>, IPositionRepository
    {
        public PositionRepository(ApplicationDbContext context) : base(context) { }
        protected override Position MapToDomain(PositionEntity db) => DomainMapper.ToPositionDomain(db);
        protected override PositionEntity MapToDb(Position domain, PositionEntity? existing = null) => DomainMapper.ToPositionDb(domain, existing);

        public Task<Position?> GetByTitleAsync(string title)
        {
            throw new NotImplementedException();
        }

        public Task<List<Position>> GetByDepartmentAsync(Guid departmentId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TitleExistsInDepartmentAsync(string title, Guid departmentId)
        {
            throw new NotImplementedException();
        }
    }
}
