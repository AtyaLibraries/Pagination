using Atya.Foundation.Primitives.Errors;

namespace Atya.Data.Pagination;

/// <summary>
/// Creates stable pagination validation errors.
/// </summary>
public static class PaginationErrors
{
    /// <summary>
    /// The page number validation error code.
    /// </summary>
    public const string PageNumberMustBePositiveCode = "atya.data.pagination.page_number_must_be_positive";

    /// <summary>
    /// The page size validation error code.
    /// </summary>
    public const string PageSizeMustBePositiveCode = "atya.data.pagination.page_size_must_be_positive";

    /// <summary>
    /// The offset overflow validation error code.
    /// </summary>
    public const string OffsetMustFitInt32Code = "atya.data.pagination.offset_must_fit_int32";

    /// <summary>
    /// The total count validation error code.
    /// </summary>
    public const string TotalCountMustNotBeNegativeCode = "atya.data.pagination.total_count_must_not_be_negative";

    /// <summary>
    /// The item count validation error code.
    /// </summary>
    public const string ItemCountMustNotExceedPageSizeCode = "atya.data.pagination.item_count_must_not_exceed_page_size";

    /// <summary>
    /// Creates the page number validation error.
    /// </summary>
    /// <returns>The validation error.</returns>
    public static Error PageNumberMustBePositive() =>
        Error.Create(PageNumberMustBePositiveCode, "Page number must be greater than zero.");

    /// <summary>
    /// Creates the page size validation error.
    /// </summary>
    /// <returns>The validation error.</returns>
    public static Error PageSizeMustBePositive() =>
        Error.Create(PageSizeMustBePositiveCode, "Page size must be greater than zero.");

    /// <summary>
    /// Creates the offset overflow validation error.
    /// </summary>
    /// <returns>The validation error.</returns>
    public static Error OffsetMustFitInt32() =>
        Error.Create(OffsetMustFitInt32Code, "The requested page offset must fit in a 32-bit signed integer.");

    /// <summary>
    /// Creates the total count validation error.
    /// </summary>
    /// <returns>The validation error.</returns>
    public static Error TotalCountMustNotBeNegative() =>
        Error.Create(TotalCountMustNotBeNegativeCode, "Total count must not be negative.");

    /// <summary>
    /// Creates the item count validation error.
    /// </summary>
    /// <returns>The validation error.</returns>
    public static Error ItemCountMustNotExceedPageSize() =>
        Error.Create(ItemCountMustNotExceedPageSizeCode, "Item count must not exceed page size.");
}
