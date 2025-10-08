using JobHuntLogger.Components;
using JobHuntLogger.Services;
using JobHuntLogger.Utilities;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents(options =>
	options.DetailedErrors = builder.Environment.IsDevelopment()).AddInteractiveServerComponents();
builder.Services.AddLogging();
builder.Logging.AddConsole();
builder.Services.AddControllersWithViews();
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

//Adding the compose secrets json as a configuration source
builder.Configuration
.AddJsonFile("/run/secrets/dbinfo", optional: true, reloadOnChange: true);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddHttpClient();
builder.Services.AddApiConfiguration();
//builder.Services.AddHttpClient();
var configuration = builder.Configuration;

builder.Services.ConfigureAuthenticationAndAuthorization(configuration);

//Optional enforce authorize on all pages
//builder.Services.AddAuthorizationBuilder()
//	.SetFallbackPolicy(new AuthorizationPolicyBuilder()
//		.RequireAuthenticatedUser()
//		.Build());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
