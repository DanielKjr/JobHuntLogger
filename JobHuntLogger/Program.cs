using JobHuntLogger.Components;
using JobHuntLogger.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Graph;
using Microsoft.Identity.Web;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();
builder.Services.AddControllersWithViews();
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
builder.Configuration
.AddJsonFile("/run/secrets/dbinfo", optional: true, reloadOnChange: true);

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JobHuntApiService>();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("Entra:Blazor"))
	.EnableTokenAcquisitionToCallDownstreamApi(new[] { "User.Read" })
	.AddMicrosoftGraph()     
	.AddInMemoryTokenCaches();



builder.Services.AddAuthorization();

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
