# Decisions

> Authoritative decision ledger. Append-only. All agents read this before starting work.

## Scope & Architecture

- **2026-02-24** — Project is a NuGet-distributable Blazor page builder. Core model: Page → Rows → Columns → Blocks. Blocks are discrete content editors (Title, Code, Image, WYSIWYG, etc.).
- **2026-02-24** — Stack: .NET + Blazor. Distribution target: NuGet package (not a standalone app).
- **2026-02-24** — Responsive layout is a first-class concern. Column collections define the responsive structure.

## Process

- **2026-02-24** — Team universe: The Expanse. No role-play, no character voices — names are identifiers only.
- **2026-02-24** — GitHub issue tracking connected to ThindalTV/BlazorBlocks.
