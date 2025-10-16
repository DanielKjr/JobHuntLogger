using Blazored.Modal;
using Blazored.SessionStorage;
using Blazored.Toast;
using JobHuntLogger.Components;
using JobHuntLogger.Services;
using JobHuntLogger.Utilities;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Serilog;
using Serilog.Core;


var builder = WebApplication.CreateBuilder(args);
var url = builder.Configuration.GetSection("Seq:Url").Value!;

builder.Services.AddRazorComponents(options =>
	options.DetailedErrors = builder.Environment.IsDevelopment()).AddInteractiveServerComponents();
//builder.Services.AddLogging();
//builder.Logging.AddConsole();
builder.Services.AddControllersWithViews();
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

//Blazored
builder.Services.AddBlazoredSessionStorage();
//https://github.com/Blazored/SessionStorage
builder.Services.AddBlazoredModal();
//https://blazored.github.io/Modal/
builder.Services.AddBlazoredToast();
//https://github.com/Blazored/Toast

//Adding the compose secrets json as a configuration source
//builder.Configuration
//.AddJsonFile("/run/secrets/dbinfo", optional: true, reloadOnChange: true);
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddHttpClient();
builder.Services.AddApiConfiguration(builder.Configuration);
//builder.Services.AddHttpClient();


var levelSwitch = new LoggingLevelSwitch();
builder.Host.UseSerilog((context, loggerconfig) =>
{
	loggerconfig.MinimumLevel.ControlledBy(levelSwitch);
	loggerconfig.Enrich.WithProperty("System: ", "JobHuntLogger").Enrich.WithHttpContextEnricher().WriteTo.Seq(url, apiKey: builder.Configuration.GetSection("Seq:ApiKey").Value, controlLevelSwitch: levelSwitch);
});
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
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();


app.Run();
