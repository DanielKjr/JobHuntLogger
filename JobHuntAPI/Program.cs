using DK.GenericLibrary.ServiceCollection;
using JobHuntAPI.Repository;
using JobHuntAPI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransientAsyncRepository<UserContext>();
builder.Services.AddDbContextFactory<UserContext>();

builder.Configuration
	.AddJsonFile("/run/secrets/dbinfo", optional: true, reloadOnChange: false);
builder.Services.AddTransient<LoginService>();

builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie().AddOpenIdConnect(o =>
{
	var oidcConfig = builder.Configuration.GetSection("OpenIdConnectSettings");


	//disable https, framework requires this but docker doesn't allow
	o.RequireHttpsMetadata = false;

	//values used in the body of API call
	o.Authority = oidcConfig["Authority"];
	o.ClientId = oidcConfig["ClientId"];
	o.ClientSecret = oidcConfig["ClientSecret"];
	o.CallbackPath = oidcConfig["RedirectUri"];
	o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	o.ResponseType = OpenIdConnectResponseType.Code;

	o.SaveTokens = true;
	o.GetClaimsFromUserInfoEndpoint = true;

	o.MapInboundClaims = false;
	o.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
	o.TokenValidationParameters.RoleClaimType = "roles";
});
var app = builder.Build();
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger(o => o.OpenApiVersion = OpenApiSpecVersion.OpenApi2_0);
app.UseSwaggerUI();
//}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
