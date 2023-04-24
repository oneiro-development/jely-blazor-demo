using JelyUI;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Refit;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRefitClient<IFacesApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5001"));
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
