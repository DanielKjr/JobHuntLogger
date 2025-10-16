using DK.GenericLibrary.ServiceCollection;
using JobHuntAPI.Repository;
using JobHuntAPI.Services;
using JobHuntAPI.Services.Interfaces;
using JobHuntAPI.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{

	c.SwaggerDoc("v1", new OpenApiInfo { Title = "JobHuntAPI", Version = "v1" });

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


builder.Services.AddScopedAsyncRepository<ApplicationContext>();
builder.Services.AddDbContextFactory<ApplicationContext>();

builder.Services.AddScoped<IApplicationService, ApplicationService>();

builder.Configuration
	.AddJsonFile("/run/secrets/dbinfo", optional: false, reloadOnChange: true);

builder.Configuration
	.AddJsonFile("/run/secrets/apiinfo", optional: false, reloadOnChange: true);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpContextEnricher>();
var url = builder.Configuration.GetSection("Seq:Url").Value!;
var levelSwitch = new LoggingLevelSwitch();
builder.Host.UseSerilog((context, loggerconfig) =>
{
	loggerconfig.MinimumLevel.ControlledBy(levelSwitch);
	loggerconfig.Enrich.WithProperty("System: ", "JobHuntApi").Enrich.WithHttpContextEnricher().WriteTo.Seq(url, apiKey: builder.Configuration.GetSection("Seq:ApiKey").Value, controlLevelSwitch: levelSwitch);
});


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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



app.Run();
