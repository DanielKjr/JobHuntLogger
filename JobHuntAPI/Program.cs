using DK.GenericLibrary.ServiceCollection;
using JobHuntAPI.Repository;
using JobHuntAPI.Services;
using JobHuntAPI.Services.Interfaces;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScopedAsyncRepository<ApplicationContext>();
builder.Services.AddDbContextFactory<ApplicationContext>();

builder.Services.AddScoped<IApplicationService, ApplicationService>();

builder.Configuration
	.AddJsonFile("/run/secrets/dbinfo", optional: true, reloadOnChange: true);

builder.Configuration
	.AddJsonFile("/run/secrets/api", optional: true, reloadOnChange: true);


builder.Services.AddAuthentication("Bearer")
	.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("Entra:JobHuntApi"));


builder.Services.AddAuthorization();
var app = builder.Build();
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger(o => o.OpenApiVersion = OpenApiSpecVersion.OpenApi2_0);
app.UseSwaggerUI();
//}


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
