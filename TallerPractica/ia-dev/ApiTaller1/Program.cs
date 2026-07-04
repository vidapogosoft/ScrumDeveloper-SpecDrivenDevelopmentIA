using ApiTaller1.Interface;
using ApiTaller1.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddScoped<IOrdenRecibo, FakeOrdenReciboService>();
}
else
{
    builder.Services.AddScoped<IOrdenRecibo, ServicesOrdenRecibo>();
}

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
