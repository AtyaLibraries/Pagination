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
        int totalCount)
    {
        Items = items;
        PageNumber = request.PageNumber;
        PageSize = request.PageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// Gets the items in the current page.
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
    /// Gets the total number of available items across all pages.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => TotalCount == 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Gets a value indicating whether a previous page exists.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets a value indicating whether a next page exists.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
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
        int totalCount)
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

        return Result.Success(new PagedResult<T>(items, request, totalCount));
    }

    /// <summary>
    /// Creates an empty page for a validated request.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="request">The validated page request.</param>
    /// <returns>A successful result containing an empty page.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    public static Result<PagedResult<T>> Empty<T>(PageRequest request) =>
        Create(Array.Empty<T>(), request, 0);
}
