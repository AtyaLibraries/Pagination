namespace Atya.Data.Pagination.Samples.ConsoleApp;

/// <summary>
/// Runs the sample console application.
/// </summary>
public static class Program
{
    private static readonly string[] s_items = ["delta", "echo", "foxtrot"];

    /// <summary>
    /// Writes a minimal paging sample.
    /// </summary>
    public static void Main()
    {
        var request = PageRequest.Create(pageNumber: 2, pageSize: 3).Value!;
        var page = PagedResult
            .Create(s_items, request, totalCount: 8)
            .Value!;

        Console.WriteLine($"Page {page.PageNumber} of {page.TotalPages}");
        Console.WriteLine($"Offset: {request.Offset}");
        Console.WriteLine($"Has next page: {page.HasNextPage}");
    }
}
