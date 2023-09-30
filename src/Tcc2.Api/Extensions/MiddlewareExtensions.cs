using Tcc2.Api.Middlewares;

namespace Tcc2.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IServiceCollection AddMiddlewares(this IServiceCollection services)
    {
        return services
            .AddTransient<ErrorHandlerMiddleware>();
    }

    public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<ErrorHandlerMiddleware>();
    }
}
