using System.Diagnostics.CodeAnalysis;

namespace Repetify.Infrastructure.Persistence.EfCore.Entities;

/// <summary>
/// Represents a deck entity in the database.
/// </summary>
[SuppressMessage("Design", "CA2227:Prefer readonly fields for collections", Justification = "Required for EF Core.")]
public class DeckEntity
{
	/// <summary>
	/// Gets or sets the unique identifier for the deck.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the name of the deck.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Gets or sets the description of the deck.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Gets or sets the unique identifier of the user who owns the deck.
	/// </summary>
	public Guid UserId { get; set; }

	/// <summary>
	///  Gets or sets the user who owns the deck.
	/// </summary>
	public UserEntity? User { get; set; }

	/// <summary>
	/// Gets or sets the original language of the deck.
	/// </summary>
	public required string OriginalLanguage { get; set; }

	/// <summary>
	/// Gets or sets the translated language of the deck.
	/// </summary>
	public required string TranslatedLanguage { get; set; }

	/// <summary>
	/// Gets or sets the collection of cards in the deck.
	/// </summary>	
	public ICollection<CardEntity> Cards { get; set; } = new List<CardEntity>();
}