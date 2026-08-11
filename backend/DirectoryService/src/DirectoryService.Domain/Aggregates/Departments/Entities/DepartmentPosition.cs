namespace DirectoryService.Domain.Aggregates.Departments.Entities;

/// <summary>
/// Связь подразделения с должностью (многие-ко-многим).
/// Описывает, какие должности доступны внутри подразделения.
/// </summary>
public sealed class DepartmentPosition
{
    public DepartmentPosition(Guid id, Guid departmentId, Guid positionId)
    {
        Id = RequireNonEmpty(id, nameof(id));
        DepartmentId = RequireNonEmpty(departmentId, nameof(departmentId));
        PositionId = RequireNonEmpty(positionId, nameof(positionId));
    }

    public Guid Id { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid PositionId { get; private set; }

    private static Guid RequireNonEmpty(Guid value, string paramName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException($"{paramName} cannot be Guid.Empty.", paramName);
        }

        return value;
    }
}
