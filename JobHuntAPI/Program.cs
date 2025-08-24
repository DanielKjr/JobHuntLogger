using DK.GenericLibrary.ServiceCollection;
using JobHuntAPI.Repository;
using JobHuntAPI.Services;
using Microsoft.OpenApi;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransientAsyncRepository<UserContext>();
builder.Services.AddDbContextFactory<UserContext>();


builder.Services.AddTransient<AuthenticationService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger(o => o.OpenApiVersion = OpenApiSpecVersion.OpenApi2_0);
app.UseSwaggerUI();
//}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
