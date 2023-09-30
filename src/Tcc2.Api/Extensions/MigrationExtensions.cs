using Microsoft.EntityFrameworkCore;
using Tcc2.Infrastructure.Persistence.RelationalDatabase;

namespace Tcc2.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TccContext>();
        context.Database.Migrate();
    }
}
