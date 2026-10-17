using BlazorBlocks.Sample.WASM;
using BlazorBlocks.Sample.WASM.CustomBlocks.SampleBlock;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorBlocks;
using BlazorBlocks.Blocks.Markdown;
using BlazorBlocks.Blocks.Wysiwyg;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazorBlocks()
    .AddMarkdownBlock()
    .AddWysiwygBlock()
    .AddBlazorBlock<SampleBlockModel, SampleBlockEditor, SampleBlockRenderer>("Sample");

await builder.Build().RunAsync();


