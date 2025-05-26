using System.Diagnostics.CodeAnalysis;

namespace Repetify.Infrastructure.Persistence.EfCore.Entities;

/// <summary>
/// Represents a deck entity in the database.
/// </summary>
[SuppressMessage("Design", "CA2227:Prefer readonly fields for collections", Justification = "Required for EF Core.")]
public class DeckEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    public required string OriginalLanguage { get; set; }
    public required string TranslatedLanguage { get; set; }
    public ICollection<CardEntity> Cards { get; set; } = new List<CardEntity>();
}
