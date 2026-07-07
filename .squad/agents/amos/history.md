# Amos — History

## Core Context

**Project:** BlazorBlocks — a Blazor page builder distributed as a NuGet package.
**Stack:** .NET, C#, Dependency Injection, NuGet packaging
**Repo:** https://github.com/ThindalTV/BlazorBlocks
**User:** Eric Johansson

**My domain:** Data models, services, block registration, DI wiring, NuGet metadata.

**Key files:**
- `src/BlazorBlocks/BlazorBlocksModel.cs` — core page/row/column/block model (mine)
- `src/BlazorBlocks/BlockRegistrationService.cs` — how blocks register themselves
- `src/BlazorBlocks/Services/` — service interfaces and implementations
- `src/BlazorBlocks/Internals/` — infrastructure utilities
- `src/BlazorBlocks/BlazorBlocks.csproj` — NuGet package config

**Architecture:** Blocks register via `BlockRegistrationService`. The model is serializable (JSON). Package consumers wire up via `IServiceCollection` extension methods.

## Learnings

<!-- Append new learnings below with date prefix -->
