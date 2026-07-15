using Atya.Foundation.Primitives.Errors;

namespace Atya.Data.Pagination.UnitTests;

public sealed class PaginationErrorsTests
{
    [Theory]
    [MemberData(nameof(ErrorFactories))]
    public void ErrorFactory_ReturnsStableCode(Func<Error> factory, string expectedCode)
    {
        var error = factory();

        error.Code.Should().Be(expectedCode);
        error.Message.Should().NotBeNullOrWhiteSpace();
    }

    public static TheoryData<Func<Error>, string> ErrorFactories() =>
        new()
        {
            { PaginationErrors.PageNumberMustBePositive, PaginationErrors.PageNumberMustBePositiveCode },
            { PaginationErrors.PageSizeMustBePositive, PaginationErrors.PageSizeMustBePositiveCode },
            { PaginationErrors.OffsetMustFitInt32, PaginationErrors.OffsetMustFitInt32Code },
            { PaginationErrors.TotalCountMustNotBeNegative, PaginationErrors.TotalCountMustNotBeNegativeCode },
            { PaginationErrors.ItemCountMustNotExceedPageSize, PaginationErrors.ItemCountMustNotExceedPageSizeCode },
        };
}
