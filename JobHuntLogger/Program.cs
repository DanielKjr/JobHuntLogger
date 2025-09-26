using JobHuntLogger.Components;
using JobHuntLogger.Components.Model.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();



builder.Services.AddRazorPages();
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
builder.Configuration
	.AddJsonFile("/run/secrets/dbinfo", optional: true, reloadOnChange: false);
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JobHuntApiService>();
builder.Services.AddHttpClient();



builder.Services.AddScoped<UserSession>();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("Entra:Blazor")).EnableTokenAcquisitionToCallDownstreamApi()
	.AddInMemoryTokenCaches();

// For Windows authentication
				 //forces all pages to require authentication by default
				 //builder.Services.AddAuthorization(options =>
				 //{
				 //	options.FallbackPolicy = new AuthorizationPolicyBuilder()
				 //		.RequireAuthenticatedUser()
				 //		.Build();
				 //});
builder.Services.AddAuthorization();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
