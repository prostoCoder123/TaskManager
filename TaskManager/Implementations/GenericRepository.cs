using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Abstractions;
using TaskManager.Extensions;

namespace TaskManager.Implementations;

public class GenericRepository<TEntity>(DbContext context) : IGenericRepository<TEntity> where TEntity : class
{
    private readonly DbContext context = context;
    private readonly DbSet<TEntity> dbSet = context.Set<TEntity>();

    public IQueryable<TEntity> Get(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "",
        (int pageNumber, int elementsOnPage)? paging = null) =>
      Get(filter).ApplyOrderBy(orderBy).ApplyPaging(paging).ApplyInclude(includeProperties);

    public async Task<TEntity?> GetByIdAsync(int? id, CancellationToken ct = default) => await dbSet.FindAsync(id, ct);

    public async Task InsertAsync(TEntity entityToInsert, CancellationToken ct = default) => await dbSet.AddAsync(entityToInsert, ct);

    public void Update(TEntity entityToUpdate)
    {
        _ = dbSet.Attach(entityToUpdate);
        context.Entry(entityToUpdate).State = EntityState.Modified; // the entity with all the props is modified !!!
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        TEntity? entityToDelete = await dbSet.FindAsync(id, ct);
        if (entityToDelete != null)
        {
            Delete(entityToDelete);
        }
    }

    public void Delete(TEntity entityToDelete)
    {
        if (context.Entry(entityToDelete).State == EntityState.Detached)
        {
            _ = dbSet.Attach(entityToDelete);
        }

        _ = dbSet.Remove(entityToDelete);
    }

    public void DeleteRange(IEnumerable<TEntity> entitiesToDelete)
    {
        foreach (TEntity entityToDelete in entitiesToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _ = dbSet.Attach(entityToDelete);
            }
        }

        dbSet.RemoveRange(entitiesToDelete);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken ct = default) =>
        filter == null ? await dbSet.CountAsync() : await dbSet.CountAsync(filter, ct);

    private IQueryable<TEntity> Get(Expression<Func<TEntity, bool>>? filter = null)
    {
        IQueryable<TEntity> query = dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return query;
    }
}