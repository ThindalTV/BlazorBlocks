# Drummer — History

## Core Context

**Project:** BlazorBlocks — a Blazor page builder distributed as a NuGet package.
**Stack:** .NET, xUnit, bUnit (Blazor component testing), C#
**Repo:** https://github.com/ThindalTV/BlazorBlocks
**User:** Eric Johansson

**My domain:** All tests. Unit tests for models/services. bUnit component tests. Edge cases.

**Key files:**
- `src/Tests/` — test project
- `src/BlazorBlocks/BlazorBlocksModel.cs` — model under test
- `src/BlazorBlocks/BlockRegistrationService.cs` — service under test
- `src/BlazorBlocks/Blocks/` — block components under test

**What to test:** Empty pages, null blocks, deeply nested rows/columns, responsive breakpoint edge cases, block registration failures, serialization round-trips.

## Learnings

<!-- Append new learnings below with date prefix -->
