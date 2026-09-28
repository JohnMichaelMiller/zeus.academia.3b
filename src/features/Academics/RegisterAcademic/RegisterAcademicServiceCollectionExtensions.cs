using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public static class RegisterAcademicServiceCollectionExtensions
{
  public static IServiceCollection AddRegisterAcademicMediatR(this IServiceCollection services)
  {
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterAcademicCommand).Assembly));
    services.AddScoped<IValidator<RegisterAcademicCommand>, RegisterAcademicCommandValidator>();
    return services;
  }
}
