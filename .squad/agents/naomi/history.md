# Naomi — History

## Core Context

**Project:** BlazorBlocks — a Blazor page builder distributed as a NuGet package.
**Stack:** .NET, Blazor (WebAssembly/Server), C#, scoped CSS
**Repo:** https://github.com/ThindalTV/BlazorBlocks
**User:** Eric Johansson

**My domain:** All `.razor` files, `wwwroot/`, `Customization/`, `Samples/` UI, and the editor shell.

**Key files:**
- `src/BlazorBlocks/BlazorBlocksEditor.razor` — main editor component (mine)
- `src/BlazorBlocks/Blocks/` — block components (shared with Amos)
- `src/BlazorBlocks/Customization/` — theming, extension points
- `src/BlazorBlocks/wwwroot/` — static assets, JS interop
- `src/Samples/` — sample Blazor app

**Architecture:** Rows → Columns → Blocks. Column collections define responsive breakpoints.

## Learnings

<!-- Append new learnings below with date prefix -->
