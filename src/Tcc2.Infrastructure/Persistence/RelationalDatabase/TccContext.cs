using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Tcc2.Domain.Entities;
using Tcc2.Domain.Entities.ValueObjects;

namespace Tcc2.Infrastructure.Persistence.RelationalDatabase;

public class TccContext : DbContext
{
    public TccContext(DbContextOptions<TccContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly(),
            type => type.GetInterfaces().Any(@interface =>
                @interface.IsGenericType
                && @interface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));
    }

    public DbSet<Person> People { get; set; }
    public DbSet<Address> Addresses { get; set; }
}
