using Microsoft.EntityFrameworkCore;
using Tcc2.Domain.Entities;
using Tcc2.Infrastructure.Persistence.RelationalDatabase;

namespace Tcc2.Api.Extensions;

public static class DatabaseExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TccContext>();
        context.Database.Migrate();
    }

    public static void AddDaysRecord(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TccContext>();
        if (context.Days.Any())
        {
            return;
        }

        context.Days.AddRange(
            new Day((short)DayOfWeek.Sunday),
            new Day((short)DayOfWeek.Monday),
            new Day((short)DayOfWeek.Tuesday),
            new Day((short)DayOfWeek.Wednesday),
            new Day((short)DayOfWeek.Thursday),
            new Day((short)DayOfWeek.Friday),
            new Day((short)DayOfWeek.Saturday));
        context.SaveChanges();
    }
}
