# Atya.Data.Pagination

*ORM-agnostic paging primitives and paged result shapes for .NET services.*

[![NuGet Version](https://img.shields.io/nuget/v/Atya.Data.Pagination?style=for-the-badge&logo=nuget&logoColor=white&label=NuGet&color=512BD4)](https://www.nuget.org/packages/Atya.Data.Pagination)
[![Downloads](https://img.shields.io/nuget/dt/Atya.Data.Pagination?style=for-the-badge&logo=nuget&logoColor=white&label=Downloads&color=512BD4)](https://www.nuget.org/packages/Atya.Data.Pagination)
![.NET 10.0](https://img.shields.io/badge/.NET_10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
[![License: MIT](https://img.shields.io/badge/License-MIT-512BD4?style=for-the-badge)](https://github.com/AtyaLibraries/Pagination/blob/development/LICENSE)

## Overview

`Atya.Data.Pagination` provides a small, provider-neutral paging model for APIs, workers, and application services. It validates page number and page size as recoverable input failures, represents one page of results with total-count metadata, and avoids any dependency on Entity Framework, LINQ providers, repositories, or query abstractions.

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

## Feature Tour

### PageRequest

`PageRequest.Create` validates one-based page number and positive page size values and returns `Result<PageRequest>` from `Atya.Foundation.Results`. Successful requests expose `PageNumber`, `PageSize`, `Offset`, and `LongOffset`.

### PagedResult

`PagedResult.Create` combines current-page items with a validated request and the total count. The returned `PagedResult<T>` exposes `TotalPages`, `HasPreviousPage`, and `HasNextPage` without depending on any query provider.

## Error Codes

| Code | When it is returned |
|---|---|
| `atya.data.pagination.page_number_must_be_positive` | `pageNumber` is zero or negative. |
| `atya.data.pagination.page_size_must_be_positive` | `pageSize` is zero or negative. |
| `atya.data.pagination.offset_must_fit_int32` | `(pageNumber - 1) * pageSize` exceeds `int.MaxValue`. |
| `atya.data.pagination.total_count_must_not_be_negative` | `totalCount` is negative. |
| `atya.data.pagination.item_count_must_not_exceed_page_size` | The supplied page item count is greater than `PageSize`. |

## Why These Dependencies

`Atya.Foundation.Guards` handles programmer errors such as null `items` and null validated requests. `Atya.Foundation.Results` supplies the fleet-canonical `Result<T>`, `Error`, and `ErrorKind` primitives used by Mediator, Http, and Idempotency packages.

## Links

- NuGet: https://www.nuget.org/packages/Atya.Data.Pagination
- Repository: https://github.com/AtyaLibraries/Pagination
- Samples: https://github.com/AtyaLibraries/Pagination/tree/development/samples
- License: https://github.com/AtyaLibraries/Pagination/blob/development/LICENSE
