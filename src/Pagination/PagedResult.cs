using System.Collections.ObjectModel;
using Atya.Foundation.Guards;
using Atya.Foundation.Results;

namespace Atya.Data.Pagination;

/// <summary>
/// Represents one page of items and the paging metadata needed by consumers.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public sealed record PagedResult<T>
{
    internal PagedResult(
        IReadOnlyCollection<T> items,
        PageRequest request,
        long totalCount)
    {
        Items = items;
        PageNumber = request.PageNumber;
        PageSize = request.PageSize;
        LongTotalCount = totalCount;
    }

    /// <summary>
    /// Gets the items in the current page. The collection is an immutable snapshot taken at creation.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; }

    /// <summary>
    /// Gets the one-based current page number.
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the positive page size.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of available items across all pages as a 64-bit value. This is the canonical total.
    /// </summary>
    public long LongTotalCount { get; }

    /// <summary>
    /// Gets the total number of available items across all pages. Totals above <see cref="int.MaxValue"/> are
    /// reported as <see cref="int.MaxValue"/>; use <see cref="LongTotalCount"/> for the exact value.
    /// </summary>
    public int TotalCount => (int)Math.Min(LongTotalCount, int.MaxValue);

    /// <summary>
    /// Gets the total number of pages as a 64-bit value, computed with integer arithmetic.
    /// </summary>
    public long LongTotalPages => (LongTotalCount / PageSize) + (LongTotalCount % PageSize == 0 ? 0 : 1);

    /// <summary>
    /// Gets the total number of pages. Values above <see cref="int.MaxValue"/> are reported as
    /// <see cref="int.MaxValue"/>; use <see cref="LongTotalPages"/> for the exact value.
    /// </summary>
    public int TotalPages => (int)Math.Min(LongTotalPages, int.MaxValue);

    /// <summary>
    /// Gets a value indicating whether a previous page exists.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets a value indicating whether a next page exists.
    /// </summary>
    public bool HasNextPage => PageNumber < LongTotalPages;
}

/// <summary>
/// Creates paged result instances.
/// </summary>
public static class PagedResult
{
    /// <summary>
    /// Creates a paged result from validated page request metadata.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="items">The current page items.</param>
    /// <param name="request">The validated page request.</param>
    /// <param name="totalCount">The total number of available items.</param>
    /// <returns>A successful result containing the page, or a validation failure.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="items"/> or <paramref name="request"/> is <see langword="null"/>.
    /// </exception>
    public static Result<PagedResult<T>> Create<T>(
        IReadOnlyCollection<T> items,
        PageRequest request,
        int totalCount) =>
        Create(items, request, (long)totalCount);

    /// <summary>
    /// Creates a paged result from validated page request metadata and a 64-bit total.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="items">
    /// The current page items. They are copied, so later changes to the source do not affect the result.
    /// </param>
    /// <param name="request">The validated page request.</param>
    /// <param name="totalCount">The total number of available items.</param>
    /// <returns>
    /// A successful result containing the page, or a validation failure when the total is negative, the items exceed the
    /// page size, or the item count contradicts the page position and the total (a sparse page, or items past the end).
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="items"/> or <paramref name="request"/> is <see langword="null"/>.
    /// </exception>
    public static Result<PagedResult<T>> Create<T>(
        IReadOnlyCollection<T> items,
        PageRequest request,
        long totalCount)
    {
        Guard.AgainstNull(items);
        Guard.AgainstNull(request);

        if (totalCount < 0)
        {
            return Result.Failure<PagedResult<T>>(PaginationErrors.TotalCountMustNotBeNegative());
        }

        if (items.Count > request.PageSize)
        {
            return Result.Failure<PagedResult<T>>(PaginationErrors.ItemCountMustNotExceedPageSize());
        }

        long remaining = totalCount > request.LongOffset ? totalCount - request.LongOffset : 0;
        if (items.Count != Math.Min(remaining, request.PageSize))
        {
            return Result.Failure<PagedResult<T>>(PaginationErrors.ItemCountMustMatchTotal());
        }

        var snapshot = new ReadOnlyCollection<T>(items.ToArray());

        return Result.Success(new PagedResult<T>(snapshot, request, totalCount));
    }

    /// <summary>
    /// Creates an empty page for a validated request.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="request">The validated page request.</param>
    /// <returns>A successful result containing an empty page.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    public static Result<PagedResult<T>> Empty<T>(PageRequest request) =>
        Create(Array.Empty<T>(), request, 0L);
}
