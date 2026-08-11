using System.Text.RegularExpressions;

namespace DirectoryService.Domain.ValueObjects;

/// <summary>
/// Короткий стабильный код сущности для URL, интеграций и дерева.
/// Не равен отображаемому названию: name можно переименовать, slug лучше не менять.
/// Формат: латиница в нижнем регистре и цифры, разделённые одинарными дефисами.
/// Пример: "sales", "b2b", "it-backend".
/// </summary>
public sealed record Slug
{
    private const int MaxLength = 100;

    // Compiled + NonBacktracking: быстрый и устойчивый к ReDoS (анализаторы MA0009/S6444).
    // ExplicitCapture: группа нужна только для группировки, без захвата (MA0023).
    private static readonly Regex Pattern = new(
        @"^[a-z0-9]+(?:-[a-z0-9]+)*$",
        RegexOptions.Compiled | RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture,
        TimeSpan.FromMilliseconds(100));

    public string Value { get; }

    public Slug(string? value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value), "Slug cannot be null.");
        }

        var trimmed = value.Trim();

        if (trimmed.Length == 0)
        {
            throw new ArgumentException("Slug cannot be empty or whitespace.", nameof(value));
        }

        if (trimmed.Length > MaxLength)
        {
            throw new ArgumentException($"Slug cannot be longer than {MaxLength} characters.", nameof(value));
        }

        if (!Pattern.IsMatch(trimmed))
        {
            throw new ArgumentException(
                "Slug must contain only lowercase latin letters and digits " +
                "separated by single hyphens (e.g. 'sales', 'it-backend').",
                nameof(value));
        }

        Value = trimmed;
    }
}
