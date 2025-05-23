using Repetify.Application.Dtos;
using Repetify.Domain.Entities;

namespace Repetify.Application.Extensions.Mappings;

/// <summary>
/// Provides extension methods for mapping Deck domain entities to DeckDto objects and vice versa.
/// </summary>
public static class DeckExtensions
{
	/// <summary>
	/// Converts a Deck domain entity to a DeckDto.
	/// </summary>
	/// <param name="deck">The Deck domain entity to convert.</param>
	/// <returns>A DeckDto representing the Deck domain entity.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the deck is null.</exception>
	public static DeckDto ToDto(this Deck deck)
	{
		ArgumentNullException.ThrowIfNull(deck);

		return new DeckDto(
			id: deck.Id,
			name: deck.Name,
			description: deck.Description,
			userId: deck.UserId,
			originalLanguage: deck.OriginalLanguage,
			translatedLanguage: deck.TranslatedLanguage
		);
	}

	/// <summary>
	/// Converts a collection of Deck domain entities to a collection of DeckDto objects.
	/// </summary>
	/// <param name="decks">The collection of Deck domain entities to convert.</param>
	/// <returns>A collection of DeckDto objects representing the Deck entities.</returns>
	public static IEnumerable<DeckDto> ToDtoList(this IEnumerable<Deck> decks)
	{
		return decks is null ? throw new ArgumentNullException(nameof(decks)) : decks.Select(deck => deck.ToDto());
	}

	/// <summary>
	/// Converts a DeckDto to a Deck domain entity.
	/// </summary>
	/// <param name="deckDto">The DeckDto to convert.</param>
	/// <returns>A Deck domain entity representing the DeckDto.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the deckDto is null.</exception>
	public static Deck ToEntity(this DeckDto deckDto)
	{
		ArgumentNullException.ThrowIfNull(deckDto);

		return new Deck(
			name: deckDto.Name,
			description: deckDto.Description,
			userId: deckDto.UserId,
			originalLanguage: deckDto.OriginalLanguage,
			translatedLanguage: deckDto.TranslatedLanguage
		);
	}

	/// <summary>
	/// Converts a AddOrUpdateDeckDto to a Deck domain entity.
	/// </summary>
	/// <param name="deckDto">The DeckDto to convert.</param>
	/// <param name="userId">The ID of the user associated with the deck.</param>
	/// <returns>A Deck domain entity representing the DeckDto.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the deckDto is null.</exception>
	public static Deck ToEntity(this AddOrUpdateDeckDto deckDto, Guid userId)
	{
		ArgumentNullException.ThrowIfNull(deckDto);

		return new Deck(
			name: deckDto.Name!,
			description: deckDto.Description,
			userId: userId,
			originalLanguage: deckDto.OriginalLanguage!,
			translatedLanguage: deckDto.TranslatedLanguage!
		);
	}
}
