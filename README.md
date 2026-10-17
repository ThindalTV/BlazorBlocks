# BlazorBlocks

**A block-based page editor for Blazor.** Let your users build rich, structured pages by arranging blocks in a visual editor, while you keep full control of what blocks exist and how content is stored and rendered.

BlazorBlocks is a Razor class library. Drop the `BlazorBlocksEditor` component into any Blazor app, give it a model, and you get an editor that produces clean HTML and round-trips to JSON.

> **Status: pre-release.** BlazorBlocks is a working prototype and has not been published to NuGet yet. APIs and the JSON format may still change. See [ROADMAP.md](./ROADMAP.md) for the path to 1.0.

## How it works

Content is organised in three levels:

- **Groups** are the layout containers on a page (1, 2, 3 or 4 columns, 1+2, and so on).
- **Columns** live inside a group.
- **Blocks** live inside columns and hold the actual content.

Editors add, remove, collapse and drag & drop groups and blocks, edit each block's properties, and see the rendered result as they go. Your app persists the result as JSON and renders it as HTML.

## Features

- Drag and drop groups and blocks, including moving blocks between columns
- Six built-in group layouts
- Built-in blocks: Title, Raw Text, Image, Quote and Carousel
- Live HTML preview of the page being edited
- JSON import and export of the whole page
- Custom block types: register your own model and editor component
- Theming through `--bb-*` CSS custom properties and `bb-`-prefixed class names

## Getting started

BlazorBlocks targets .NET 10. Until a stable release is on NuGet, clone the repository and reference the project directly:

```bash
git clone https://github.com/ThindalTV/BlazorBlocks.git
```

Open `src/BlazorBlocks.slnx` in Visual Studio or Rider and run `BlazorBlocks.Sample.WASM` to see the editor in action.

To use it in your own app, add a project reference to `src/BlazorBlocks/BlazorBlocks.csproj`, then register the services in `Program.cs`:

```csharp
builder.Services.AddBlazorBlocks();
```

and use the editor in a page:

```razor
<BlazorBlocksEditor Model="_model" OnModelUpdated="OnModelUpdated" />

@code {
    private readonly BlazorBlocksModel _model = new();

    private void OnModelUpdated()
    {
        // Persist or re-render the model here.
    }
}
```

The sample's [Index.razor](./src/Samples/BlazorBlocks.Sample.WASM/Pages/Index.razor) shows rendering to HTML and JSON import/export.

## Writing a custom block

A custom block is a model plus an editor component, registered at startup:

```csharp
builder.Services.AddBlazorBlocks()
    .AddBlazorBlockEditor<SampleBlockModel, SampleBlockEditor>("Sample");
```

Full documentation is still to come. For now, see [SampleBlockEditor](./src/Samples/BlazorBlocks.Sample.WASM/CustomBlocks/SampleBlock/SampleBlockEditor.razor) and [SampleBlockModel](./src/Samples/BlazorBlocks.Sample.WASM/CustomBlocks/SampleBlock/SampleBlockModel.cs).

## Project layout

| Path | Contents |
|---|---|
| `src/BlazorBlocks` | The library: editor component, models, built-in blocks and groups |
| `src/Samples/BlazorBlocks.Sample.WASM` | Blazor WebAssembly demo app |
| `src/Tests` | xUnit tests |
| `Web` | Source of the project's GitHub Pages site |

## Contributing

Contributions are welcome! If you have ideas, suggestions or bug reports, please open an issue or submit a pull request.

## License

BlazorBlocks is licensed under the MIT License. See [LICENSE](./LICENSE) for details.
