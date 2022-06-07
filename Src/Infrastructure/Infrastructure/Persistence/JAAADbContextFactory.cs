using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Class JAAADbContextFactory
    /// </summary>
    public class JAAADbContextFactory : DesignTimeDbContextFactoryBase<JAAADbContext>
    {
        /// <summary>
        /// Creates new instance of <see cref="JAAADbContext"/>
        /// </summary>
        /// <param name="options">DbContext Options</param>
        /// <returns><see cref="JAAADbContext"/></returns>
        protected override JAAADbContext CreateNewInstance(DbContextOptions<JAAADbContext> options)
        {
            return new JAAADbContext(options);
        }
    }
}
