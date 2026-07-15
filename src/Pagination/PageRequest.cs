using Atya.Foundation.Primitives.Results;
using PrimitivePagedRequest = Atya.Foundation.Primitives.Paging.PagedRequest;

namespace Atya.Data.Pagination;

/// <summary>
/// Represents a validated one-based page request.
/// </summary>
public sealed record PageRequest
{
    /// <summary>
    /// The default page number used when callers do not provide one.
    /// </summary>
    public const int DefaultPageNumber = 1;

    /// <summary>
    /// The default page size used when callers do not provide one.
    /// </summary>
    public const int DefaultPageSize = 20;

    private PageRequest(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// Gets the one-based page number.
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the positive page size.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the zero-based item offset for this page.
    /// </summary>
    public int Offset => checked((PageNumber - 1) * PageSize);

    /// <summary>
    /// Gets the zero-based item offset for this page as a 64-bit value.
    /// </summary>
    public long LongOffset => ((long)PageNumber - 1) * PageSize;

    /// <summary>
    /// Creates a validated page request.
    /// </summary>
    /// <param name="pageNumber">The one-based page number.</param>
    /// <param name="pageSize">The positive page size.</param>
    /// <returns>A successful result containing the request, or a validation failure.</returns>
    public static Result<PageRequest> Create(
        int pageNumber = DefaultPageNumber,
        int pageSize = DefaultPageSize)
    {
        if (pageNumber <= 0)
        {
            return Result.Failure<PageRequest>(PaginationErrors.PageNumberMustBePositive());
        }

        if (pageSize <= 0)
        {
            return Result.Failure<PageRequest>(PaginationErrors.PageSizeMustBePositive());
        }

        if (((long)pageNumber - 1) * pageSize > int.MaxValue)
        {
            return Result.Failure<PageRequest>(PaginationErrors.OffsetMustFitInt32());
        }

        return Result.Success(new PageRequest(pageNumber, pageSize));
    }

    /// <summary>
    /// Converts this request to the Foundation.Primitives paging shape.
    /// </summary>
    /// <returns>The equivalent primitive paged request.</returns>
    public PrimitivePagedRequest ToPrimitiveRequest() => new(PageNumber, PageSize);
}
