using Microsoft.EntityFrameworkCore;

namespace TaskManager.Extensions;

public static class IQueryableExtensions
{
  public static IQueryable<TEntity> ApplyOrderBy<TEntity>(
  this IQueryable<TEntity> _source,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? _orderBy) =>
    _orderBy == null ? _source : _orderBy(_source);

  public static IQueryable<TEntity> ApplyInclude<TEntity>(
    this IQueryable<TEntity> _source,
    string _includedPaths = "") where TEntity : class
  {
    if (!string.IsNullOrWhiteSpace(_includedPaths))
    {
      foreach (string includedProperty in _includedPaths.Split
        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
      {
        _source = EntityFrameworkQueryableExtensions.Include(_source, includedProperty);
      }
    }

    return _source;
  }

  /// <summary>
  /// Apply pagination. OrderBy/OrderByDescending methods should be invoked before Skip/Take !!!
  /// </summary>
  public static IQueryable<TEntity> ApplyPaging<TEntity>(
    this IQueryable<TEntity> _source,
    (int pageNumber, int elementsOnPage)? _paging) =>
    _paging == null || _paging.Value.pageNumber < 0 || _paging.Value.elementsOnPage < 1
      // LIMIT MAX NUMBER OF ENTITIES if one of page/elements was incorrect or elements > 1000
      ? _source.Take(Constants.MaxElementsOnPage)
      : _source.Skip(_paging.Value.pageNumber * _paging.Value.elementsOnPage)
               .Take(int.Min(_paging.Value.elementsOnPage, Constants.MaxElementsOnPage));
}
