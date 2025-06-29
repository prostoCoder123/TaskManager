using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Abstractions;
using TaskManager.Extensions;

namespace TaskManager.Implementations;

public class GenericRepository<TEntity>(DbContext _context) : IGenericRepository<TEntity> where TEntity : class
{
    private readonly DbContext p_context = _context;
    private readonly DbSet<TEntity> p_dbSet = _context.Set<TEntity>();

    public IQueryable<TEntity> Get(
        Expression<Func<TEntity, bool>>? _filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? _orderBy = null,
        string _includeProperties = "",
        (int pageNumber, int elementsOnPage)? _paging = null) =>
      Get(_filter).ApplyOrderBy(_orderBy).ApplyPaging(_paging).ApplyInclude(_includeProperties);

    public TEntity? GetById(int? _id) => p_dbSet.Find(_id);

    public void Insert(TEntity _entityToInsert) => _ = p_dbSet.Add(_entityToInsert);

    public void Update(TEntity _entityToUpdate)
    {
        _ = p_dbSet.Attach(_entityToUpdate);
        p_context.Entry(_entityToUpdate).State = EntityState.Modified; // the entity with all the props is modified !!!
    }

    public void Delete(int _id)
    {
        TEntity? entityToDelete = p_dbSet.Find(_id);
        if (entityToDelete != null)
        {
            Delete(entityToDelete);
        }
    }

    public void Delete(TEntity _entityToDelete)
    {
        if (p_context.Entry(_entityToDelete).State == EntityState.Detached)
        {
            _ = p_dbSet.Attach(_entityToDelete);
        }

        _ = p_dbSet.Remove(_entityToDelete);
    }

    public void DeleteRange(IEnumerable<TEntity> _entitiesToDelete)
    {
        foreach (TEntity entityToDelete in _entitiesToDelete)
        {
            if (p_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _ = p_dbSet.Attach(entityToDelete);
            }
        }

        p_dbSet.RemoveRange(_entitiesToDelete);
    }

    public int Count(Func<TEntity, bool>? _filter = null) => _filter == null ? p_dbSet.Count() : p_dbSet.Count(_filter);

    private IQueryable<TEntity> Get(Expression<Func<TEntity, bool>>? _filter = null)
    {
        IQueryable<TEntity> query = p_dbSet;

        if (_filter != null)
        {
            query = query.Where(_filter);
        }

        return query;
    }
}