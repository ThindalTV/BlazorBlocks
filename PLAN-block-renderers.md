# Plan: split rendering out of the block Model into a `*Renderer`

## Today

- `BaseBlockModel` has `abstract string Render()` (`src/BlazorBlocks/Customization/BaseBlockModel.cs:21`). Every block model (Quote, Title, RawText, Image, Carousel, Sample) builds HTML with string interpolation.
- Call chain: `BlazorBlocksModel.GetHtml()` -> `EditorGroupModel.Render()` -> `EditorColumnModel.Render()` -> `block.Block.Render()`. It is synchronous and returns a `string`. `Index.razor` in the sample calls `GetHtml()` in two places.
- Registration is `BlockRegistration<TModel, TEditor>` via `RegisterBlock` / `AddBlazorBlockEditor<TModel, TEditor>`.
- Models also carry editor wiring that is not data: the `base(typeof(XEditor))` constructor arg, `Parameters`, `EditorName`, and `OnValuesChanged`.
- Existing tests are xunit in `BlazorBlocks.Tests.Model`. `LoadTests` only asserts that `GetHtml()` is not null.

## Design decision: .razor or .cs?

**Recommendation: support both behind one small interface, and write the built-in blocks as `.razor`.**

```csharp
public interface IBlockRenderer<in TModel> where TModel : BaseBlockModel
{
    // Contract the pipeline calls. Pure: model in, HTML out.
}
```

A `.razor` renderer is a normal `ComponentBase` with `[Parameter] Model`. This gives IntelliSense, markup, and HTML-encoding by default. It is rendered to a string with `Microsoft.AspNetCore.Components.Web.HtmlRenderer`. That class is already in the `Components.Web` 10.0 package the library references, so there is no new dependency.

A `.cs` renderer is a plain class with `string Render(TModel)`. It is for people who want zero Blazor machinery, and it is the cheapest to unit test.

Rejected alternatives:
- **Razor only.** This forces every custom block author through `HtmlRenderer`.
- **`.cs` only.** This gives up the IntelliSense you asked for.

### Razor caveats to accept up front

1. **Async.** `HtmlRenderer.RenderComponentAsync` is async, so the pipeline becomes `GetHtmlAsync()` (and `RenderAsync` down the chain). Don't make a sync wrapper with `.GetAwaiter().GetResult()`. It can deadlock on the single-threaded WASM runtime. Options:
   - Keep `GetHtml()` as `[Obsolete]` and make it work only if all renderers are the sync `.cs` kind.
   - Drop it (the 1.0 roadmap allows breaking changes). I lean towards dropping it and updating `Index.razor`.
2. **Encoding changes output.** Today `Quote`, `Title`, `Image.AltText` and the carousel fields are interpolated raw, which is an HTML-injection hole. Razor encodes them. This is the right outcome, but `LoadTests` JSON contains `<b>` in RawText, so RawText must explicitly emit `(MarkupString)Model.Content`. Call this out in the changelog.
3. **Needs services.** `HtmlRenderer` needs an `IServiceProvider` and an `ILoggerFactory`. `BlazorBlocksModel` currently has no DI, because it is `new`'d directly and `Load()` uses the static `BlockRegistrationService`. Plan: a `BlockHtmlRenderer` service. It is registered by `AddBlazorBlocks`, with a static `BlockHtmlRenderer.CreateStandalone()` using `NullLoggerFactory` and an empty provider. That keeps `new BlazorBlocksModel()` usable in tests and in plain server code.

## Target shape

```
Blocks/QuoteBlock/
  QuoteBlockModel.cs      // pure data: Quote only, no Render, no logic
  QuoteBlockEditor.razor
  QuoteBlockRenderer.razor  // <blockquote>@Model.Quote</blockquote>
```

- `BaseBlockModel` loses `Render()`. Its other responsibilities (`BlockType`/`Parameters`/`EditorName`) are editor metadata that should move to the registration. That is a bigger cleanup, so make it **phase 3, optional**, and keep it out of the first PR.
- `BlockRenderer<TModel> : ComponentBase` is the base class for razor renderers (mirrors `BlockEditor<T>`), with `[Parameter, EditorRequired] TModel Model`.
- `BlockRegistration<TModel, TEditor, TRenderer>`. The renderer type is stored on the registration so the pipeline can look it up by model type. A two-argument registration overload is kept only if a renderer can be discovered by convention, which is probably not worth it. Prefer an explicit third type parameter, because the roadmap is pre-1.0.
- Row/group/column rendering (`EditorGroupModel.Render`, `EditorColumnModel.Render`) also contains rendering logic. Give them the same treatment in phase 2 (`GroupRenderer.razor`, `ColumnRenderer.razor`) so the whole output pipeline is one mechanism. These are internal, so they carry no compatibility risk.

## Testability (the key requirement)

Layer 1, unit tests per renderer, no DI and no browser:
```csharp
await using var r = BlockHtmlRenderer.CreateStandalone();
var html = await r.RenderAsync<QuoteBlockRenderer>(new QuoteBlockModel { Quote = "<x>" });
Assert.Equal("<blockquote>&lt;x&gt;</blockquote>", html);
```
`.cs` renderers are called directly and need no harness at all. Add `RenderToString` test helpers in a small `BlazorBlocks.Testing` namespace (or inside the test project) so custom-block authors can reuse them.

Layer 2, pipeline tests: `BlazorBlocksModel.Load(json)` then `GetHtmlAsync()`, with exact-string assertions per block type. Replace the `Assert.NotNull` in `LoadTests` with real expected HTML.

Layer 3, regression guard: before changing anything, **capture today's output as golden strings** for every block type with a representative model (including empty `Image`, carousel items with a null `ImageUrl`, each `TitleSizeEnum`). Compare after the migration. Intentional differences (encoding) are updated by hand and listed in the changelog.

Test project: `BlazorBlocks.Tests.Model` already references the library (net10.0). No bUnit is needed, because we are testing string output, not interactive components. The only new package, if any, is `Microsoft.Extensions.Logging.Abstractions` (it is probably already transitive).

## Phases

1. **Safety net.** Add golden-output tests against the current `Render()` for all 5 built-in blocks + the sample block, plus the column/group wrappers. Nothing else changes.
2. **Infrastructure.** `IBlockRenderer<T>` (or equivalent), `BlockRenderer<TModel>` base, `BlockHtmlRenderer` service, the 3-type `BlockRegistration`, new `RegisterBlock` / `AddBlazorBlockEditor` signature (rename to something like `AddBlazorBlock<TModel, TEditor, TRenderer>`), and an async render pipeline. Keep `Render()` temporarily and call it as a fallback when no renderer is registered, so blocks can be migrated one at a time.
3. **Migrate blocks** one per commit: Quote, RawText, Title, Image, Carousel, then the Sample block. Each commit adds the renderer, removes `Render()` from the model, and updates the test expectations.
4. **Cleanup.** Remove `Render()` from `BaseBlockModel` and the fallback. Convert group/column rendering. Switch `Index.razor` to `GetHtmlAsync()`. Optionally move `EditorName` / `typeof(Editor)` / `Parameters` off the models.
5. **Docs.** Update the README / custom-block guide to a 3-file block (Model, Editor, Renderer), and add a changelog note on encoding and the async API.

## Risks and open questions

- **JSON compatibility.** Removing `Render()` doesn't touch serialization, but check `JsonSerializerTypeResolver` for anything that relies on the models' members. The stored JSON contains `EditorName`, and it stays unless phase 4 moves it. Make that a deliberate decision and don't let it drift.
- **HtmlRenderer in Blazor WASM.** It works, but it is untested in this repo, so add a sample-app smoke test in phase 2 before migrating all the blocks.
- **Whitespace.** Razor output differs slightly from the hand-built strings (indentation and newlines in the carousel). Golden tests will show this, and I'd accept the razor output as the new truth.
- **Decision for you:** drop sync `GetHtml()` (my recommendation), or keep it for `.cs`-only setups?
- **Decision for you:** explicit third type parameter on registration (my recommendation), or a convention like `[BlockRenderer(typeof(...))]` on the model?
