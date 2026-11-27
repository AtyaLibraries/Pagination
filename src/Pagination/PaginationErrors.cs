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
    [Obsolete("Offsets beyond Int32 are valid and PageRequest no longer reports this error. Removed in 2.0.")]
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
    /// The error code reported when the items of a page contradict its position and the total.
    /// </summary>
    public const string ItemCountMustMatchTotalCode = "atya.data.pagination.item_count_must_match_total";

    /// <summary>
    /// The maximum page size validation error code.
    /// </summary>
    public const string MaxPageSizeMustBePositiveCode = "atya.data.pagination.max_page_size_must_be_positive";

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
    [Obsolete("Offsets beyond Int32 are valid and PageRequest no longer reports this error. Removed in 2.0.")]
    public static Error OffsetMustFitInt32() =>
        new("atya.data.pagination.offset_must_fit_int32", "The requested page offset must fit in a 32-bit signed integer.", ErrorKind.Validation);

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

    /// <summary>
    /// Creates the error reported when the items of a page contradict its position and the total.
    /// </summary>
    /// <returns>The validation error.</returns>
    public static Error ItemCountMustMatchTotal() =>
        new(ItemCountMustMatchTotalCode, "Item count must equal the number of items the total count places on this page.", ErrorKind.Validation);

    /// <summary>
    /// Creates the maximum page size validation error.
    /// </summary>
    /// <returns>The validation error.</returns>
    public static Error MaxPageSizeMustBePositive() =>
        new(MaxPageSizeMustBePositiveCode, "Maximum page size must be greater than zero.", ErrorKind.Validation);
}
