# BlazorBlocks: Status Inventory & Roadmap to 1.0

*Inventory date: 2026-10-04 · `main` @ `6862bfc` · no code was changed during this review*

## TL;DR

- **Where it stands:** a working prototype of a block-based page editor for Blazor (~2,600 lines). It loads content, adds and deletes groups and blocks, supports drag & drop, and outputs HTML and JSON. It has never been released and nothing is on NuGet.
- **Biggest problems:**
  1. **Unsafe output.** Block content goes into the HTML without encoding, so anything typed into a block can inject markup or script.
  2. **Loading can crash.** Saved content that references an unregistered block type takes down the whole app. Loading before `AddBlazorBlocks()` has run also throws (#19), which is why the only model test fails.
  3. **Silent data loss.** Image height is never saved. Several edits never notify the host: adding a group, every drag-move, and Quote and Carousel edits.
  4. **Editor dead end.** Closing the "Add group" dialog with Esc means it can't be reopened until the page is reloaded.
  5. **No real CI.** The green checks on `main` came from placeholder Squad workflows that only `echo`, while the test suite is red. Those workflows are now deleted locally, but nothing has replaced them yet.
  6. **Not shippable yet.** There's no NuGet metadata, the README's getting-started section is a placeholder, and the sample still contains template leftovers.
- **Size of the job:** roughly **20–30 focused dev-days** to a 1.0 you'd be comfortable freezing, or **10–15 days** to a safe preview on NuGet that doesn't crash.
- **Good timing:** nothing has been published yet, so breaking changes (registration API, JSON format, namespaces) cost nothing **now**. After 1.0, each one needs a major version bump, so do them first.

---

## 1. Snapshot

| | |
|---|---|
| History | 82 commits, Jan 2023 → Jul 2026. Last commit 2026-07-07 (~3 months ago) |
| Stack | .NET 10 (LTS), Razor class library, Blazor WebAssembly sample, xUnit |
| Projects | `BlazorBlocks` (library), `BlazorBlocks.Sample.WASM`, `BlazorBlocks.Tests.Model`, all in `src/BlazorBlocks.slnx` |
| Size | Library ≈ 2,600 lines (57 `.cs`/`.razor`/`.css` files, 16 components) · sample ≈ 460 · tests ≈ 85 |
| Build | ✅ Clean full rebuild: 0 warnings, 0 errors |
| Tests | ❌ 14 pass, 1 fails. All 14 passing tests cover `CssHelper` (a one-line regex). The only model test fails (#19). No component tests. |
| CI | ⚠️ None. The placeholder Squad workflows (which never built or tested anything) are deleted locally but not pushed yet. |
| Releases | None: no tags, no NuGet package, no package metadata |
| Issues | 12 open, 11 closed. PR #24 merged. No milestones. |
| Dependencies | Patch-level behind (10.0.0 → 10.0.12). No known vulnerabilities. |
| Branches | `docs/editor-ux-suggestions`: an unmerged UX review in 4 commits. `fix/issue-21-css-class-injection`: already merged, safe to delete. |

**Activity timeline**

- 2023-01 → 2024-03: initial editor, block registration, serialization, real-time rendering.
- 2025-05 → 2025-06: drag & drop, groups.
- 2025-10 → 2026-01: .NET 10 upgrade, test project, `--bb-*` CSS token system, layout/memory/a11y fixes, dialog rework.
- 2026-02: Squad AI-team setup, CSS class injection fix (#21), four bug issues filed (#19, #20, #22, #23).
- 2026-07: Squad config removed (its workflows were not), `DraggableContainer` refactor, image ARIA attributes.

## 2. What works

- Loads the demo JSON and shows groups → columns → blocks in the editor, with a live HTML preview.
- You can add groups (6 layouts) and blocks (5 built in, plus the sample's custom block), and delete and collapse them.
- Drag & drop works for groups and for blocks, including moving blocks between columns. Issue #12 is mostly implemented in code, but it wasn't exercised with a real mouse in this review.
- Title, Raw Text and Image edits update the preview live.
- Registering a custom block works end to end, as the sample's `SampleBlock` shows.
- The foundations are solid: nullable enabled, a warning-free build, `--bb-*` design tokens, `bb-`-prefixed class names, the native `<dialog>` element, and sanitised CSS classes with tests.

## 3. Problems found

Items marked **(seen)** were reproduced in the running sample. Everything else comes from reading the code.

### 3.1 Blockers for 1.0

| # | Problem | Where |
|---|---|---|
| B1 | **HTML/script injection in the rendered output.** Title, Quote, Image (`src`, `alt`) and Carousel insert user text without encoding. Typing `<b>bold</b>` into a Quote produced a real `<b>` element in the output **(seen)**, so something like `<img src=x onerror=…>` would run script on any page that displays the HTML. #21 only fixed CSS class names. | [QuoteBlockModel.cs:16](src/BlazorBlocks/Blocks/QuoteBlock/QuoteBlockModel.cs#L16), [TitleBlockModel.cs:28](src/BlazorBlocks/Blocks/TitleBlock/TitleBlockModel.cs#L28), [ImageBlockModel.cs:33](src/BlazorBlocks/Blocks/ImageBlock/ImageBlockModel.cs#L33), [CarouselBlockModel.cs:26](src/BlazorBlocks/Blocks/CarouselBlock/CarouselBlockModel.cs#L26) |
| B2 | **One unknown block type crashes the app.** Changing a single `$modelType` to an unregistered name and loading it brought up Blazor's "An unhandled error has occurred" bar **(seen)**. Removing a plugin block or renaming a block class makes every page that uses it unloadable. | [JsonSerializerTypeResolver.cs](src/BlazorBlocks/Internals/Models/JsonSerializerTypeResolver.cs), [BlazorBlocksModel.cs:66](src/BlazorBlocks/BlazorBlocksModel.cs#L66) |
| B3 | **Static registration and initialisation order** (#19, #22, #23). The registry is a pair of static lists, and each model's serializer captures them when it's constructed. Loading without `AddBlazorBlocks()` throws, which is why `LoadTests` fails. Calling `AddBlazorBlocks` twice duplicates every registration, and tests can't be isolated from each other. | [BlockRegistrationService.cs:17](src/BlazorBlocks/BlockRegistrationService.cs#L17), [BlazorBlocksModel.cs:26](src/BlazorBlocks/BlazorBlocksModel.cs#L26) |
| B4 | **Image height is never saved.** `Height` is a public *field*, and System.Text.Json skips fields. The preview showed `height="200"`, but the exported JSON had no Height at all **(seen)**. | [ImageBlockModel.cs:12](src/BlazorBlocks/Blocks/ImageBlock/ImageBlockModel.cs#L12) |
| B5 | **The host isn't told about many changes.** `OnModelUpdated` doesn't fire when a group is added, when a group or block is dragged, or when a Quote is edited (the preview stayed stale until "Render HTML" was clicked) **(seen)**. Carousel edits don't fire it either. A host that saves on change loses all of these edits. | [BlazorBlocksEditor.razor:26](src/BlazorBlocks/BlazorBlocksEditor.razor#L26), [BlazorBlocksEditor.razor:71](src/BlazorBlocks/BlazorBlocksEditor.razor#L71), [EditorColumn.razor:78](src/BlazorBlocks/Internals/Editors/EditorColumn.razor#L78), [QuoteBlockEditor.razor](src/BlazorBlocks/Blocks/QuoteBlock/QuoteBlockEditor.razor), [CarouselBlockEditor.razor](src/BlazorBlocks/Blocks/CarouselBlock/CarouselBlockEditor.razor) |
| B6 | **Dialog dead end.** Closing "Add group" with Esc leaves `IsOpen` set to `true` in Blazor, so no "Add group" button does anything until a reload **(seen)**. The "Add block" dialog uses the same component, so it should behave the same way. The dialogs have no close button either. | [Dialog.razor:13](src/BlazorBlocks/Internals/Components/Dialog.razor#L13), [NewGroupDialog.razor](src/BlazorBlocks/Internals/Components/Dialogs/NewGroupDialog.razor) |
| B7 | **The editor can't be used with a keyboard or screen reader.** The add buttons are `<div>`s and the delete, collapse and drag controls are `<span>`s, so the accessibility tree doesn't expose any of them as controls **(seen)**. Dragging is the only way to reorder. In the "Add block" dialog, "Raw text" is an `<img>` of the Iconify site's logo that fails to load, has no alt text and can't be focused **(seen)**. | [AddBlockRow.razor](src/BlazorBlocks/Internals/Containers/AddBlockRow.razor), [DraggableContainer.razor](src/BlazorBlocks/Internals/Containers/DraggableContainer.razor), [CollapseButton.razor](src/BlazorBlocks/Internals/Components/CollapseButton.razor), [RawTextBlockRegistration.cs](src/BlazorBlocks/Blocks/RawTextBlock/RawTextBlockRegistration.cs) |
| B8 | **The output CSS restyles the host page.** `blazorblocks.css`, the stylesheet meant for displaying content, includes global `button`, `input`, `textarea` and `select` rules and a global `*` box-sizing reset. | [blazorblocks-components.css:8](src/BlazorBlocks/wwwroot/blazorblocks-components.css#L8), [blazorblocks-layout.css:5](src/BlazorBlocks/wwwroot/blazorblocks-layout.css#L5) |
| B9 | **The output isn't responsive**, even though the project's recorded decision is that responsive layout is a first-class concern. The default layouts use fixed-width columns that never stack: at phone width the rendered 2-column group stayed side by side **(seen)**. Images have no `max-width`, so the demo image overflowed its column and made the whole page 2,740 px wide in a 1,440 px window **(seen)**. | [BlockRegistrationService.cs:162](src/BlazorBlocks/BlockRegistrationService.cs#L162), [blazorblocks-layout.css](src/BlazorBlocks/wwwroot/blazorblocks-layout.css) |
| B10 | **Not packaged or documented.** The project has no PackageId, version, description, license or readme metadata. The README says to clone `your-username/BlazorBlocks`, and custom blocks are marked "To Be Documented". | [BlazorBlocks.csproj](src/BlazorBlocks/BlazorBlocks.csproj), [README.md](README.md) |

### 3.2 Important: fix before 1.0 or soon after

- **The carousel isn't a carousel.** Its CSS hides every item except the first, caps the height at `100px` and offers no navigation. Its editor doesn't send change notifications either.
- **There's no block for body text.** The defaults are Title, Quote, Image, Carousel and "Raw Text", and Raw Text is really raw HTML. There's no safe way to write a paragraph.
- **The text "No image" leaks into the published HTML** whenever an Image block has no URL ([ImageBlockModel.cs:27](src/BlazorBlocks/Blocks/ImageBlock/ImageBlockModel.cs#L27)).
- **Memory leak:** each `EditorColumn` subscribes to `DragService.DraggedBlockChanged` and never unsubscribes, so old columns keep reacting to drags after `Load()` ([EditorColumn.razor:67](src/BlazorBlocks/Internals/Editors/EditorColumn.razor#L67)).
- **Blazor anti-patterns:** `async void CloseDialog`; `CollapseButton` overwrites its own parameter; the `Dialog.IsOpen` setter starts JS interop and fires events ([Dialog.razor:59](src/BlazorBlocks/Internals/Components/Dialog.razor#L59), [CollapseButton.razor:16](src/BlazorBlocks/Internals/Components/CollapseButton.razor#L16)).
- **Duplicate element IDs:** the Image and Title editors use fixed IDs such as `imageHeight` and `titleText`, so labels point at the wrong input once a page has two blocks of the same type. The Quote and Raw Text inputs have no label at all.
- **Debug logging ships in the library.** `Load()` calls `Console.WriteLine` throughout, including the garbled message from #20 **(seen in the browser console)**.
- **Deleting has no confirmation and no undo** (the code has a `// Add warning dialog here` comment at [BlazorBlocksEditor.razor:63](src/BlazorBlocks/BlazorBlocksEditor.razor#L63)).
- **Every column renders its own copy of the "Add block" dialog**: the sample's DOM had 4 of them for 4 columns **(seen)**.
- **Bootstrap leftovers (#14):** `form-group`/`form-control` in the Image and sample block editors, and `btn btn-info me-2` in the group dialog. Bootstrap is no longer loaded, so these classes do nothing.
- **The public API exposes internals.** `BlazorBlocksModel.Groups` returns types from `BlazorBlocks.Internals.Models`, and `CssHelper`, `ObjectDroppedResult`, `DragObjectType`, `AddBlockRow` and others are all public. The models also contain logic (`Render()`, `Load()`, `GetJson()`), which went against the "Models must not contain logic" guideline from the now-removed `.github/copilot-instructions.md`.
- **The registration API is awkward:** four `AddBlazorBlocks` overloads, told apart only by list type and by booleans with inconsistent defaults (`includeDefaultBlocks = false` in one, `includeDefaults = true` in another). `BlockRegistration` also takes untyped `Type` values.
- **The JSON format isn't stable.** The type discriminator is the CLR class name, so renaming a class, or two plugins using the same class name, breaks stored content. There's no format version, and the display name (`EditorName`) gets saved into the data.
- **Naming drift:** the code and UI correctly say *group*, but the README, issue titles (#12, #15) and some code comments and docs still say *row* from the very first version. *Group* is the settled term, because one group can span several visual rows, especially when responsive layouts stack its columns.

### 3.3 Cleanup

- **CSS:** the same rules appear both in `blazorblocks-editor.css` and in the component `.razor.css` files, and there are dead rules (the old custom dialog, drop zones, block headers). `.bb-padding-x-sm` uses the `md` spacing. The Title editor's CSS targets `.bb-title-editor__control`, but its markup uses `title-control` plus inline styles. The drag handle hard-codes `fill="#000000"`.
- **Dead code:** an unused `DragService` injection in `BlockContainer`; `_dragging`, `ActiveDraggableClass` and `SetReadyToDrop` in `EditorColumn`; commented-out dialog markup; VS-generated `ExcludeFromSingleFile` items in the csproj.
- **Sample leftovers:** an unused `NavMenu` (with Counter/Fetch data links and a "BlazorBaseBlocks" brand), an "About" link to docs.microsoft.com, unreferenced Bootstrap and open-iconic assets, an include for an empty `CustomBlocks\TestBlock\` folder, and an unnecessary `#pragma`.
- `TitleBlockModel` is `partial` for no reason, and `Model.Groups?.Count` uses `?.` on a list that is never null.

## 4. Repo & process debt

- **Squad leftovers: mostly removed locally (2026-10-04), not yet pushed.**
  - ✅ In the working tree, `.github/agents/squad.agent.md` and all 12 `squad-*` workflows are deleted, and no tracked file mentions Squad any more.
  - ⏳ The deletion isn't committed or pushed, so GitHub still lists 11 active Squad workflows. They stop once the commit reaches `main`.
  - ✅ All 26 Squad-era labels were deleted from GitHub on 2026-10-04. None was in use. The original 12 labels remain.
  - ✅ `.github/copilot-instructions.md` was deleted on purpose, along with the Copilot-specific setup. It held a "Models must not contain logic" guideline; whether that still applies is part of decision 5 in §9.
  - With the placeholder workflows gone, the repo has **no CI at all** until Phase 0 adds a real workflow.
- **No SDK pin.** There's no `global.json`, so builds use whichever SDK is newest on the machine. On this machine that's 11.0 RC1.
- **Unmerged design work.** `docs/editor-ux-suggestions` holds a thorough, prioritised editor UX review with mockups. #6 links to it, but it was never merged.
- **Decisions worth keeping from the deleted Squad log:** BlazorBlocks is a NuGet-distributed library; content is structured as page → groups → columns → blocks (originally "rows"); **responsive layout is a first-class concern**.

## 5. Open issue triage

| Issue | Verdict | Plan |
|---|---|---|
| #19 Deserialization init order | Real; it's the cause of the failing test | Phase 1 (registry/serializer redesign) |
| #22 Static mutable registration lists | Real | Phase 1 |
| #23 Type-info cache ignores options | Low impact as written: the cache belongs to each instance and isn't static. It disappears with the redesign. | Phase 1 |
| #20 Operator precedence in log string | Real, trivial | Phase 0 (delete the logging) |
| #12 Drag & drop | Mostly implemented | Phase 2: add a keyboard alternative and change notifications, then close |
| #14 Remove Bootstrap | About 90% done | Phase 3 |
| #6 Editor styling | A UX review already exists on a branch | Phase 3 |
| #15 More control over rows/columns | Partly possible today with a custom `GroupRegistration` | Responsive defaults in Phase 3; the rest after 1.0 |
| #16 Markdown block | — | Phase 4 (should-have, as an extension package) |
| #13 WYSIWYG block | — | After 1.0, as an extension package |
| #17 Base64 images | — | After 1.0 |
| #18 SVG block | — | After 1.0 |

## 6. What "1.0" should mean

Proposed definition of done:

1. **Easy to adopt.** `dotnet add package BlazorBlocks`, one registration call, `<BlazorBlocksEditor>` on a page, save and load JSON, render HTML. All of it is shown in the README and works in both Blazor WebAssembly and a Blazor Web App using interactive server rendering.
2. **Safe by default.** Every built-in block encodes its output. Raw HTML is opt-in and clearly marked as for trusted input only.
3. **Content survives.** Every block round-trips through JSON. Unknown block types load as placeholders and are saved back unchanged. The format carries a version number and is documented.
4. **The editor has no dead ends.** It works with both mouse and keyboard and tells the host about every change.
5. **Responsive by default.** The output adapts to screen width, and the CSS doesn't affect anything outside BlazorBlocks' own markup.
6. **No Bootstrap** anywhere.
7. **CI is green and real.** It runs unit tests for the model, serialization and rendering, plus bUnit tests for the main editor flows.
8. **The public API is reviewed and documented** (XML docs), and SemVer applies from then on.

## 7. Roadmap

Estimates are focused dev-days for one person who already knows the code.

### Phase 0: Clean slate & real CI (1–2 days)

- ~~Delete the Squad workflows and `squad.agent.md`~~ (done locally on 2026-10-04): commit and push the deletion (Squad labels already removed). Delete the merged `fix/issue-21…` branch. Merge the UX review doc from `docs/editor-ux-suggestions`, since it's the design input for Phase 3.
- Add a `global.json` that pins the 10.0.x SDK, and a `Directory.Build.props` for shared settings with warnings treated as errors.
- Replace CI with a single workflow that restores, builds and tests on every PR and every push to `main`, and keeps the output of `dotnet pack` as an artifact.
- Bump packages to the current patch versions.
- Remove the `Console.WriteLine` debugging from `Load()`, which closes #20.
- Add NuGet metadata (PackageId, version `1.0.0-preview.1`, description, MIT license expression, README, repository URL, tags, symbols) and turn on XML doc generation.
- **Exit:** CI shows red because of the genuinely failing test. Phase 1 fixes it first.

### Phase 1: Foundations: registry, format, rendering (5–7 days)

*This phase holds all the breaking changes, done once while nobody depends on the API.*

- **Registry via DI:** replace the static `BlockRegistrationService` with a registry that's built once at startup through a typed builder, for example `AddBlazorBlocks(b => b.AddDefaultBlocks().AddBlock<MyModel, MyEditor>("my-block", "My block"))`. This fixes #19, #22 and #23 and lets tests run in isolation.
- **Serializer as a service:** move JSON loading and saving out of `BlazorBlocksModel` into a service that gets the registry from DI and builds its options once.
- **Stable JSON format:**
  - Identify block types by explicit IDs instead of CLR class names.
  - Add a root `version` field.
  - Stop saving `EditorName`.
  - Use properties rather than fields, which fixes Image height.
  - Document the format.
- **Unknown blocks:** load them as placeholders that keep the original JSON and write it back on save. The editor shows "Unknown block: x", and the rendered output skips them.
- **Move rendering out of the models** into one renderer per block, which keeps the models as plain data (see decision 5 in §9). Encode HTML and attributes by default, and allow only safe URL schemes in `src`. Stop writing "No image" into the output.
- **Raw Text becomes an "HTML" block** that has to be registered explicitly and is documented as for trusted input only.
- **Public API shape:** move the document model out of `Internals` into a public namespace, make the true internals `internal`, and replace the remaining "row" wording in code comments, XML docs and parameter names (for example `rowToDelete`, "default rows") with "group".
- **Tests:** a round trip for each block, loading an unknown block, encoding of hostile input, and registry behaviour.
- **Exit:** the model tests pass, including the old `LoadTests`, and the format is documented.

### Phase 2: Editor correctness & accessibility (4–6 days)

- **Dialogs:**
  - Fix the dead end by syncing state when the native dialog is closed with Esc or a backdrop click.
  - Add a close button and manage focus.
  - Render one shared "Add block" dialog instead of one per column.
- **Change notifications:** fire one on **every** change, including adding a group, all moves and every block field. Fix live updates for Quote and Carousel. Consider standard `@bind-Model`-style naming.
- **Blazor fixes:** fix the `EditorColumn` subscription leak, remove the `async void`, and stop components from writing to their own parameters.
- **Delete confirmation:** ask before deleting groups or blocks that have content.
- **Accessibility:**
  - Make add, delete, collapse and drag real `<button>`s with labels.
  - Add a keyboard alternative to dragging: move up, move down, move to another column.
  - Give each block instance its own input IDs.
  - Label the Quote and HTML inputs.
  - Set `aria-expanded` on the collapse control.
- **bUnit tests** covering rendering a model, adding a group and a block, deleting, reopening a dialog after Esc, and change notifications firing.
- **Checkpoint: publish `1.0.0-preview.1` to NuGet.** It's safe, doesn't lose data and doesn't crash, but it still looks rough.

### Phase 3: Look, layout & CSS (4–6 days)

- Implement the high- and medium-priority items from the UX review (#6): a group header stripe, colour accents per nesting level, a styled dialog, empty-state placeholders, a chevron collapse icon and drag-handle states.
- Finish removing Bootstrap (#14) from the library and the sample.
- **Clean up the CSS:**
  - Scope it all under `.bb-editor` or a `.bb-content` wrapper.
  - Remove the global `*` reset from the output stylesheet.
  - De-duplicate the global and scoped CSS and delete dead rules.
  - Fix the Title editor's class mismatch and inline styles.
- **Responsive by default:**
  - Default layouts stack below the `md` breakpoint, using the `bb-layout-column--12 bb-layout-column--md-6` pattern.
  - Images get `max-width: 100%; height: auto`.
  - Editor columns stack on narrow screens too.
- **Exit:** the sample looks right at 375, 768 and 1440 px wide, and adding `blazorblocks.css` to a Bootstrap or Tailwind page changes nothing outside BlazorBlocks content.

### Phase 4: Block catalogue (3–5 days, +2–3 for Markdown)

- **New core "Text" block:** plain, encoded text where blank lines become paragraphs. It fills the "no body text" gap without adding any dependencies.
- **Polish the Title, Quote and Image editors:** labels, live updates, and an optional quote citation.
- **Carousel:** take it out of the defaults for 1.0, keeping the code and marking it experimental, then rebuild it in a 1.x release using CSS scroll-snap.
- **Markdown (#16):** make it the first extension package (`BlazorBlocks.Blocks.Markdown`, built on Markdig), which also tests the extension-package approach planned in #13. This is a should-have and can slip to 1.1.
- **Exit:** the default set is Title, Text, Quote and Image, plus the opt-in HTML block. Each has an editor, a renderer, and round-trip and encoding tests.
- **Checkpoint: publish `1.0.0-preview.2`.**

### Phase 5: Docs, samples, release (3–4 days)

- **Rewrite the README** to cover:
  - installing the package and registering it;
  - which CSS files to link;
  - the render-mode requirement (the editor needs an interactive render mode);
  - saving and loading;
  - displaying output safely;
  - writing a custom block (replacing "To Be Documented");
  - theming with the `--bb-*` tokens.
- **Samples:** clean up the WASM sample and add a Blazor Web App sample with interactive server rendering, since only WASM has ever been tried. Optionally deploy the WASM sample to GitHub Pages as a live demo.
- **Release workflow:** pushing a `v*` tag packs the library, pushes it to NuGet and creates a GitHub release. Start a CHANGELOG.
- **Final pass** over the public API and XML docs.
- **Release `1.0.0-rc.1`, then `1.0.0`.**

### Totals

| Milestone | Phases | Effort | Result |
|---|---|---|---|
| Usable preview | 0–2 | ~10–15 days | `1.0.0-preview.1` on NuGet: safe and stable, still rough-looking |
| Feature-complete | 3–4 | +7–11 days | `1.0.0-preview.2`: looks right, responsive, a useful set of blocks |
| 1.0 | 5 | +3–4 days | `1.0.0` |
| **Total** | | **~20–30 days** (+2–3 with Markdown) | |

## 8. After 1.0

Undo/redo · WYSIWYG block (#13) · code block · base64 images (#17) · SVG block (#18) · a real carousel · more per-group and per-column layout control (#15) · read-only/preview mode · a Blazor component renderer as an alternative to the HTML string · Playwright end-to-end tests for drag & drop · dark-mode tokens · translatable editor text.

## 9. Decisions for you

1. ~~**"Group" or "row"?**~~ **Decided: group.** "Rows" was the very first version's name. The scope changed so that one group can be split over multiple rows, especially in responsive layouts. To do: update the README, the titles of #12 and #15, and the leftover "row" wording in code comments and docs.
2. **Carousel:** drop it from the 1.0 defaults (recommended), or fix it now for another 2–3 days?
3. **Markdown in 1.0 or 1.1?** I'd include it in 1.0 if time allows, because it's the cheapest way to prove that extension packages work.
4. **HTML block:** opt-in and for trusted input only (recommended, no extra dependency), or sanitised (needs a sanitiser package, ideally in an extension)?
5. **Where rendering lives:** separate renderers (recommended: the models stay plain data, which is easier to serialize and test, and matches your earlier "no logic in models" guideline), or keep `Render()` on the block models (less churn, but it locks logic into the models in the 1.0 API)?

On the target framework: keep **net10.0 only**. .NET 8 and .NET 9 both reach end of support on 10 November 2026, and .NET 10 is supported until November 2028.

---

## Appendix: how this was checked

- Read every source, style, sample and test file on `main`. Also reviewed the open issues, PRs, branches, workflows and run history on GitHub, plus the Squad notes deleted in `fd12904`.
- `dotnet build` (full rebuild): 0 warnings, 0 errors. `dotnet test`: 14 passed and 1 failed (`LoadTests.LoadSemiComplicatedJson`, a `NotSupportedException` caused by #19).
- `dotnet list package --outdated` and `--vulnerable`: only patch updates, no vulnerabilities.
- Ran the WASM sample in a browser at 1440 px and 375 px wide. Reproduced B1, B2, B4, B5 (Quote), B6, B7 and B9, the debug logging, and the one-dialog-per-column issue.
- **Not verified:** drag & drop with a real mouse or on touch screens (checked in code only), hosting in Blazor Server or a Blazor Web App, screen readers (only the accessibility tree was inspected), and performance on large documents.
