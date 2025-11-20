# BlazorBlocks

BlazorBlocks is a WYSIWYG block editor for Blazor.

## Features

- Drag and drop blocks to create dynamic web pages
- Customize block properties and styles
- Real-time preview of the web page as you edit
- Export and import blocks for reusability
- Extensible with custom block types

## Getting Started

To get started with BlazorBlocks, follow these steps:

1. Clone the repository: `git clone https://github.com/your-username/BlazorBlocks.git`
2. Open the solution in Visual Studio
3. Build and run the project
4. Access the BlazorBlocks editor in your web browser

In the future, the project will be available as a nuget package.
As we're not at a stable release yet, we recommend cloning the repository and building the project yourself.

## Usage

BlazorBlocks provides an intuitive interface for creating and editing web page content. Here's how you can use it:

1. Add blocks onto the canvas to add them to your page
2. Customize the properties and styles of each block using the editor panel
3. Preview your changes in real-time to see how the page will look
4. Export blocks to reuse
5. Extend BlazorBlocks with custom block types to add new functionality

## Writing a new block type
To Be Documented... Example in [SampleBlockEditor](./src/Samples/BlazorBlocks.Sample.WASM/CustomBlocks/SampleBlock/SampleBlockEditor.razor)
and [SampleBlockModel](./src/Samples/BlazorBlocks.Sample.WASM/CustomBlocks/SampleBlock/SampleBlockModel.cs) for now.

## Contributing

Contributions are welcome!
If you have any ideas, suggestions, or bug reports, please open an issue or submit a pull request.

## License

BlazorBlocks is licensed under the MIT License. See [LICENSE](./LICENSE) for more information.
