using Atya.Foundation.Results;

namespace Atya.Data.Pagination;

/// <summary>
/// An explicit, non-global page-size policy that clamps oversized requested page sizes.
/// </summary>
public sealed record PageSizePolicy
{
    private PageSizePolicy(int maxPageSize)
    {
        MaxPageSize = maxPageSize;
    }

    /// <summary>
    /// Gets the largest page size that <see cref="PageRequest.Create(int, int, PageSizePolicy)"/> will produce.
    /// </summary>
    public int MaxPageSize { get; }

    /// <summary>
    /// Creates a policy.
    /// </summary>
    /// <param name="maxPageSize">The positive maximum page size.</param>
    /// <returns>A successful result containing the policy, or a validation failure.</returns>
    public static Result<PageSizePolicy> Create(int maxPageSize) =>
        maxPageSize <= 0
            ? Result.Failure<PageSizePolicy>(PaginationErrors.MaxPageSizeMustBePositive())
            : Result.Success(new PageSizePolicy(maxPageSize));

    /// <summary>
    /// Returns the page size limited to <see cref="MaxPageSize"/>.
    /// </summary>
    /// <param name="pageSize">The requested page size.</param>
    /// <returns>The smaller of <paramref name="pageSize"/> and <see cref="MaxPageSize"/>.</returns>
    public int Clamp(int pageSize) => Math.Min(pageSize, MaxPageSize);
}
