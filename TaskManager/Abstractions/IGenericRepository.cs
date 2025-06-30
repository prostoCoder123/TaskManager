using System.Linq.Expressions;

namespace TaskManager.Abstractions;

public interface IGenericRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Get(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "",
        (int pageNumber, int elementsOnPage)? paging = null);

    Task<TEntity?> GetByIdAsync(int? id, CancellationToken ct = default);

    Task InsertAsync(TEntity entityToInsert, CancellationToken ct = default);

    void Update(TEntity entityToUpdate);

    Task DeleteAsync(int id, CancellationToken ct = default);

    void Delete(TEntity entityToDelete);

    void DeleteRange(IEnumerable<TEntity> entitiesToDelete);

    Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter, CancellationToken ct = default);
}