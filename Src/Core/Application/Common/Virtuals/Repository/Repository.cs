using Application.Common.Interfaces.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Common.Virtuals.Repository
{
    public class Repository<TEntity> where TEntity : class
    {
        private IJAAADbContext _dbContext;
        private DbSet<TEntity> _dbSet;

        public Repository(IJAAADbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async virtual Task Delete(TEntity entity)
        {
            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        public async virtual Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedEnumerable<TEntity>> orderBy = null, string includeProperties = "", CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async virtual Task<TEntity> GetObjectBy(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(filter).FirstOrDefaultAsync(cancellationToken);

        }

        public async virtual Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var entry = await _dbSet.AddAsync(entity, cancellationToken);

            return entry.Entity;
        }

        public async virtual Task UpdateAsync(TEntity entity)
        {
            _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

    }
}
