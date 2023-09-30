using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tcc2.Domain.Entities;
using Tcc2.Domain.Entities.ValueObjects;

namespace Tcc2.Infrastructure.Persistence.RelationalDatabase.Mappings;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired();
        builder
            .HasOne(x => x.Address)
            .WithOne(x => x.Person)
            .HasForeignKey<Address>(x => x.PersonId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
