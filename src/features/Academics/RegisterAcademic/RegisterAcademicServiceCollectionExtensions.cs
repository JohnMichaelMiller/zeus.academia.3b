using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Zeus.Academia.Features.Academics.RegisterAcademic.Register;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public static class RegisterAcademicServiceCollectionExtensions
{
  public static IServiceCollection AddRegisterAcademicPersistence(
    this IServiceCollection services,
    string connectionString)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

    services.AddDbContext<RegisterAcademicDbContext>(options =>
      options.UseSqlServer(connectionString));

    return services;
  }

  public static IServiceCollection AddRegisterAcademicMediatR(
    this IServiceCollection services)
  {
    services.AddValidatorsFromAssembly(typeof(RegisterAcademicCommandValidator).Assembly);

    services.AddMediatR(cfg =>
      cfg.RegisterServicesFromAssembly(typeof(RegisterAcademicDbContext).Assembly));

    return services;
  }
}
