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
	/// Gets or sets the front side of the card.
	/// </summary>
	public required string Front { get; set; }

	/// <summary>
	/// Gets or sets the back side of the card.
	/// </summary>
	public required string Back { get; set; }

	/// <summary>
	/// Gets or sets the number of consecutive correct reviews for the card.
	/// </summary>
	public int CorrectReviewStreak { get; set; }

	/// <summary>
	/// The number of consecutive successful reviews with a quality rating of 3 or higher,
	/// as defined by the SM-2 spaced repetition algorithm. This value is used to determine
	/// the interval until the next review. It resets to 0 when the quality is less than 3.
	/// </summary>
	public int Repetitions { get; set; }

	/// <summary>
	/// The ease factor (EF) used by the SM-2 spaced repetition algorithm to determine 
	/// how the review interval grows after each successful review. 
	/// A higher value results in longer intervals between reviews. 
	/// The EF is adjusted after each review based on the quality of the response.
	/// </summary>
	public double EaseFactor { get; set; }

	/// <summary>
	/// The number of days between the last successful review and the next one,
	/// as determined by the SM-2 algorithm. This interval increases or resets 
	/// based on the quality of the user's responses.
	/// </summary>
	public int Interval { get; set; }

	/// <summary>
	/// Gets or sets the date when the card is next due for review.
	/// </summary>
	public DateTime NextReviewDate { get; set; }

	/// <summary>
	/// Gets or sets the date of the previous correct review for the card.
	/// </summary>
	public DateTime PreviousCorrectReview { get; set; }
}
