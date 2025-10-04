using JobHuntLogger.Components;
using JobHuntLogger.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents(options =>
	options.DetailedErrors = builder.Environment.IsDevelopment()).AddInteractiveServerComponents();


builder.Services.AddControllersWithViews();
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

//Adding the compose secrets json as a configuration source
builder.Configuration
.AddJsonFile("/run/secrets/dbinfo", optional: true, reloadOnChange: true);

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JobHuntApiService>();
builder.Services.AddScoped<AuthenticationService>();

builder.Services.AddHttpClient();
var Configuration = builder.Configuration;

//Entra with SQL distributed token cache
builder.Services.AddDistributedSqlServerCache(options =>
{
	options.ConnectionString = Configuration.GetConnectionString("TokenDb");
	options.SchemaName = "Tokens";
	options.TableName = "TokenCache";
});
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("Entra:Blazor"))
	.EnableTokenAcquisitionToCallDownstreamApi(new[] { "User.Read" })
	.AddMicrosoftGraph().
	AddDistributedTokenCaches();


builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

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
