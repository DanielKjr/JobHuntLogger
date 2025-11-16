using Blazored.Modal;
using Blazored.SessionStorage;
using Blazored.Toast;
using JobHuntLogger.Components;
using JobHuntLogger.Services;
using JobHuntLogger.Utilities;
using JobHuntLogger.Utilities.Extensions;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Serilog;
using Serilog.Core;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents(options =>
	options.DetailedErrors = builder.Environment.IsDevelopment()).AddInteractiveServerComponents();
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

//configure logging
var levelSwitch = new LoggingLevelSwitch();
builder.Host.UseSerilog((context, services, loggerconfig) =>
{
	// Retrieve IHttpContextAccessor from the host's service provider to pass into the enricher
	var httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();

	loggerconfig.MinimumLevel.ControlledBy(levelSwitch);
	// Use a clean property name
	loggerconfig.Enrich.WithProperty("Application", "JobHuntLogger")
		.Enrich.WithHttpContextEnricher(httpContextAccessor)
		.WriteTo.Seq(context.Configuration.GetValue<string>("Seq:Url")!, apiKey: context.Configuration.GetValue<string>("Seq:ApiKey"), controlLevelSwitch: levelSwitch);
	loggerconfig.WriteTo.Console();
});

builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.RegisterTokenCache(builder.Configuration);
builder.Services.RegisterAuthorization(builder.Configuration);

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
	app.UseHsts();
}
else
{
	app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//ensure authentication/authorization is run before endpoints are executed
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();


app.MapControllers();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();


app.Run();
