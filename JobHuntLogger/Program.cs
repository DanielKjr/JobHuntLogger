using JobHuntLogger.Components;
using JobHuntLogger.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents(options =>
	options.DetailedErrors = builder.Environment.IsDevelopment()).AddInteractiveServerComponents();

builder.Services.AddControllersWithViews();
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
//builder.Configuration
//.AddJsonFile("/run/secrets/dbinfo", optional: true, reloadOnChange: true);

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JobHuntApiService>();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("Entra:Blazor"))
	.EnableTokenAcquisitionToCallDownstreamApi(new[] { "User.Read" })
	.AddMicrosoftGraph()
	.AddInMemoryTokenCaches();
builder.Services.Configure<CookieAuthenticationOptions>(options =>
{
	options.Cookie.Name = "PersistentLogin";

	options.ExpireTimeSpan = TimeSpan.FromDays(14); // Persistent for 14 days
	options.SlidingExpiration = true;
});


builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

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
