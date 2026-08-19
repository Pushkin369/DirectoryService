using DirectoryService.Domain.Extensions;

namespace DirectoryService.Domain.Aggregates.Departments.Entities;

/// <summary>
/// Связь подразделения с должностью (многие-ко-многим).
/// Описывает, какие должности доступны внутри подразделения.
/// </summary>
public sealed class DepartmentPosition
{
    private DepartmentPosition() { }   // для EF Core; доменный конструктор остаётся как есть
    public DepartmentPosition( Guid departmentId, Guid positionId)
    {
        Id = Guid.CreateVersion7();
        DepartmentId = departmentId.EnsureNotEmpty(nameof(departmentId));
        PositionId = positionId.EnsureNotEmpty(nameof(positionId));
    }

    public Guid Id { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid PositionId { get; private set; }
}
