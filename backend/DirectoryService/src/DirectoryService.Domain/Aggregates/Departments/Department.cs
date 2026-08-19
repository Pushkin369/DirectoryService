using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Aggregates.Departments;

/// <summary>
/// Подразделение компании. Образует иерархию через <see cref="ParentId"/>.
/// Путь <see cref="TreePath"/> — стабильный путь в дереве для поиска, breadcrumbs и UI.
/// </summary>
public sealed class Department
{
    // для EF Core; доменный конструктор остаётся как есть
    private Department()
    {
        Name = null!;
        Slug = null!;
        TreePath = null!;
    }   
    
    public Department( Name name, Slug slug, TreePath treePath, Guid? parentId = null)
    {
        Id = Guid.CreateVersion7();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Slug = slug ?? throw new ArgumentNullException(nameof(slug));
        TreePath = treePath ?? throw new ArgumentNullException(nameof(treePath));

        if (parentId.HasValue && parentId.Value == Guid.Empty)
        {
            throw new ArgumentException("ParentId cannot be Guid.Empty. Use null for root department.", nameof(parentId));
        }

        ParentId = parentId;

        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid? ParentId { get; private set; }
    public Name Name { get; private set; }
    public Slug Slug { get; private set; }
    public TreePath TreePath { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
}
