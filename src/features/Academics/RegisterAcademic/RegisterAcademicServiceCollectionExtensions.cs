using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public static class RegisterAcademicServiceCollectionExtensions
{
  public static IServiceCollection AddRegisterAcademicPersistence(
    this IServiceCollection services,
    string connectionString)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
    services.AddDbContext<RegisterAcademicDbContext>(options => options.UseSqlServer(connectionString));
    return services;
  }

  public static IServiceCollection AddRegisterAcademicMediatR(this IServiceCollection services)
  {
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterAcademicHandler).Assembly));
    services.AddScoped<IValidator<RegisterAcademicCommand>, RegisterAcademicCommandValidator>();
    return services;
  }
}
