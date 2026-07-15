using Atya.Foundation.Primitives.Paging;

namespace Atya.Data.Pagination.UnitTests;

public sealed class PageRequestTests
{
    [Fact]
    public void Create_Defaults_ReturnsDefaultRequest()
    {
        var result = PageRequest.Create();

        result.IsSuccess.Should().BeTrue();
        result.Value!.PageNumber.Should().Be(PageRequest.DefaultPageNumber);
        result.Value.PageSize.Should().Be(PageRequest.DefaultPageSize);
        result.Value.Offset.Should().Be(0);
        result.Value.LongOffset.Should().Be(0);
    }

    [Fact]
    public void Create_ValidInputs_ReturnsRequest()
    {
        var result = PageRequest.Create(3, 25);

        result.IsSuccess.Should().BeTrue();
        result.Value!.PageNumber.Should().Be(3);
        result.Value.PageSize.Should().Be(25);
        result.Value.Offset.Should().Be(50);
        result.Value.LongOffset.Should().Be(50);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_PageNumberNotPositive_ReturnsValidationFailure(int pageNumber)
    {
        var result = PageRequest.Create(pageNumber, 20);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PaginationErrors.PageNumberMustBePositiveCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_PageSizeNotPositive_ReturnsValidationFailure(int pageSize)
    {
        var result = PageRequest.Create(1, pageSize);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PaginationErrors.PageSizeMustBePositiveCode);
    }

    [Fact]
    public void Create_OffsetExceedsInt32_ReturnsValidationFailure()
    {
        var result = PageRequest.Create(int.MaxValue, 2);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PaginationErrors.OffsetMustFitInt32Code);
    }

    [Fact]
    public void ToPrimitiveRequest_ReturnsEquivalentPrimitiveRequest()
    {
        var request = PageRequest.Create(4, 10).Value!;

        PagedRequest primitive = request.ToPrimitiveRequest();

        primitive.PageNumber.Should().Be(4);
        primitive.PageSize.Should().Be(10);
        primitive.Skip.Should().Be(30);
    }
}
