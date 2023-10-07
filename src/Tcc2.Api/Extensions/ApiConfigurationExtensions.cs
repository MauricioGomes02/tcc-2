using Microsoft.AspNetCore.Mvc;
using Tcc2.Api.Interfaces.Services;
using Tcc2.Api.Services;

namespace Tcc2.Api.Extensions;

public static class ApiConfigurationExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        return services.AddTransient<IHateoasGeneratorService, HateoasGeneratorService>();
    }
}
