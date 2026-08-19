using DirectoryService.Domain.Aggregates.Positions;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations
{
    public class PositionConfiguration: IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder
                .ToTable("position")
                .HasKey(p=>p.Id)
                .HasName("pk_position");

            builder
                .Property(p=>p.Id)
                .HasColumnName("id");

            builder
                .Property(p=>p.Name)
                .HasColumnName("name")
                .HasConversion(ValueObjectConverters.NameToString, ValueObjectConverters.NameComparer)
                .IsRequired()
                .HasMaxLength(Name.MaxLength);

            builder
                .Property(p=>p.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder
                .Property(p=>p.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();
        }
    }
}