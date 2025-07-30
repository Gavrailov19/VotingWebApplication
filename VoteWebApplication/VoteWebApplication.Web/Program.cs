using VoteWebApplication.Web;
using VoteWebApplication.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddHttpClient("PollApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["PollApiBaseUrl"] ?? "https://localhost:7524/");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();