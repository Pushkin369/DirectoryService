using DirectoryService.Domain.Aggregates.Locations;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder
                .ToTable("location")
                .HasKey(l=>l.Id)
                .HasName("pk_location");

            builder
                .Property(l=>l.Id)
                .HasColumnName("id");

            builder
                .Property(l=>l.Name)
                .HasColumnName("name")
                .HasConversion(ValueObjectConverters.NameToString, ValueObjectConverters.NameComparer)
                .IsRequired()
                .HasMaxLength(Name.MaxLength);


            builder.ComplexProperty(l=>l.Address, address=>
            {
                address
                    .Property(adr=>adr.Country)
                    .HasColumnName("country")
                    .IsRequired(true)
                    .HasMaxLength(Address.MaxFieldLength);
                
                address
                    .Property(adr=>adr.Region)
                    .HasColumnName("region")
                    .IsRequired(false)
                    .HasMaxLength(Address.MaxFieldLength);

                address
                    .Property(adr=>adr.District)
                    .HasColumnName("district")
                    .IsRequired(false)
                    .HasMaxLength(Address.MaxFieldLength);
                
                address
                    .Property(adr=>adr.City)
                    .HasColumnName("city")
                    .IsRequired(true)
                    .HasMaxLength(Address.MaxFieldLength);
                
                address
                    .Property(adr=>adr.Street)
                    .HasColumnName("street")
                    .IsRequired(true)
                    .HasMaxLength(Address.MaxFieldLength);
                
                address
                    .Property(adr=>adr.Building)
                    .HasColumnName("building")
                    .IsRequired(true)
                    .HasMaxLength(Address.MaxFieldLength);

                address
                    .Property(adr=>adr.Floor)
                    .HasColumnName("floor")
                    .IsRequired(false)
                    .HasMaxLength(Address.MaxFieldLength);
                
                address
                    .Property(adr=>adr.Room)
                    .HasColumnName("room")
                    .IsRequired(false)
                    .HasMaxLength(Address.MaxFieldLength);

                address
                    .Property(adr=>adr.PostalCode)
                    .HasColumnName("postal_code")
                    .IsRequired(false)
                    .HasMaxLength(Address.MaxFieldLength); 

            });
            
            
            builder
                .Property(l=>l.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder
                .Property(l=>l.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();
        }
    }
}