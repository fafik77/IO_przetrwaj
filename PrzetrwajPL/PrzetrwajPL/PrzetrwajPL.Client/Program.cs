using System.Globalization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Przetrwaj.CommonLibrary.Consts;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var polishCulture = new CultureInfo("pl-PL");
CultureInfo.DefaultThreadCurrentCulture = polishCulture;
CultureInfo.DefaultThreadCurrentUICulture = polishCulture;

await builder.Build().RunAsync();
