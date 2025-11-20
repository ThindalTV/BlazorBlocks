using BlazorBlocks.Services;
using BlazorBlocks.Services.Registrations;
using BlazorBlocks.Sample.WASM;
using BlazorBlocks.Sample.WASM.CustomBlocks.SampleBlock;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazorBlocks([new SampleBlockRegistration()], true);

await builder.Build().RunAsync();


