using Microsoft.Extensions.DependencyInjection;
using Tcc2.Application.Interfaces.Services;
using Tcc2.Application.Services;

namespace Tcc2.Application.Extensions;

public static class ApplicationConfigurationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services
            .AddTransient<IPersonService, PersonService>()
            .AddTransient<IPersonValidationService, PersonValidationService>()
            .AddTransient<IAddressValidationService, AddressValidationService>()
            .AddTransient<IActivityValidationService, ActivityValidationService>();
    }
}
