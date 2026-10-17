# BlazorBlocks: Status & Roadmap to 1.0

*Updated 2026-10-17 · `main` · replaces the 2026-10-04 inventory. Status comes from the code, the test run and GitHub; the editor was not re-run in a browser for this update.*

## TL;DR

- **Where it stands:** a working block-based page editor for Blazor with drag & drop, six group layouts, JSON round trip and HTML output. Block rendering has moved out of the models into Razor renderers, the registration API is rebuilt, Bootstrap is gone, and the library now has two extension packages (Markdown, WYSIWYG). It has still never been released and nothing is on NuGet.
- **Since the last inventory:** all 23 issues are closed. Done: the init-order, thread-safety and cache bugs (#19, #22, #23), the log-string bug (#20), Bootstrap removal (#14), Markdown, WYSIWYG, SVG and embedded-image blocks (#13, #16, #17, #18), group layout previews and styled dialogs (#6, #15), drag & drop (#12), delete confirmation, and a GitHub Pages site.
- **Carousel removed (2026-10-17):** it never worked and is too big to fix now. A real one (CSS scroll-snap) is a post-1.0 item.
- **Still blocking 1.0:**
  1. **Unknown block types still crash loading.** The resolver is set to `FailSerialization`.
  2. **Image height is still not saved.** `Height` is a public field.
  3. **No CI.** Only the Pages workflow exists. Nothing builds or tests on a PR.
  4. **Not packaged.** No NuGet metadata, no `Directory.Build.props`, no release workflow, README still says "clone the repo".
  5. **Editor accessibility is only partly done** (see §3).

---

## 1. Snapshot

| | |
|---|---|
| Stack | .NET 10 (pinned in `src/global.json`), Razor class libraries, Blazor WebAssembly sample, xUnit |
| Projects | `BlazorBlocks` (core), `BlazorBlocks.Blocks.Markdown`, `BlazorBlocks.Blocks.Wysiwyg`, `BlazorBlocks.Sample.WASM`, and three test projects, all in `src/BlazorBlocks.slnx` |
| Build | ✅ Clean |
| Tests | ✅ 81 pass: core 66 (render golden tests, editors, group previews, load, `CssHelper`), Markdown 9, WYSIWYG 6 |
| CI | ⚠️ Only `pages.yml` (deploys the site). No build/test workflow. |
| Releases | None: no tags, no NuGet package |
| Issues | 0 open, 23 closed |
| Branches | Merged feature branches (`Editors`, `WysiwygBlock`, `block-renderers`, `fix-issues-19-20-22-23`, `remove-bootstrap-remnants`, `fix/issue-21-…`) can be deleted. `docs/editor-ux-suggestions` is an unmerged UX review. |

**Built-in blocks:** Title, Quote, Image (URL or embedded), SVG (sanitised), Raw Text. **Extension packages:** Markdown (Markdig), WYSIWYG (TinyMCE, sanitised).

## 2. What changed since 2026-10-04

| Old blocker | State |
|---|---|
| B1 Unencoded output | **Largely fixed.** Rendering is done by Razor renderers, which encode by default; SVG and WYSIWYG output are sanitised. Hostile-input tests for every block are still missing. |
| B3 Static registry / init order (#19, #22, #23) | **Fixed.** `LoadTests` passes; the registry is locked and snapshotted. The registry is still static, not DI-scoped. |
| B6 Dialog dead end | **Believed fixed** (native `@onclose` sync, shared dialog component). Needs a regression test. |
| B8 Global CSS in output | **Not re-checked.** |
| B9 Responsive output | **Partly done.** `md` breakpoint classes exist in `blazorblocks-layout.css`. Whether the default layouts stack on phones and images get `max-width` is unverified. |
| #14 Bootstrap | **Done.** |
| Rendering in models | **Done.** Models are plain data; `BlockRenderer`/`BlockHtmlRenderer` do the output. |
| Delete confirmation | **Done.** |
| Squad leftovers, `copilot-instructions.md` | **Gone.** |
| `global.json` | **Done** (`src/global.json`, 10.0.100, roll forward to latest feature band). |
| Carousel | **Removed.** |

## 3. Still open

### Blockers for 1.0

| # | Problem | Where |
|---|---|---|
| B2 | **Unknown block type crashes loading.** `UnknownDerivedTypeHandling = FailSerialization`, so a page that references a removed or renamed block can't be opened. Needs a placeholder block that keeps the original JSON and writes it back on save. | [JsonSerializerTypeResolver.cs](src/BlazorBlocks/Internals/Models/JsonSerializerTypeResolver.cs) |
| B4 | **Image height is never saved.** `Height` is a public field, which System.Text.Json skips. | [ImageBlockModel.cs:39](src/BlazorBlocks/Blocks/ImageBlock/ImageBlockModel.cs#L39) |
| B5 | **Change notifications are incomplete.** `OnModelUpdated` now fires from a few places (`BlockEditor`, `EditorColumn`, the editor). Audit that *every* change fires it: add group, all drag moves, every field of every block. | [BlazorBlocksEditor.razor](src/BlazorBlocks/BlazorBlocksEditor.razor), [EditorColumn.razor](src/BlazorBlocks/Internals/Editors/EditorColumn.razor) |
| B7 | **Keyboard and screen-reader support is unfinished.** Some controls are `<button>`s now, but dragging is still the only way to reorder, and `aria-expanded` on the collapse control and per-instance input IDs/labels haven't been checked. | [DraggableContainer.razor](src/BlazorBlocks/Internals/Containers/DraggableContainer.razor), [CollapseButton.razor](src/BlazorBlocks/Internals/Components/CollapseButton.razor) |
| B10 | **Not packaged or documented.** No PackageId, version, description, license or readme metadata on any library project; README getting-started still says to clone and add a project reference; custom-block docs say "Full documentation is still to come". | [BlazorBlocks.csproj](src/BlazorBlocks/BlazorBlocks.csproj), [README.md](README.md) |
| B11 | **No CI.** Nothing builds or tests on a PR or push to `main`. | `.github/workflows/` |

### Important: fix before 1.0 or soon after

- **Debug logging ships in the library:** `Load()` still has `Console.WriteLine` calls ([BlazorBlocksModel.cs:90](src/BlazorBlocks/BlazorBlocksModel.cs#L90) and below).
- **JSON format isn't versioned or documented.** The discriminator is the CLR class name, there's no root `version`, and the format isn't written down. Renaming a class breaks stored content. Do this before the first package ships.
- **Public API surface:** review what is `public` (the document model lives in `Internals.Models`; `CssHelper`, `ObjectDroppedResult`, `DragObjectType` and others). There are still several `AddBlazorBlocks` overloads with inconsistent boolean defaults, and `BlockRegistration` takes untyped `Type`s.
- **Responsive check:** verify the default group layouts stack below `md` and that images can't overflow their column. Scope the output CSS so adding it to a Bootstrap/Tailwind page restyles nothing outside `.bb-` markup.
- **CSS cleanup:** duplicated rules between global and scoped CSS, dead rules, hard-coded drag-handle fill.
- **Leftovers:** "row" wording in comments and XML docs (the settled term is *group*); `ExcludeFromSingleFile` items in the core csproj; template leftovers in the sample (not re-checked).
- **Tests:** no bUnit/component tests; no round-trip test per block; no hostile-input encoding tests; no tests for Esc-then-reopen of dialogs.
- **Dependencies:** core still references `Microsoft.AspNetCore.Components.Web` 10.0.0.

## 4. What "1.0" means

1. **Easy to adopt.** `dotnet add package BlazorBlocks`, one registration call, `<BlazorBlocksEditor>` on a page, save and load JSON, render HTML. Shown in the README and works in WebAssembly and in a Blazor Web App with interactive server rendering.
2. **Safe by default.** Every built-in block encodes its output, except Raw Text, which deliberately emits raw HTML and is documented as trusted input only.
3. **Content survives.** Every block round-trips; unknown blocks load as placeholders and save back unchanged; the format carries a version and is documented.
4. **No dead ends in the editor.** Works with mouse and keyboard and tells the host about every change.
5. **Responsive by default**, and the CSS stays inside BlazorBlocks' own markup.
6. **CI is green and real**, with unit tests plus bUnit tests for the main editor flows.
7. **API reviewed and documented** (XML docs), SemVer from then on.

## 5. Roadmap

### Phase 1: Safety and format

*The remaining breaking changes. Do them before the first package exists.*

- Unknown-block placeholder that preserves and re-emits its JSON (B2), with editor text "Unknown block: x" and no output.
- Move the image `Height` to a property (B4).
- JSON format: explicit type IDs instead of CLR class names, a root `version` field, document it.
- Remove the `Console.WriteLine` calls.
- Tests: round trip per block, unknown block, hostile-input encoding per block.
- **Exit:** a page with a removed block type opens and saves unchanged; the format is documented.

### Phase 2: CI and packaging

- Build/test workflow on every PR and push to `main`; keep `dotnet pack` output as an artifact.
- `Directory.Build.props` (shared settings, warnings as errors), NuGet metadata on the three library projects (version `1.0.0-preview.1`, MIT, readme, repository URL, symbols), XML docs.
- Bump packages to current patches. Delete merged branches.
- **Checkpoint: publish `1.0.0-preview.1`.**

### Phase 3: Editor correctness and accessibility

- Audit `OnModelUpdated` so every change fires it (B5); add tests.
- Keyboard alternative to dragging: move up, move down, move to another column.
- Real `<button>`s with labels everywhere, `aria-expanded` on collapse, per-instance input IDs, labels on Quote and Raw Text inputs.
- bUnit tests: render a model, add group/block, delete, reopen a dialog after Esc, change notifications.
- Fix remaining Blazor anti-patterns (parameter writes, `async void`) if any are still present.

### Phase 4: Look, layout and CSS

- Verify and fix responsive output at 375, 768 and 1440 px (stacking groups, images capped at column width).
- Scope the output stylesheet; remove any global resets/element rules; de-duplicate and delete dead CSS.
- Pick up the remaining items from the UX review on `docs/editor-ux-suggestions` that matter, or drop the branch.
- **Exit:** adding `blazorblocks.css` to a Bootstrap or Tailwind page changes nothing outside BlazorBlocks content.

### Phase 5: API review, docs, release

- Public API pass: shrink `public` to what is intended, collapse the `AddBlazorBlocks` overloads, move the document model out of `Internals`.
- README: install, registration, which CSS to link, interactive render mode requirement, save/load, rendering output safely, writing a custom block, theming with `--bb-*`.
- Samples: tidy the WASM sample and add a Blazor Web App (interactive server) sample.
- Release workflow: a `v*` tag packs, pushes to NuGet and creates a GitHub release; start a CHANGELOG.
- **Release `1.0.0-rc.1`, then `1.0.0`.**

### Milestones

| Milestone | Phases |
|---|---|
| `1.0.0-preview.1` on NuGet | 1–2 |
| Feature-complete | 3–4 |
| `1.0.0` | 5 |

## 6. After 1.0

A real carousel (CSS scroll-snap, back as an extension package or a core block) · undo/redo · code block · more per-group and per-column layout control · read-only/preview mode · a Blazor component renderer as an alternative to the HTML string · Playwright end-to-end tests for drag & drop · dark-mode tokens · translatable editor text · DI-scoped registry instead of the static one.

## 7. Decisions

Settled:

- **Group**, not "row". Rows were the first version's name; a group can span several visual rows when layouts stack.
- **Rendering lives in separate renderers**, not on the models. Done.
- **Markdown ships as an extension package.** Done.
- **Carousel dropped** (2026-10-17).
- **net10.0 only.** .NET 8 and 9 reach end of support on 10 November 2026; .NET 10 is supported until November 2028.

Open:

1. **Registry:** keep it static for 1.0 (now thread-safe), or move to a DI-scoped registry first? Moving it later is a breaking change.
