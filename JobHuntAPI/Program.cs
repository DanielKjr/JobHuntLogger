using DK.GenericLibrary.ServiceCollection;
using JobHuntAPI.Repository;
using JobHuntAPI.Services;
using JobHuntAPI.Services.Interfaces;
using JobHuntAPI.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region swaggerConfig
builder.Services.AddControllers()
	.AddJsonOptions(o =>
	{
		o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{

	c.SwaggerDoc("v1", new OpenApiInfo { Title = "JobHuntAPI", Version = "v1" });
	//kept in case I want to retry required props on the dtos
	//c.SupportNonNullableReferenceTypes();
	//c.UseAllOfToExtendReferenceSchemas();
	// Add JWT Bearer token support for Swagger UI
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = JwtBearerDefaults.AuthenticationScheme
				}
			},
			Array.Empty<string>()
		}
	});
});
#endregion

//singletons to fit with applicationservice for caching
builder.Services.AddSingletonAsyncRepository<ApplicationContext>();
builder.Services.AddSingletonRepository<ApplicationContext>();

builder.Services.AddDbContextFactory<ApplicationContext>();
builder.Services.AddTransient<IDesignTimeDbContextFactory<ApplicationContext>, ApplicationContextFactory>();
builder.Services.AddScoped<IPdfService, PdfService>();

//TODO remove this later, used for caching during development
builder.Services.AddSingleton<IApplicationService, ApplicationService>();

//builder.Services.AddScoped<IApplicationService, ApplicationService>();

#region dockerConfigs
builder.Configuration
	.AddJsonFile("/run/secrets/dbinfo", optional: false, reloadOnChange: true);

builder.Configuration
	.AddJsonFile("/run/secrets/apiinfo", optional: false, reloadOnChange: true);
#endregion

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpContextEnricher>();

//Logging
var levelSwitch = new LoggingLevelSwitch();
builder.Host.UseSerilog((context, services, loggerconfig) =>
{
	var httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();

	loggerconfig.MinimumLevel.ControlledBy(levelSwitch);
	loggerconfig.Enrich.WithProperty("Application", "JobHuntAPI")
		.Enrich.WithHttpContextEnricher(httpContextAccessor)
		.WriteTo.Seq(context.Configuration.GetValue<string>("Seq:Url")!, apiKey: context.Configuration.GetValue<string>("Seq:ApiKey"), controlLevelSwitch: levelSwitch);
	loggerconfig.WriteTo.Console();
});

//CORS
//TODO cors configuration not confirmed to be functioning right yet.
var frontEnd = "https://localhost:44394";
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
	{
		policy.WithOrigins(frontEnd)
			  .AllowAnyMethod()
			  .WithHeaders("Authorization", "Content-Type")
			  .AllowCredentials();
	});
});

//Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("Entra:JobHuntApi"));

builder.Services.AddAuthorization();
var app = builder.Build();
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}


app.UseHttpsRedirection();
app.UseRouting();

//Part of above todo, no clue if works
app.UseCors("AllowFrontend");
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();




app.Run();
