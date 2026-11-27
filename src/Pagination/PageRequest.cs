using System.Text.Json.Serialization;
using Atya.Foundation.Guards;
using Atya.Foundation.Results;

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
    /// Gets the zero-based item offset for this page as a 32-bit value.
    /// </summary>
    /// <exception cref="OverflowException">The offset does not fit in a 32-bit signed integer.</exception>
    [Obsolete("Use LongOffset, the canonical 64-bit offset. This property is removed in 2.0.")]
    [JsonIgnore]
    public int Offset => checked((int)LongOffset);

    /// <summary>
    /// Gets the zero-based item offset for this page. This is the canonical offset. It is always non-negative and
    /// cannot overflow: the largest value is (Int32.MaxValue - 1) * Int32.MaxValue.
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

        return Result.Success(new PageRequest(pageNumber, pageSize));
    }

    /// <summary>
    /// Creates a validated page request whose page size is clamped to the maximum of an explicit policy.
    /// </summary>
    /// <param name="pageNumber">The one-based page number.</param>
    /// <param name="pageSize">The requested page size. Values above <see cref="PageSizePolicy.MaxPageSize"/> are clamped.</param>
    /// <param name="policy">The page-size policy. It is passed explicitly; there is no global setting.</param>
    /// <returns>A successful result containing the request, or a validation failure.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="policy"/> is <see langword="null"/>.</exception>
    public static Result<PageRequest> Create(int pageNumber, int pageSize, PageSizePolicy policy)
    {
        Guard.AgainstNull(policy);

        return pageSize <= 0
            ? Create(pageNumber, pageSize)
            : Create(pageNumber, policy.Clamp(pageSize));
    }
}
