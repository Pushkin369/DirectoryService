using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DirectoryService.Infrastructure.Postgres.Configurations
{
    /// <summary>
    /// Переиспользуемые пары «конвертер + компаратор» для value object'ов домена.
    /// Конвертер сворачивает VO в строку колонки и разворачивает обратно через
    /// конструктор/фабрику домена (валидация при чтении из БД).
    /// Comparer обязателен: без него EF сравнивает объекты по ссылке, не видит
    /// изменений свойства и молча не генерирует UPDATE по этой колонке.
    /// </summary>
    public static class ValueObjectConverters
    {
        public static readonly ValueConverter<Name, string> NameToString = new(
            name => name.Value,
            value => new Name(value));

        public static readonly ValueComparer<Name> NameComparer = new(
            (left, right) => left!.Value == right!.Value,
            value => StringComparer.Ordinal.GetHashCode(value.Value),
            value => new Name(value.Value));

        public static readonly ValueConverter<Slug, string> SlugToString = new(
            slug => slug.Value,
            value => new Slug(value));

        public static readonly ValueComparer<Slug> SlugComparer = new(
            (left, right) => left!.Value == right!.Value,
            value => StringComparer.Ordinal.GetHashCode(value.Value),
            value => new Slug(value.Value));

        public static readonly ValueConverter<TreePath, string> TreePathToString = new(
            treePath => treePath.Value,
            value => TreePath.Restore(value));

        public static readonly ValueComparer<TreePath> TreePathComparer = new(
            (left, right) => left!.Value == right!.Value,
            value => StringComparer.Ordinal.GetHashCode(value.Value),
            value => TreePath.Restore(value.Value));
    }
}
