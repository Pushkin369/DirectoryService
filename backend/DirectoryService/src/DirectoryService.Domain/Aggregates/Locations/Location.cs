using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Aggregates.Locations;

/// <summary>
/// Локация — место, где работает подразделение (адрес офиса/точки).
/// </summary>
public sealed class Location
{
    // для EF Core; доменный конструктор остаётся как есть
    private Location()
    {
        Name = null!;
        Address = null!;
    }   
    public Location(Name name, Address address)
    {
        Id = Guid.CreateVersion7();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address ?? throw new ArgumentNullException(nameof(address));

        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public Name Name { get; private set; }
    public Address Address { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
}
