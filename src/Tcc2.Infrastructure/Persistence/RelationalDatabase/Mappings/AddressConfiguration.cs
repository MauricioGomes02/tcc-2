using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tcc2.Domain.Entities.ValueObjects;

namespace Tcc2.Infrastructure.Persistence.RelationalDatabase.Mappings;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(x => x.Street).IsRequired();
        builder.Property(x => x.Neighborhood).IsRequired();
        builder.Property(x => x.City).IsRequired();
        builder.Property(x => x.State).IsRequired();
        builder.Property(x => x.Country).IsRequired();
        builder.Property(x => x.PostalCode).IsRequired(false);

        builder.OwnsOne(x => x.GeographicCoordinate);
    }
}
