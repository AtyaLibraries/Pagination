using System.Text.Json;

namespace Atya.Data.Pagination.UnitTests;

public sealed class JsonTests
{
    [Fact]
    public void PageRequest_Serializes_WithCanonicalLongOffset_AndWithoutTheObsoleteIntOffset()
    {
        var request = PageRequest.Create(int.MaxValue, 2).Value!;

        using JsonDocument document = JsonDocument.Parse(JsonSerializer.Serialize(request));
        JsonElement root = document.RootElement;

        root.GetProperty("PageNumber").GetInt32().Should().Be(int.MaxValue);
        root.GetProperty("PageSize").GetInt32().Should().Be(2);
        root.GetProperty("LongOffset").GetInt64().Should().Be(((long)int.MaxValue - 1) * 2);
        root.TryGetProperty("Offset", out _).Should().BeFalse();
    }

    [Fact]
    public void PagedResult_Serializes_ExactLongTotals_AndSaturatedIntCompatibilityTotals()
    {
        var request = PageRequest.Create(1, 2).Value!;
        long total = (long)int.MaxValue * 3;

        var page = PagedResult.Create(new[] { "a", "b" }, request, total).Value!;
        using JsonDocument document = JsonDocument.Parse(JsonSerializer.Serialize(page));
        JsonElement root = document.RootElement;

        root.GetProperty("Items").GetArrayLength().Should().Be(2);
        root.GetProperty("LongTotalCount").GetInt64().Should().Be(total);
        root.GetProperty("TotalCount").GetInt32().Should().Be(int.MaxValue);
        root.GetProperty("LongTotalPages").GetInt64().Should().Be((total / 2) + (total % 2));
        root.GetProperty("TotalPages").GetInt32().Should().Be(int.MaxValue);
        root.GetProperty("HasNextPage").GetBoolean().Should().BeTrue();
        root.GetProperty("HasPreviousPage").GetBoolean().Should().BeFalse();
    }

    [Fact]
    public void PagedResult_SmallTotals_SerializeAsBefore()
    {
        var request = PageRequest.Create(2, 3).Value!;

        var page = PagedResult.Create(new[] { 4, 5, 6 }, request, 8).Value!;
        using JsonDocument document = JsonDocument.Parse(JsonSerializer.Serialize(page));
        JsonElement root = document.RootElement;

        root.GetProperty("PageNumber").GetInt32().Should().Be(2);
        root.GetProperty("PageSize").GetInt32().Should().Be(3);
        root.GetProperty("TotalCount").GetInt32().Should().Be(8);
        root.GetProperty("TotalPages").GetInt32().Should().Be(3);
    }
}
