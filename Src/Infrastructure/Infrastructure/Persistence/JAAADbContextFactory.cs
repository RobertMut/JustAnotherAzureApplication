using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class JAAADbContextFactory : DesignTimeDbContextFactoryBase<JAAADbContext>
    {
        protected override JAAADbContext CreateNewInstance(DbContextOptions<JAAADbContext> options)
        {
            return new JAAADbContext(options);
        }
    }
}
