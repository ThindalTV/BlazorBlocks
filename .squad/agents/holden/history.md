# Holden — History

## Core Context

**Project:** BlazorBlocks — a Blazor page builder distributed as a NuGet package.
**Stack:** .NET, Blazor (WebAssembly/Server), C#
**Repo:** https://github.com/ThindalTV/BlazorBlocks
**User:** Eric Johansson

**Architecture:** Page → Rows → Columns (column collections define responsive layout) → Blocks. A Block is a discrete content editor (Title, Code, Image, WYSIWYG, etc.).

**Key files:**
- `src/BlazorBlocks/BlazorBlocksEditor.razor` — main editor component
- `src/BlazorBlocks/BlazorBlocksModel.cs` — core data model
- `src/BlazorBlocks/BlockRegistrationService.cs` — block discovery/registration
- `src/BlazorBlocks/Blocks/` — individual block implementations
- `src/BlazorBlocks/Services/` — service layer
- `src/Tests/` — test project

## Learnings

<!-- Append new learnings below with date prefix -->
