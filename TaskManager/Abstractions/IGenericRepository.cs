using System.Linq.Expressions;

namespace TaskManager.Abstractions;

/// <summary>
/// The repository pattern abstracts the data access logic, 
/// hiding the specifics of how data is stored and retrieved. 
/// This allows you to change the underlying data storage without affecting the business logic.
/// Generic repository helps to to minimize redundant code.
/// </summary>
/// <typeparam name="TEntity"> An entity type </typeparam>
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