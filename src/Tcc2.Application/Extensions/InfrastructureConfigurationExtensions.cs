using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tcc2.Domain.Interfaces.Infrastructure.Repositories;
using Tcc2.Domain.Interfaces.Infrastructure.Services;
using Tcc2.Infrastructure.Persistence.RelationalDatabase;
using Tcc2.Infrastructure.Persistence.RelationalDatabase.Repositories;
using Tcc2.Infrastructure.Services;

namespace Tcc2.Application.Extensions;

public static class InfrastructureConfigurationExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Tcc")!;
        return services
            .AddDbContext<TccContext>(options => options
                .UseMySQL(connectionString)
                .LogTo(Console.WriteLine))
            .AddTransient<IGeographicCoordinateService, GeographicCoordinateService>();
    }

    public static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
    {
        return services.AddTransient<IPersonRepository, PersonRepository>();
    }

    public static IServiceCollection AddInfrastructureHttpClients(this IServiceCollection services)
    {
        return services.AddHttpClient();
    }
}
