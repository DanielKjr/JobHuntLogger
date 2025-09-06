using AuthorizationServer;
using AuthorizationServer.Contexts;
using AuthorizationServer.Workers;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

//Get configuration from docker secret
builder.Configuration
	.AddJsonFile("/run/secrets/dbinfo", optional: true, reloadOnChange: false);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<HttpClient>();
builder.Services.AddTransient<LoginClient>(o => new LoginClient("http://jobhuntapi:8080/", new HttpClient()));
builder.Services.AddDbContext<AuthenticationContext>();


builder.Services.AddOpenIddict()
	.AddCore(options =>
	{
		options.UseEntityFrameworkCore()
			.UseDbContext<AuthenticationContext>();

	})
	.AddServer(options =>
	{

		options.SetTokenEndpointUris("/connect/token")
			.SetAuthorizationEndpointUris("/connect/authorize");

		options.AllowClientCredentialsFlow()
			.AllowAuthorizationCodeFlow();

		options.AddDevelopmentEncryptionCertificate()
			.AddDevelopmentSigningCertificate();

		//disable transport security is for local development only, the framework will go through https which docker doesn't allow, so it was
		//either this or dealing with SSL certificates.
		options.UseAspNetCore().EnableAuthorizationEndpointPassthrough().DisableTransportSecurityRequirement();

		options.AllowPasswordFlow();
		options.RegisterScopes(
			OpenIddictConstants.Scopes.OpenId,
			OpenIddictConstants.Scopes.Profile
		);


	})
	.AddValidation(options =>
	{
		options.UseLocalServer();
		options.UseAspNetCore();

	});

builder.Services.AddHostedService<ApiWorker>();
builder.Services.AddHostedService<BlazorWorker>();




var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseRouting();
app.UseCors();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// openiddict needs a callback method to validate the requests. This returns the code used in fetching token
//app.MapGet("/signin-oidc", (HttpContext ctx) =>
//{
//	var code = ctx.Request.Query["code"];
//	return Results.Text(code);
//});

app.Run();
