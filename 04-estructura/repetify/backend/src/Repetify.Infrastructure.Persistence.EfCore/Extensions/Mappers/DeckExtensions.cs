using Repetify.Domain.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Entities;

namespace Repetify.Infrastructure.Persistence.EfCore.Extensions.Mappers;

/// <summary>
/// Provides methods to map between Deck domain objects and DeckEntity data objects.
/// </summary>
public static class DeckExtensions
{
	/// <summary>
	/// Maps a Deck domain model to a DeckEntity.
	/// </summary>
	/// <param name="deckDomain">The Deck domain model.</param>
	/// <returns>The corresponding DeckEntity.</returns>
	public static DeckEntity ToDataEntity(this Deck deckDomain)
	{
		ArgumentNullException.ThrowIfNull(deckDomain);

		return new DeckEntity
		{
			Id = deckDomain.Id,
			Name = deckDomain.Name,
			Description = deckDomain.Description,
			UserId = deckDomain.UserId,
			OriginalLanguage = deckDomain.OriginalLanguage,
			TranslatedLanguage = deckDomain.TranslatedLanguage
		};
	}

	/// <summary>
	/// Maps a DeckEntity to a Deck domain model.
	/// </summary>
	/// <param name="deckEntity">The DeckEntity.</param>
	/// <returns>The corresponding Deck domain model, or null if the entity is null.</returns>
	public static Deck ToDomain(this DeckEntity deckEntity)
	{
		ArgumentNullException.ThrowIfNull(deckEntity);

		return new Deck(
			id: deckEntity.Id,
			name: deckEntity.Name,
			description: deckEntity.Description,
			userId: deckEntity.UserId,
			originalLanguage: deckEntity.OriginalLanguage,
			translatedLanguage: deckEntity.TranslatedLanguage
		);
	}

	/// <summary>  
	/// Updates the properties of a DeckEntity from a Deck domain model.  
	/// </summary>  
	/// <param name="deckEntity">The DeckEntity to update.</param>  
	/// <param name="deck">The Deck domain model with updated values.</param>  
	public static void UpdateFromDomain(this DeckEntity deckEntity, Deck deck)
	{
		ArgumentNullException.ThrowIfNull(deckEntity);
		ArgumentNullException.ThrowIfNull(deck);

		deckEntity.Name = deck.Name;
		deckEntity.Description = deck.Description;
		deckEntity.UserId = deck.UserId;
		deckEntity.OriginalLanguage = deck.OriginalLanguage;
		deckEntity.TranslatedLanguage = deck.TranslatedLanguage;
	}
}
