using JelyUI;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Refit;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
if(!builder.Environment.IsDevelopment())  {
    builder.Configuration.AddEnvironmentVariables("AZURE_");
    builder.Configuration.AddAzureKeyVault(
        builder.Configuration["Azure:KeyVault:ResourceEndpoint"] 
            ?? builder.Configuration["KEYVAULT_RESOURCEENDPOINT"],
        builder.Configuration["Azure:KeyVault:ClientId"]
            ?? builder.Configuration["KEYVAULT_CLIENTID"],
        builder.Configuration["Azure:KeyVault:ClientSecret"]
            ?? builder.Configuration["KEYVAULT_CLIENTSECRET"]);
}
builder.Services.AddOptions<FacesApiConfig>().Bind(builder.Configuration.GetSection("FacesApi"));

builder.Services.AddOptions<FacesApiConfig>().Bind(builder.Configuration.GetSection("FacesApi"));
builder.Services.AddOptions<ConfidentialClientApplicationOptions>().Bind(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddSingleton(sp => ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(sp.GetRequiredService<IOptions<ConfidentialClientApplicationOptions>>().Value).Build());


builder.Services.AddTransient<AzureAuthHandler>();

var refitClientBuilder = builder.Services.AddRefitClient<IFacesApi>();
if(builder.Environment.IsProduction())
    refitClientBuilder.AddHttpMessageHandler<AzureAuthHandler>();
refitClientBuilder.ConfigureHttpClient((sp, client) => client.BaseAddress = new (sp.GetRequiredService<IOptions<FacesApiConfig>>().Value.BaseAddress));

builder.Services.AddScoped<IAppStateService, AppStateService>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

record FacesApiConfig {
    public string BaseAddress { get; init; } = "";
}