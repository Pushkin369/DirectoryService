using DirectoryService.Domain.Extensions;

namespace DirectoryService.Domain.Aggregates.Departments.Entities;

/// <summary>
/// Связь подразделения с локацией (многие-ко-многим).
/// <see cref="IsPrimary"/> отличает основной офис подразделения от дополнительных мест работы.
/// </summary>
public sealed class DepartmentLocation
{
    private DepartmentLocation() { }   // для EF Core; доменный конструктор остаётся как есть
    public DepartmentLocation(Guid departmentId, Guid locationId, bool isPrimary = false)
    {
        Id = Guid.CreateVersion7();
        DepartmentId = departmentId.EnsureNotEmpty(nameof(departmentId));
        LocationId = locationId.EnsureNotEmpty(nameof(locationId));
        IsPrimary = isPrimary;
    }

    public Guid Id { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid LocationId { get; private set; }
    public bool IsPrimary { get; private set; }
}
