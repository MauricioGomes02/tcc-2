using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tcc2.Domain.Entities;

namespace Tcc2.Infrastructure.Persistence.RelationalDatabase.Mappings;

public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(x => x.Start).IsRequired();
        builder.Property(x => x.End).IsRequired();
        builder.Property(x => x.PersonId).IsRequired();
        builder.OwnsOne(x => x.Address, a =>
        {
            a.Property(x => x.Country).IsRequired();
            a.Property(x => x.State).IsRequired();
            a.Property(x => x.Neighborhood).IsRequired();
            a.Property(x => x.City).IsRequired();
            a.Property(x => x.Street).IsRequired();
            a.Property(x => x.Number).IsRequired();
            a.Property(x => x.PostalCode).IsRequired();
        });

        builder.Navigation(x => x.ActivityDay).AutoInclude();
    }
}
