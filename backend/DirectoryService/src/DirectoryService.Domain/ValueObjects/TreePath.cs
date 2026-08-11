using System.Text.RegularExpressions;

namespace DirectoryService.Domain.ValueObjects;

/// <summary>
/// Полный путь подразделения в дереве, собранный из slug родителей и самого подразделения.
/// Нужен для быстрого поиска ветки оргструктуры, breadcrumbs и дерева в UI.
/// Пример: "company/sales/b2b".
///
/// Строится ТОЛЬКО через фабрики <see cref="Root"/> и <see cref="Child"/>,
/// чтобы корневое и дочернее подразделения получали путь по единому правилу.
/// </summary>
public sealed record TreePath
{
    // Compiled + NonBacktracking: быстрый и устойчивый к ReDoS (анализаторы MA0009/S6444).
    // Non-capturing group (?:...) — группа нужна только для группировки (MA0023).
    private static readonly Regex SegmentPattern = new(
        @"^[a-z0-9]+(?:-[a-z0-9]+)*$",
        RegexOptions.Compiled | RegexOptions.NonBacktracking,
        TimeSpan.FromMilliseconds(100));

    public string Value { get; }

    private TreePath(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Путь корневого подразделения: совпадает с его slug.
    /// Например: <c>TreePath.Root(new Slug("company"))</c> → "company".
    /// </summary>
    public static TreePath Root(Slug slug)
    {
        ArgumentNullException.ThrowIfNull(slug);
        return new TreePath(slug.Value);
    }

    /// <summary>
    /// Путь дочернего подразделения: путь родителя + "/" + slug текущего.
    /// Например: <c>TreePath.Child(parentPath, new Slug("b2b"))</c> → "company/sales/b2b".
    /// </summary>
    public static TreePath Child(TreePath parentPath, Slug slug)
    {
        ArgumentNullException.ThrowIfNull(parentPath);
        ArgumentNullException.ThrowIfNull(slug);
        return new TreePath($"{parentPath.Value}/{slug.Value}");
    }

    /// <summary>
    /// Восстановить путь из строки (например, при чтении из БД).
    /// Строка должна состоять из валидных slug-сегментов, разделённых "/".
    /// </summary>
    public static TreePath Restore(string? value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value), "TreePath cannot be null.");
        }

        var trimmed = value.Trim();

        if (trimmed.Length == 0)
        {
            throw new ArgumentException("TreePath cannot be empty or whitespace.", nameof(value));
        }

        // Запрещаем ведущий/завершающий слэш и пустые сегменты "//".
        if (trimmed.StartsWith('/')
            || trimmed.EndsWith('/')
            || trimmed.Contains("//", StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "TreePath must not start or end with '/' and must not contain empty segments.",
                nameof(value));
        }

        var invalidSegment = trimmed.Split('/')
            .FirstOrDefault(s => !SegmentPattern.IsMatch(s));

        if (invalidSegment is not null)
        {
            throw new ArgumentException(
                $"TreePath segment '{invalidSegment}' is not a valid slug.",
                nameof(value));
        }

        return new TreePath(trimmed);
    }
}
