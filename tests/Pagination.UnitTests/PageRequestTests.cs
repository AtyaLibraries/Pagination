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
        result.Value.LongOffset.Should().Be(0);
    }

    [Fact]
    public void Create_ValidInputs_ReturnsRequest()
    {
        var result = PageRequest.Create(3, 25);

        result.IsSuccess.Should().BeTrue();
        result.Value!.PageNumber.Should().Be(3);
        result.Value.PageSize.Should().Be(25);
        result.Value.LongOffset.Should().Be(50);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Create_PageNumberNotPositive_ReturnsValidationFailure(int pageNumber)
    {
        var result = PageRequest.Create(pageNumber, 20);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PaginationErrors.PageNumberMustBePositiveCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Create_PageSizeNotPositive_ReturnsValidationFailure(int pageSize)
    {
        var result = PageRequest.Create(1, pageSize);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PaginationErrors.PageSizeMustBePositiveCode);
    }

    [Fact]
    public void Create_OffsetBeyondInt32_IsAcceptedThroughLongOffset()
    {
        var result = PageRequest.Create(int.MaxValue, 2);

        result.IsSuccess.Should().BeTrue();
        result.Value!.LongOffset.Should().Be(((long)int.MaxValue - 1) * 2);
        result.Value.LongOffset.Should().BeGreaterThan(int.MaxValue);
    }

    [Fact]
    public void Create_LargestInputs_DoNotOverflowLongOffset()
    {
        var result = PageRequest.Create(int.MaxValue, int.MaxValue);

        result.IsSuccess.Should().BeTrue();
        result.Value!.LongOffset.Should().Be(((long)int.MaxValue - 1) * int.MaxValue);
    }

    [Fact]
    public void LongOffset_Property_AlwaysEqualsExactArithmetic()
    {
        var random = new Random(65);

        for (int i = 0; i < 2000; i++)
        {
            int pageNumber = random.Next(1, int.MaxValue);
            int pageSize = random.Next(1, int.MaxValue);

            var request = PageRequest.Create(pageNumber, pageSize).Value!;

            request.LongOffset.Should().Be(((long)pageNumber - 1) * pageSize);
            request.LongOffset.Should().BeGreaterThanOrEqualTo(0);
        }
    }

#pragma warning disable CS0618 // The obsolete Int32 Offset is exercised deliberately.
    [Fact]
    public void Offset_WhenItFitsInt32_EqualsLongOffset()
    {
        var request = PageRequest.Create(3, 25).Value!;

        request.Offset.Should().Be(50);
    }

    [Fact]
    public void Offset_WhenItDoesNotFitInt32_ThrowsOverflow()
    {
        var request = PageRequest.Create(int.MaxValue, 2).Value!;

        Func<int> action = () => request.Offset;

        action.Should().Throw<OverflowException>();
    }

    [Fact]
    public void Offset_IsMarkedObsolete()
    {
        var attribute = typeof(PageRequest).GetProperty(nameof(PageRequest.Offset))!
            .GetCustomAttributes(typeof(ObsoleteAttribute), inherit: false);

        attribute.Should().ContainSingle();
    }

    [Fact]
    public void ObsoleteOffsetError_StillReturnsStableCode()
    {
        PaginationErrors.OffsetMustFitInt32().Code.Should().Be(PaginationErrors.OffsetMustFitInt32Code);
    }
#pragma warning restore CS0618

    [Theory]
    [InlineData(1000, 100, 100)]
    [InlineData(100, 100, 100)]
    [InlineData(10, 100, 10)]
    [InlineData(int.MaxValue, 50, 50)]
    public void Create_WithPolicy_ClampsOversizedPageSize(int requested, int max, int expected)
    {
        var policy = PageSizePolicy.Create(max).Value!;

        var result = PageRequest.Create(2, requested, policy);

        result.IsSuccess.Should().BeTrue();
        result.Value!.PageSize.Should().Be(expected);
        result.Value.LongOffset.Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Create_WithPolicy_StillRejectsNonPositivePageSize(int requested)
    {
        var policy = PageSizePolicy.Create(10).Value!;

        var result = PageRequest.Create(1, requested, policy);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PaginationErrors.PageSizeMustBePositiveCode);
    }

    [Fact]
    public void Create_WithPolicy_StillRejectsNonPositivePageNumber()
    {
        var policy = PageSizePolicy.Create(10).Value!;

        var result = PageRequest.Create(0, 5, policy);

        result.Error.Code.Should().Be(PaginationErrors.PageNumberMustBePositiveCode);
    }

    [Fact]
    public void Create_WithNullPolicy_Throws()
    {
        var action = () => PageRequest.Create(1, 5, null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PageSizePolicy_NonPositiveMax_ReturnsValidationFailure(int max)
    {
        var result = PageSizePolicy.Create(max);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PaginationErrors.MaxPageSizeMustBePositiveCode);
    }

    [Fact]
    public void PageSizePolicy_IsNotGlobal_TwoPoliciesCoexist()
    {
        var small = PageSizePolicy.Create(5).Value!;
        var large = PageSizePolicy.Create(500).Value!;

        PageRequest.Create(1, 100, small).Value!.PageSize.Should().Be(5);
        PageRequest.Create(1, 100, large).Value!.PageSize.Should().Be(100);
        PageRequest.Create(1, 100).Value!.PageSize.Should().Be(100);
    }
}
