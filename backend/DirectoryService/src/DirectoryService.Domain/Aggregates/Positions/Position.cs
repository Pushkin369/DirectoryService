using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Aggregates.Positions;

/// <summary>
/// Должность — роль, доступная внутри подразделений.
/// Переиспользуется между подразделениями через <see cref="Aggregates.Departments.Entities.DepartmentPosition"/>.
/// </summary>
public sealed class Position
{
    public Position(Guid id, Name name)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));

        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public Name Name { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
}
