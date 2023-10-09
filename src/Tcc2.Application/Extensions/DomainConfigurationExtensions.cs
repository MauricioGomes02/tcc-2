using Microsoft.Extensions.DependencyInjection;
using Tcc2.Domain.Interfaces.Services;
using Tcc2.Domain.Services;

namespace Tcc2.Application.Extensions;

public static class DomainConfigurationExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services
            .AddTransient<IGeographicProximityService, GeographicProximityService>()
            .AddTransient<IFunctionalProximityService, FunctionalProximityService>()
            .AddTransient<IPersonService, PersonService>();
    }
}
