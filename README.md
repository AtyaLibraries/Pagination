<h1 align="center">Atya.Data.Pagination</h1>

<p align="center"><i>ORM-agnostic paging primitives and paged result shapes for .NET services.</i></p>

<p align="center">
  <a href="https://www.nuget.org/packages/Atya.Data.Pagination"><img src="https://img.shields.io/nuget/v/Atya.Data.Pagination?style=for-the-badge&logo=nuget&logoColor=white&label=NuGet&color=512BD4" alt="NuGet Version"></a>
  <a href="https://www.nuget.org/packages/Atya.Data.Pagination"><img src="https://img.shields.io/nuget/dt/Atya.Data.Pagination?style=for-the-badge&logo=nuget&logoColor=white&label=Downloads&color=512BD4" alt="NuGet Downloads"></a>
  <img src="https://img.shields.io/badge/.NET_10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="Target Framework">
  <a href="LICENSE"><img src="https://img.shields.io/github/license/AtyaLibraries/Pagination?style=for-the-badge&color=512BD4" alt="License"></a>
  <a href="https://github.com/AtyaLibraries/Pagination/actions"><img src="https://img.shields.io/github/actions/workflow/status/AtyaLibraries/Pagination/ci.yml?branch=development&style=for-the-badge&logo=githubactions&logoColor=white&label=Build" alt="Build"></a>
</p>

## Overview

`Atya.Data.Pagination` provides a small, provider-neutral paging model for APIs, workers, and application services. It validates page number and page size as recoverable input failures, represents one page of results with total-count metadata, and avoids any dependency on Entity Framework, LINQ providers, repositories, or query abstractions.

## Features

- `PageRequest` models one-based page number and positive page size values.
- `PagedResult<T>` carries page items, total count, total pages, and previous/next flags.
- Invalid page input returns `Result<T>` from `Atya.Foundation.Primitives` instead of throwing for expected caller mistakes.
- `PageRequest.ToPrimitiveRequest()` bridges to the existing Foundation.Primitives paging shape.

## Installation

```bash
dotnet add package Atya.Data.Pagination
```

```powershell
Install-Package Atya.Data.Pagination
```

```xml
<PackageReference Include="Atya.Data.Pagination" Version="<latest-stable>" />
```

## Quick Start

```csharp
using Atya.Data.Pagination;

var request = PageRequest.Create(pageNumber: 2, pageSize: 3);

if (request.IsFailure)
{
    Console.WriteLine(request.Error.Code);
    return;
}

var page = PagedResult.Create(
    new[] { "delta", "echo", "foxtrot" },
    request.Value!,
    totalCount: 8);

Console.WriteLine($"Page {page.Value!.PageNumber} of {page.Value.TotalPages}");
Console.WriteLine($"Has next page: {page.Value.HasNextPage}");
```

## Error Codes

| Code | When it is returned |
|---|---|
| `atya.data.pagination.page_number_must_be_positive` | `pageNumber` is zero or negative. |
| `atya.data.pagination.page_size_must_be_positive` | `pageSize` is zero or negative. |
| `atya.data.pagination.offset_must_fit_int32` | `(pageNumber - 1) * pageSize` exceeds `int.MaxValue`. |
| `atya.data.pagination.total_count_must_not_be_negative` | `totalCount` is negative. |
| `atya.data.pagination.item_count_must_not_exceed_page_size` | The supplied page item count is greater than `PageSize`. |

## Why These Dependencies

`Atya.Foundation.Guards` handles programmer errors such as null `items` and null validated requests. `Atya.Foundation.Primitives` supplies the fleet's primitive `Result<T>`, `Error`, and existing primitive paging bridge.

## Compatibility

Targets `net10.0`.

## Benchmarks

Performance benchmarks live in `benchmarks/Pagination.Benchmarks`.

```bash
dotnet run -c Release --project benchmarks/Pagination.Benchmarks/Pagination.Benchmarks.csproj -- --list flat
```

## Links

- NuGet: https://www.nuget.org/packages/Atya.Data.Pagination
- Repository: https://github.com/AtyaLibraries/Pagination
- Samples: https://github.com/AtyaLibraries/Pagination/tree/development/samples
- License: https://github.com/AtyaLibraries/Pagination/blob/development/LICENSE

## License

Released under the MIT license. See [LICENSE](LICENSE) for details.
