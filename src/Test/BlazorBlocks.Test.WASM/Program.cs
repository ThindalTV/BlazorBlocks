using BlazorBlocks;
using BlazorBlocks.Model.Registrations;
using BlazorBlocks.Test.WASM;
using BlazorBlocks.Test.WASM.CustomBlocks.TestBlock;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazorBlocks(new List<BlazorBlocksBlockRegistration>()
{
    new BlazorBlocksBlockRegistration("Test Block", null, typeof(TestBlockModel), typeof(TestBlockEditor))
}, true);

await builder.Build().RunAsync();
