using DirectoryService.Domain.Aggregates.Departments;
using DirectoryService.Domain.Aggregates.Departments.Entities;
using DirectoryService.Domain.Aggregates.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations
{
    public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
    {
        public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
        {
            builder
                .ToTable("department_location")
                .HasKey(dl=>dl.Id)
                .HasName("pk_department_location");

            builder
                .Property(dl=>dl.Id)
                .HasColumnName("id");

            builder
                .HasOne<Department>()
                .WithMany()
                .HasForeignKey(dl => dl.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_department_location_department");

            builder
                .Property(dl => dl.DepartmentId)
                .HasColumnName("department_id");

            builder
                .HasOne<Location>()
                .WithMany()
                .HasForeignKey(dl => dl.LocationId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_department_location_location");

            builder
                .Property(dl => dl.LocationId)
                .HasColumnName("location_id");

            builder
                .HasIndex(dl => dl.DepartmentId)
                .HasDatabaseName("ix_department_location_department_id");

            builder
                .HasIndex(dl => dl.LocationId)
                .HasDatabaseName("ix_department_location_location_id");

            builder
                .Property(dl=>dl.IsPrimary)
                .HasColumnName("is_primary")
                .IsRequired(true);

        }
    }
}