using DirectoryService.Domain.Aggregates.Departments;
using DirectoryService.Domain.Aggregates.Departments.Entities;
using DirectoryService.Domain.Aggregates.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations
{
    public class DepartmentPositionConfiguration: IEntityTypeConfiguration<DepartmentPosition>
    {
        public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
        {
            builder
                .ToTable("department_position")
                .HasKey(dp=>dp.Id)
                .HasName("pk_department_position");

            builder
                .Property(dp=>dp.Id)
                .HasColumnName("id");

            builder
                .HasOne<Department>()
                .WithMany()
                .HasForeignKey(dp => dp.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_department_position_department");

            builder
                .Property(dp => dp.DepartmentId)
                .HasColumnName("department_id");

            builder
                .HasOne<Position>()
                .WithMany()
                .HasForeignKey(dp => dp.PositionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_department_position_position");

            builder
                .Property(dp => dp.PositionId)
                .HasColumnName("position_id");

            builder
                .HasIndex(dp => dp.DepartmentId)
                .HasDatabaseName("ix_department_position_department_id");

            builder
                .HasIndex(dp => dp.PositionId)
                .HasDatabaseName("ix_department_position_position_id");

        }
    }
}