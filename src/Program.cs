using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Shared.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddDbContext<AcademiaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Academia")
        ?? throw new InvalidOperationException("Missing connection string 'Academia'.")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program;
