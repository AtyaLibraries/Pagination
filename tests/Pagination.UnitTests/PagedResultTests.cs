namespace Atya.Data.Pagination.UnitTests;

public sealed class PagedResultTests
{
    [Fact]
    public void Create_ValidInputs_ReturnsPageMetadata()
    {
        var request = PageRequest.Create(2, 3).Value!;
        var items = new[] { "four", "five", "six" };

        var result = PagedResult.Create(items, request, 8);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().Equal(items);
        result.Value.PageNumber.Should().Be(2);
        result.Value.PageSize.Should().Be(3);
        result.Value.TotalCount.Should().Be(8);
        result.Value.TotalPages.Should().Be(3);
        result.Value.HasPreviousPage.Should().BeTrue();
        result.Value.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void Create_FirstPageWithoutNextPage_ReturnsExpectedFlags()
    {
        var request = PageRequest.Create(1, 10).Value!;
        var items = new[] { 1, 2 };

        var result = PagedResult.Create(items, request, 2);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalPages.Should().Be(1);
        result.Value.HasPreviousPage.Should().BeFalse();
        result.Value.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Create_ZeroTotalCount_ReturnsZeroPages()
    {
        var request = PageRequest.Create(1, 10).Value!;

        var result = PagedResult.Create(Array.Empty<int>(), request, 0);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalPages.Should().Be(0);
        result.Value.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Create_NullItems_Throws()
    {
        var request = PageRequest.Create(1, 10).Value!;

        var action = () => PagedResult.Create<int>(null!, request, 0);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_NullRequest_Throws()
    {
        var action = () => PagedResult.Create(Array.Empty<int>(), null!, 0);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_NegativeTotalCount_ReturnsValidationFailure()
    {
        var request = PageRequest.Create(1, 10).Value!;

        var result = PagedResult.Create(Array.Empty<int>(), request, -1);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PaginationErrors.TotalCountMustNotBeNegativeCode);
    }

    [Fact]
    public void Create_ItemCountGreaterThanPageSize_ReturnsValidationFailure()
    {
        var request = PageRequest.Create(1, 2).Value!;
        var items = new[] { 1, 2, 3 };

        var result = PagedResult.Create(items, request, 3);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PaginationErrors.ItemCountMustNotExceedPageSizeCode);
    }

    [Fact]
    public void Empty_ValidRequest_ReturnsEmptyPage()
    {
        var request = PageRequest.Create(3, 5).Value!;

        var result = PagedResult.Empty<int>(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.PageNumber.Should().Be(3);
        result.Value.PageSize.Should().Be(5);
        result.Value.TotalCount.Should().Be(0);
    }
}
