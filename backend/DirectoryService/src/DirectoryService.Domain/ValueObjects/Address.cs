namespace DirectoryService.Domain.ValueObjects;

/// <summary>
/// Адрес локации. Обязательные части — страна, город, улица, здание.
/// Остальное (регион, район, этаж, комната, индекс) — опционально.
/// </summary>
public sealed record Address
{
    private const int MaxFieldLength = 200;

    public string Country { get; }
    public string? Region { get; }
    public string? District { get; }
    public string City { get; }
    public string Street { get; }
    public string Building { get; }
    public string? Floor { get; }
    public string? Room { get; }
    public string? PostalCode { get; }

    public Address(
        string? country,
        string? region,
        string? district,
        string? city,
        string? street,
        string? building,
        string? floor = null,
        string? room = null,
        string? postalCode = null)
    {
        Country = RequireNonEmpty(country, nameof(country));
        City = RequireNonEmpty(city, nameof(city));
        Street = RequireNonEmpty(street, nameof(street));
        Building = RequireNonEmpty(building, nameof(building));

        Region = NormalizeOptional(region, nameof(region));
        District = NormalizeOptional(district, nameof(district));
        Floor = NormalizeOptional(floor, nameof(floor));
        Room = NormalizeOptional(room, nameof(room));
        PostalCode = NormalizeOptional(postalCode, nameof(postalCode));
    }

    private static string RequireNonEmpty(string? value, string paramName)
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");
        }

        var trimmed = value.Trim();

        if (trimmed.Length == 0)
        {
            throw new ArgumentException($"{paramName} cannot be empty or whitespace.", paramName);
        }

        if (trimmed.Length > MaxFieldLength)
        {
            throw new ArgumentException($"{paramName} cannot be longer than {MaxFieldLength} characters.", paramName);
        }

        return trimmed;
    }

    private static string? NormalizeOptional(string? value, string paramName)
    {
        if (value is null)
        {
            return null;
        }

        var trimmed = value.Trim();

        if (trimmed.Length == 0)
        {
            return null;
        }

        if (trimmed.Length > MaxFieldLength)
        {
            throw new ArgumentException($"{paramName} cannot be longer than {MaxFieldLength} characters.", paramName);
        }

        return trimmed;
    }
}
