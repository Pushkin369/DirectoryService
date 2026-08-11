namespace DirectoryService.Domain.ValueObjects;

/// <summary>
/// Отображаемое название сущности (Department, Location, Position).
/// Можно переименовать, поэтому это не slug.
/// </summary>
public sealed record Name
{
    private const int MaxLength = 200;

    public string Value { get; }

    public Name(string? value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value), "Name cannot be null.");
        }

        var trimmed = value.Trim();

        if (trimmed.Length == 0)
        {
            throw new ArgumentException("Name cannot be empty or whitespace.", nameof(value));
        }

        if (trimmed.Length > MaxLength)
        {
            throw new ArgumentException($"Name cannot be longer than {MaxLength} characters.", nameof(value));
        }

        Value = trimmed;
    }
}
