namespace DirectoryService.Domain.Aggregates.Departments.Entities;

/// <summary>
/// Связь подразделения с локацией (многие-ко-многим).
/// <see cref="IsPrimary"/> отличает основной офис подразделения от дополнительных мест работы.
/// </summary>
public sealed class DepartmentLocation
{
    public DepartmentLocation(Guid id, Guid departmentId, Guid locationId, bool isPrimary = false)
    {
        Id = RequireNonEmpty(id, nameof(id));
        DepartmentId = RequireNonEmpty(departmentId, nameof(departmentId));
        LocationId = RequireNonEmpty(locationId, nameof(locationId));
        IsPrimary = isPrimary;
    }

    public Guid Id { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid LocationId { get; private set; }
    public bool IsPrimary { get; private set; }

    private static Guid RequireNonEmpty(Guid value, string paramName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException($"{paramName} cannot be Guid.Empty.", paramName);
        }

        return value;
    }
}
