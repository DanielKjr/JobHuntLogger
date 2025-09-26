using DK.GenericLibrary.ServiceCollection;
using JobHuntAPI.Repository;
using JobHuntAPI.Services;
using Microsoft.Identity.Web;
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
