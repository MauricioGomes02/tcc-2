using Tcc2.Api.Extensions;
using Tcc2.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDomainServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddInfrastructureRepositories();
builder.Services.AddInfrastructureHttpClients();
builder.Services.AddApplicationServices();
builder.Services.AddApiServices();
builder.Services.AddMiddlewares();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
    app.AddDaysRecord();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddlewares();

app.Run();
