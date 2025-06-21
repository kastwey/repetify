namespace Repetify.Infrastructure.Persistence.EfCore.Entities;

/// <summary>
/// Represents a card entity in the database.
/// </summary>
public class CardEntity
{
	/// <summary>
	/// Gets or sets the unique identifier for the card.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the unique identifier for the deck to which the card belongs.
	/// </summary>
	public Guid DeckId { get; set; }

	/// <summary>
	/// Gets or sets the deck to which the card belongs.
	/// </summary>
	public DeckEntity? Deck { get; set; }

	/// <summary>
	/// Gets or sets the original word on the card.
	/// </summary>
	public required string OriginalWord { get; set; }

	/// <summary>
	/// Gets or sets the translated word on the card.
	/// </summary>
	public required string TranslatedWord { get; set; }

	/// <summary>
	/// Gets or sets the number of consecutive correct reviews for the card.
	/// </summary>
	public int CorrectReviewStreak { get; set; }

	/// <summary>
	/// Gets or sets the date when the card is next due for review.
	/// </summary>
	public DateTime NextReviewDate { get; set; }

	/// <summary>
	/// Gets or sets the date of the previous correct review for the card.
	/// </summary>
	public DateTime PreviousCorrectReview { get; set; }
}
