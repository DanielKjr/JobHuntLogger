using Blazored.Modal;
using Blazored.SessionStorage;
using Blazored.Toast;
using JobHuntLogger.Components;
using JobHuntLogger.Services.Authentication;
using JobHuntLogger.Utilities.Extensions;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents(options =>
	options.DetailedErrors = builder.Environment.IsDevelopment()).AddInteractiveServerComponents();
builder.Services.AddControllersWithViews();
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredToast();

//Adding the compose secrets json as a configuration source
//builder.Configuration.AddJsonFile("/run/secrets/dbinfo", optional: true, reloadOnChange: true);


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddHttpClient();


builder.Host.AddSerilogEnricher();
builder.Services.AddLoggingConfiguration(builder.Configuration);
builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.RegisterTokenCache(builder.Configuration);
builder.Services.RegisterAuthenticationAndAuthorization(builder.Configuration);

//Optional if login should be forced
//builder.Services.AddAuthorizationBuilder()
//	.SetFallbackPolicy(new AuthorizationPolicyBuilder()
//		.RequireAuthenticatedUser()
//		.Build());

var app = builder.Build();


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


app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();


app.MapControllers();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();


app.Run();
