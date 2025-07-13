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
			Front = cardDomain.Front,
			Back = cardDomain.Back,
			CorrectReviewStreak = cardDomain.CorrectReviewStreak,
			Repetitions = cardDomain.Repetitions,
			Interval = cardDomain.Interval,
			EaseFactor = cardDomain.EaseFactor,
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
			front: cardEntity.Front,
			back: cardEntity.Back,
			correctReviewStreak: cardEntity.CorrectReviewStreak,
			repetitions: cardEntity.Repetitions,
			easeFactor: cardEntity.EaseFactor,
			interval: cardEntity.Interval,
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

		cardEntity.Front = card.Front;
		cardEntity.Back = card.Back;
		cardEntity.CorrectReviewStreak = card.CorrectReviewStreak;
		cardEntity.NextReviewDate = card.NextReviewDate;
		cardEntity.PreviousCorrectReview = card.PreviousCorrectReview;
	}
}
