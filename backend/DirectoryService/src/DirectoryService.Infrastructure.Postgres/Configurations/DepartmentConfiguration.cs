using DirectoryService.Domain.Aggregates.Departments;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder
                .ToTable("department")
                .HasKey(d=>d.Id)
                .HasName("pk_department");

            builder
                .Property(d=>d.Id)
                .HasColumnName("id");

            builder.HasOne<Department>()
                .WithMany()
                .HasForeignKey(d=>d.ParentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_department_parent");

            builder
                .HasIndex(d => d.ParentId)
                .HasDatabaseName("ix_department_parent_id");

            builder
                .Property(d=>d.ParentId)
                .HasColumnName("parent_id")
                .IsRequired(false);

            builder
                .Property(d=>d.Name)
                .HasColumnName("name")
                .HasConversion(ValueObjectConverters.NameToString, ValueObjectConverters.NameComparer)
                .IsRequired()
                .HasMaxLength(Name.MaxLength);

            builder
                .Property(d=>d.Slug)
                .HasColumnName("slug")
                .HasConversion(ValueObjectConverters.SlugToString, ValueObjectConverters.SlugComparer)
                .IsRequired()
                .HasMaxLength(Slug.MaxLength);

            builder
                .Property(d=>d.TreePath)
                .HasColumnName("tree_path")
                .HasConversion(ValueObjectConverters.TreePathToString, ValueObjectConverters.TreePathComparer)
                .IsRequired()
                .HasMaxLength(TreePath.MaxLength);

            builder
                .HasIndex(d => d.TreePath)
                .IsUnique()
                .HasDatabaseName("ix_department_tree_path");

            builder
                .Property(d=>d.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder
                .Property(d=>d.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

        }
    }
}