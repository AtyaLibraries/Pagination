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

`PageRequest.Create` validates one-based page number and positive page size values and returns `Result<PageRequest>` from `Atya.Foundation.Results`. Successful requests expose `PageNumber`, `PageSize`, and `LongOffset`, the canonical 64-bit offset. Every offset the Int32 inputs can produce is accepted. The 32-bit `Offset` property is obsolete and is removed in 2.0; it throws `OverflowException` when the offset does not fit.

To cap page sizes, pass an explicit policy (there is no global setting): `PageRequest.Create(pageNumber, pageSize, PageSizePolicy.Create(100).Value!)` clamps oversized page sizes to 100.

### PagedResult

`PagedResult.Create` combines current-page items with a validated request and the total count. The returned `PagedResult<T>` exposes `LongTotalCount`, `LongTotalPages`, `HasPreviousPage`, and `HasNextPage` without depending on any query provider. `TotalCount` and `TotalPages` remain as 32-bit values that saturate at `int.MaxValue`. Totals may be 64-bit (`Create` has a `long totalCount` overload). `Items` is an immutable snapshot of the supplied collection.

A page whose item count contradicts its position and the total is rejected: a non-final page must be full, the final page must hold exactly the remaining items, and a page past the end must be empty.

## Migrating to 1.1

- Use `LongOffset`; `Offset` is obsolete and will be removed in 2.0.
- `PageRequest.Create` no longer fails with `offset_must_fit_int32`: such requests are now valid.
- `PagedResult.Create` now rejects contradictory pages (`item_count_must_match_total`) that 1.0 accepted, and copies the items instead of keeping the caller's collection.
- `TotalCount` and `TotalPages` saturate at `int.MaxValue`; read `LongTotalCount` and `LongTotalPages` for exact values. The JSON shape gains `LongTotalCount`, `LongTotalPages` and `LongOffset` and drops the obsolete `Offset`.
- Cursor (keyset) pagination is intentionally not part of this release.
- Last behavior-compatible version: 1.0.0.

## Error Codes

| Code | When it is returned |
|---|---|
| `atya.data.pagination.page_number_must_be_positive` | `pageNumber` is zero or negative. |
| `atya.data.pagination.page_size_must_be_positive` | `pageSize` is zero or negative. |
| `atya.data.pagination.offset_must_fit_int32` | Obsolete; no longer returned. Offsets beyond `int.MaxValue` are valid. |
| `atya.data.pagination.total_count_must_not_be_negative` | `totalCount` is negative. |
| `atya.data.pagination.item_count_must_not_exceed_page_size` | The supplied page item count is greater than `PageSize`. |
| `atya.data.pagination.item_count_must_match_total` | The item count contradicts the page position and the total (sparse non-final page, items past the end). |
| `atya.data.pagination.max_page_size_must_be_positive` | `PageSizePolicy.Create` received a zero or negative maximum. |

## Why These Dependencies

`Atya.Foundation.Guards` handles programmer errors such as null `items` and null validated requests. `Atya.Foundation.Results` supplies the fleet-canonical `Result<T>`, `Error`, and `ErrorKind` primitives used by Mediator, Http, and Idempotency packages.

## Links

- NuGet: https://www.nuget.org/packages/Atya.Data.Pagination
- Repository: https://github.com/AtyaLibraries/Pagination
- Samples: https://github.com/AtyaLibraries/Pagination/tree/development/samples
- License: https://github.com/AtyaLibraries/Pagination/blob/development/LICENSE
