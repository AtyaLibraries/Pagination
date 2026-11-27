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
        result.Value.LongTotalCount.Should().Be(8);
        result.Value.TotalPages.Should().Be(3);
        result.Value.LongTotalPages.Should().Be(3);
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

    [Theory]
    [InlineData(-1)]
    [InlineData(long.MinValue)]
    public void Create_NegativeTotalCount_ReturnsValidationFailure(long total)
    {
        var request = PageRequest.Create(1, 10).Value!;

        var result = PagedResult.Create(Array.Empty<int>(), request, total);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PaginationErrors.TotalCountMustNotBeNegativeCode);
    }

    [Fact]
    public void Create_NegativeIntTotalCount_ReturnsValidationFailure()
    {
        var request = PageRequest.Create(1, 10).Value!;

        var result = PagedResult.Create(Array.Empty<int>(), request, -1);

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
    public void Create_SparseNonFinalPage_IsRejected()
    {
        var request = PageRequest.Create(1, 10).Value!;

        var result = PagedResult.Create(new[] { 1, 2, 3 }, request, 100);

        result.Error.Code.Should().Be(PaginationErrors.ItemCountMustMatchTotalCode);
    }

    [Fact]
    public void Create_ItemsBeyondTheTotal_AreRejected()
    {
        var request = PageRequest.Create(1, 10).Value!;

        var result = PagedResult.Create(new[] { 1, 2, 3 }, request, 2);

        result.Error.Code.Should().Be(PaginationErrors.ItemCountMustMatchTotalCode);
    }

    [Fact]
    public void Create_OutOfRangePageWithItems_IsRejected()
    {
        var request = PageRequest.Create(5, 10).Value!;

        var result = PagedResult.Create(new[] { 1 }, request, 12);

        result.Error.Code.Should().Be(PaginationErrors.ItemCountMustMatchTotalCode);
    }

    [Fact]
    public void Create_OutOfRangePageWithoutItems_IsAcceptedAndHasNoNextPage()
    {
        var request = PageRequest.Create(5, 10).Value!;

        var result = PagedResult.Create(Array.Empty<int>(), request, 12);

        result.IsSuccess.Should().BeTrue();
        result.Value!.HasNextPage.Should().BeFalse();
        result.Value.HasPreviousPage.Should().BeTrue();
        result.Value.TotalPages.Should().Be(2);
    }

    [Fact]
    public void Create_FinalPartialPage_IsAccepted()
    {
        var request = PageRequest.Create(3, 10).Value!;

        var result = PagedResult.Create(new[] { 1, 2 }, request, 22);

        result.IsSuccess.Should().BeTrue();
        result.Value!.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Create_TotalBeyondInt32_IsAcceptedWithExactLongMetadata()
    {
        var request = PageRequest.Create(1, 20).Value!;
        long total = (long)int.MaxValue * 4;
        var items = Enumerable.Range(0, 20).ToArray();

        var result = PagedResult.Create(items, request, total);

        result.IsSuccess.Should().BeTrue();
        result.Value!.LongTotalCount.Should().Be(total);
        result.Value.TotalCount.Should().Be(int.MaxValue);
        result.Value.LongTotalPages.Should().Be((total / 20) + (total % 20 == 0 ? 0 : 1));
        result.Value.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void Create_DeepPageBeyondInt32Offset_IsAccepted()
    {
        var request = PageRequest.Create(int.MaxValue, 2).Value!;
        long total = request.LongOffset + 1;

        var result = PagedResult.Create(new[] { 7 }, request, total);

        result.IsSuccess.Should().BeTrue();
        result.Value!.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void TotalPages_Property_UsesExactIntegerArithmetic()
    {
        var random = new Random(66);

        for (int i = 0; i < 2000; i++)
        {
            int pageSize = random.Next(1, 5000);
            long total = random.NextInt64(0, 10_000_000_000);
            var request = PageRequest.Create(int.MaxValue, pageSize).Value!;

            var page = PagedResult.Create(Array.Empty<int>(), request, total).Value!;

            long expected = (total + pageSize - 1) / pageSize;
            page.LongTotalPages.Should().Be(expected);
            page.TotalPages.Should().Be((int)Math.Min(expected, int.MaxValue));
        }
    }

    [Fact]
    public void Property_AcceptedPages_SatisfyTheInvariants_AndRejectedOnesDoNot()
    {
        var random = new Random(67);

        for (int i = 0; i < 3000; i++)
        {
            int pageSize = random.Next(1, 30);
            int pageNumber = random.Next(1, 40);
            long total = random.Next(0, 800);
            int count = random.Next(0, pageSize + 1);
            var request = PageRequest.Create(pageNumber, pageSize).Value!;
            long expected = Math.Min(Math.Max(total - request.LongOffset, 0), pageSize);

            var result = PagedResult.Create(new int[count], request, total);

            result.IsSuccess.Should().Be(count == expected);
        }
    }

    [Fact]
    public void Items_AreASnapshot_LaterChangesToTheSourceDoNotAffectTheResult()
    {
        var request = PageRequest.Create(1, 5).Value!;
        var source = new List<string> { "a", "b" };

        var page = PagedResult.Create(source, request, 2).Value!;
        source[0] = "changed";
        source.Add("extra");

        page.Items.Should().Equal("a", "b");
    }

    [Fact]
    public void Items_CannotBeMutatedThroughACast()
    {
        var request = PageRequest.Create(1, 5).Value!;
        var source = new[] { "a", "b" };

        var page = PagedResult.Create(source, request, 2).Value!;

        page.Items.Should().NotBeAssignableTo<string[]>();
        var list = (IList<string>)page.Items;
        var add = () => list.Add("c");
        var set = () => list[0] = "z";
        add.Should().Throw<NotSupportedException>();
        set.Should().Throw<NotSupportedException>();
        source[0] = "mutated";
        page.Items.Should().Equal("a", "b");
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
