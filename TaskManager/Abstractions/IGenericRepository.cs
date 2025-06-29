using System.Linq.Expressions;

namespace TaskManager.Abstractions;

public interface IGenericRepository<TEntity> where TEntity : class
{
  IQueryable<TEntity> Get(
      Expression<Func<TEntity, bool>>? _filter = null,
      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? _orderBy = null,
      string _includeProperties = "",
      (int pageNumber, int elementsOnPage)? _paging = null);

  TEntity? GetById(int? _id);

  void Insert(TEntity _entityToInsert);

  void Update(TEntity _entityToUpdate);

  void Delete(int _id);

  void Delete(TEntity _entityToDelete);

  void DeleteRange(IEnumerable<TEntity> _entitiesToDelete);

  int Count(Func<TEntity, bool>? _filter = null);
}