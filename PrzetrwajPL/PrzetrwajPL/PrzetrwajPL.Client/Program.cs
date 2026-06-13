using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Przetrwaj.CommonLibrary.Consts;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHttpClient(Consts.PrzetrwajApiClientName, client =>
{
	client.BaseAddress =
	//new Uri("https://przetrwaj-api.grayflower-7f624026.polandcentral.azurecontainerapps.io/");
	new Uri("https://localhost:7072/");
})
//.AddHttpMessageHandler<JwtAuthorizationHandler>(); // This links the handler to "ServerAPI"
;

await builder.Build().RunAsync();
