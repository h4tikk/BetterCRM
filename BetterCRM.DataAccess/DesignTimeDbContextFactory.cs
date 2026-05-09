using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace BetterCRM.DataAccess
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=bettercrm;Username=postgres;Password=test123");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}