using Atya.Foundation.Results;

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
        new(PageNumberMustBePositiveCode, "Page number must be greater than zero.", ErrorKind.Validation);

    /// <summary>
    /// Creates the page size validation error.
    /// </summary>
    /// <returns>The validation error.</returns>
    public static Error PageSizeMustBePositive() =>
        new(PageSizeMustBePositiveCode, "Page size must be greater than zero.", ErrorKind.Validation);

    /// <summary>
    /// Creates the offset overflow validation error.
    /// </summary>
    /// <returns>The validation error.</returns>
    public static Error OffsetMustFitInt32() =>
        new(OffsetMustFitInt32Code, "The requested page offset must fit in a 32-bit signed integer.", ErrorKind.Validation);

    /// <summary>
    /// Creates the total count validation error.
    /// </summary>
    /// <returns>The validation error.</returns>
    public static Error TotalCountMustNotBeNegative() =>
        new(TotalCountMustNotBeNegativeCode, "Total count must not be negative.", ErrorKind.Validation);

    /// <summary>
    /// Creates the item count validation error.
    /// </summary>
    /// <returns>The validation error.</returns>
    public static Error ItemCountMustNotExceedPageSize() =>
        new(ItemCountMustNotExceedPageSizeCode, "Item count must not exceed page size.", ErrorKind.Validation);
}
