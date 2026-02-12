using JobHuntAPI.Services;
using JobHuntAPI.Services.Interfaces;
using JobHuntAPI.Utility;
using JobHuntAPI.Utility.ServiceExtentions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);



builder.Services.RegisterRepository();

//TODO remove this later, used for caching during development
builder.Services.AddSingleton<IApplicationService, ApplicationService>();

builder.Services.AddTransient<IUserHelper, UserHelper>();
//builder.Services.AddScoped<IApplicationService, ApplicationService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpContextEnricher>();

builder.Services.ConfigureSwagger();
builder.Configuration.AddJsonSecrets();
builder.Host.AddSerilogEnricher();


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
