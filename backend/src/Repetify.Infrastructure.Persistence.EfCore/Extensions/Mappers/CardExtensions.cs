using Repetify.Crosscutting;
using Repetify.Domain.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Entities;

namespace Repetify.Infrastructure.Persistence.EfCore.Extensions.Mappers;

/// <summary>
/// Provides methods to map between Card domain objects and CardEntity data objects.
/// </summary>
public static class CardExtensions
{
	/// <summary>
	/// Maps a Card domain object to a CardEntity data object.
	/// </summary>
	/// <param name="cardDomain">The Card domain object to map.</param>
	/// <param name="deckId">The ID of the deck to associate with the card.</param>
	/// <returns>A CardEntity data object.</returns>
	public static CardEntity ToDataEntity(this Card cardDomain)
	{
		ArgumentNullException.ThrowIfNull(cardDomain);

		return new CardEntity
		{
			Id = cardDomain.Id,
			DeckId = cardDomain.DeckId,
			OriginalWord = cardDomain.OriginalWord,
			TranslatedWord = cardDomain.TranslatedWord,
			CorrectReviewStreak = cardDomain.CorrectReviewStreak,
			NextReviewDate = cardDomain.NextReviewDate,
			PreviousCorrectReview = cardDomain.PreviousCorrectReview
		};
	}

	/// <summary>
	/// Maps a CardEntity data object to a Card domain object.
	/// </summary>
	/// <param name="cardEntity">The CardEntity data object to map.</param>
	/// <returns>A Card domain object.</returns>
	public static Result<Card> ToDomain(this CardEntity cardEntity)
	{
		ArgumentNullException.ThrowIfNull(cardEntity);

		return Card.RehidrateFromPersistence(
			id: cardEntity.Id,
			deckId: cardEntity.DeckId,
			originalWord: cardEntity.OriginalWord,
			translatedWord: cardEntity.TranslatedWord,
			correctReviewStreak: cardEntity.CorrectReviewStreak,
			nextReviewDate: cardEntity.NextReviewDate,
			previousCorrectReview: cardEntity.PreviousCorrectReview
		);
	}

	/// <summary>  
	/// Updates a CardEntity data object with values from a Card domain object.  
	/// </summary>  
	/// <param name="cardEntity">The CardEntity data object to update.</param>  
	/// <param name="card">The Card domain object containing updated values.</param>  
	public static void UpdateFromDomain(this CardEntity cardEntity, Card card)
	{
		ArgumentNullException.ThrowIfNull(cardEntity);
		ArgumentNullException.ThrowIfNull(card);

		cardEntity.OriginalWord = card.OriginalWord;
		cardEntity.TranslatedWord = card.TranslatedWord;
		cardEntity.CorrectReviewStreak = card.CorrectReviewStreak;
		cardEntity.NextReviewDate = card.NextReviewDate;
		cardEntity.PreviousCorrectReview = card.PreviousCorrectReview;
	}
}
