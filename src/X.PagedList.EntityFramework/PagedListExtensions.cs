using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace X.PagedList.EntityFramework;

/// <summary>
/// EntityFramework extension methods designed to simplify the creation of instances of <see cref="PagedList{T}"/>.
/// </summary>
public static class PagedListExtensions
{
    /// <summary>
    /// Async creates a subset of this collection of objects that can be individually accessed by index and
    /// containing metadata about the collection of objects the subset was created from.
    /// </summary>
    /// <typeparam name="T">The type of object the collection should contain.</typeparam>
    /// <param name="superset">The collection of objects to be divided into subsets. If the collection implements <see cref="IQueryable{T}"/>, it will be treated as such.</param>
    /// <param name="pageNumber">The one-based index of the subset of objects to be contained by this instance.</param>
    /// <param name="pageSize">The maximum size of any individual subset.</param>
    /// <param name="totalSetCount">The total size of set.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A subset of this collection of objects that can be individually accessed by index and containing metadata
    /// about the collection of objects the subset was created from.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="superset"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="pageNumber"/> is less than 1, <paramref name="pageSize"/> is less than 1, or <paramref name="totalSetCount"/> is negative.</exception>
    /// <exception cref="InvalidOperationException">Thrown by EF6 if <paramref name="superset"/> does not have an OrderBy clause.</exception>
    /// <seealso cref="PagedList{T}"/>
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> superset, int pageNumber, int pageSize, int? totalSetCount = null, CancellationToken cancellationToken = default)
    {
        if (superset == null)
        {
            throw new ArgumentNullException(nameof(superset));
        }

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "pageNumber cannot be below 1");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "pageSize cannot be less than 1");
        }

        if (totalSetCount.HasValue && totalSetCount.Value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalSetCount), totalSetCount.Value, "totalSetCount cannot be negative");
        }

        int totalCount = totalSetCount ?? await superset
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);

        int skip = (pageNumber - 1) * pageSize;

        List<T> subset;
        if (skip < totalCount)
        {
            subset = await superset
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            subset = new List<T>();
        }

        return new StaticPagedList<T>(subset, pageNumber, pageSize, totalCount);
    }
}
