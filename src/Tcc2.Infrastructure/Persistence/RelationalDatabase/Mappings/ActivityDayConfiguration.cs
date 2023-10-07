using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tcc2.Domain.Entities;

namespace Tcc2.Infrastructure.Persistence.RelationalDatabase.Mappings;

public class ActivityDayConfiguration : IEntityTypeConfiguration<ActivityDay>
{
    public void Configure(EntityTypeBuilder<ActivityDay> builder)
    {
        builder.HasKey(x => new { x.ActivityId, x.DayId });
        builder.Property(x => x.ActivityId).IsRequired();
        builder.Property(x => x.DayId).IsRequired();

        builder
            .HasOne(x => x.Activity)
            .WithMany(x => x.ActivityDay)
            .HasForeignKey(x => x.ActivityId);

        builder
            .HasOne(x => x.Day)
            .WithMany(x => x.ActivityDay)
            .HasForeignKey(x => x.DayId);

        builder.Navigation(x => x.Activity).AutoInclude();
        builder.Navigation(x => x.Day).AutoInclude();
    }
}
