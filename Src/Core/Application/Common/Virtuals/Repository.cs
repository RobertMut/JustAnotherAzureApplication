﻿using Application.Common.Interfaces.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Common.Virtuals;

public class Repository<TEntity> where TEntity : class
{
    private IJAAADbContext _dbContext;
    private DbSet<TEntity?> _dbSet;
    
    public Repository(IJAAADbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TEntity>();
    }

    /// <summary>
    /// Deletes from repository
    /// </summary>
    /// <param name="entity">Entity</param>
    public virtual async Task Delete(TEntity? entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
    }

    /// <summary>
    /// Gets entities by filter
    /// </summary>
    /// <param name="filter">Expression filter</param>
    /// <param name="orderBy">Order by function</param>
    /// <param name="includeProperties">Properties</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>IEnumerable of entities</returns>
    public virtual async Task<IEnumerable<TEntity?>> GetAsync(Expression<Func<TEntity?, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedEnumerable<TEntity>> orderBy = null, string includeProperties = "", CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity?> query = _dbSet;

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

    /// <summary>
    /// Gets single entity object by filter
    /// </summary>
    /// <param name="filter">Expression filter</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Entity</returns>
    public virtual async Task<TEntity?> GetObjectBy(Expression<Func<TEntity?, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsQueryable().Where(filter).FirstOrDefaultAsync(cancellationToken);

    }

    /// <summary>
    /// Inserts to dbset
    /// </summary>
    /// <param name="entity">Entity to be insterted</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Entity</returns>
    public virtual async Task<TEntity?> InsertAsync(TEntity? entity, CancellationToken cancellationToken = default)
    {
        var entry = await _dbSet.AddAsync(entity, cancellationToken);
            
        return entry.Entity;
    }

    /// <summary>
    /// Updates entity
    /// </summary>
    /// <param name="entity">Entity to be updated</param>
    public virtual async Task UpdateAsync(TEntity? entity)
    {
        _dbSet.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;
    }

}