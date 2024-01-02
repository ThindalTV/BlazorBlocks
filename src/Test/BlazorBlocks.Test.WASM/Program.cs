using BlazorBlocks;

using BlazorBlocks.Model;
using BlazorBlocks.Test.WASM;
using BlazorBlocks.Test.WASM.CustomBlocks.TestBlock;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

/*builder.Services.AddScoped<List<BlockRegistration>>(sp =>
{
    return new List<BlockRegistration>()
    {
        new BlockRegistration("Test Block", null, typeof(TestBlockModel), typeof(TestBlockEditor))
    };
});*/

builder.Services.AddBlazorBlocks(new List<BlockRegistration>()
{
    new BlockRegistration("Test Block", null, typeof(TestBlockModel), typeof(TestBlockEditor))
});

await builder.Build().RunAsync();
