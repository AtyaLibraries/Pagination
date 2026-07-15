using BenchmarkDotNet.Attributes;

namespace Atya.Data.Pagination.Benchmarks;

/// <summary>
/// Benchmarks representative pagination hot paths.
/// </summary>
[MemoryDiagnoser]
public class PaginationBenchmarks
{
    private int[] _items = Array.Empty<int>();
    private PageRequest _request = PageRequest.Create().Value!;

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    [Params(10, 50, 100)]
    public int PageSize { get; set; }

    /// <summary>
    /// Sets up benchmark inputs.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        _request = PageRequest.Create(pageNumber: 2, PageSize).Value!;
        _items = Enumerable.Range(1, PageSize).ToArray();
    }

    /// <summary>
    /// Creates a validated page request.
    /// </summary>
    /// <returns>The validated request result.</returns>
    [Benchmark(Baseline = true)]
    public object CreatePageRequest() => PageRequest.Create(pageNumber: 2, PageSize);

    /// <summary>
    /// Creates a paged result from validated metadata.
    /// </summary>
    /// <returns>The paged result.</returns>
    [Benchmark]
    public object CreatePagedResult() => PagedResult.Create(_items, _request, totalCount: PageSize * 3);
}
