using System.ComponentModel.DataAnnotations;

namespace Repetify.Application.Dtos;

/// <summary>
/// Data Transfer Object for a Card.
/// </summary>
public class CardDto
{
	/// <summary>
	/// Gets or sets the unique identifier for the card.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the unique identifier for the deck.
	/// </summary>
	public Guid DeckId { get; set; }

	/// <summary>
	/// Gets or sets the original word.
	/// </summary>
	public string OriginalWord { get; set; }

	/// <summary>
	/// Gets or sets the translated word.
	/// </summary>
	public string TranslatedWord { get; set; }

	/// <summary>
	/// Gets or sets the correct review streak count.
	/// </summary>
	public int CorrectReviewStreak { get; set; }

	/// <summary>
	/// Gets or sets the date for the next review.
	/// </summary>
	public DateTime NextReviewDate { get; set; }

	/// <summary>
	/// Gets or sets the date of the previous correct review.
	/// </summary>
	public DateTime PreviousCorrectReview { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="CardDto"/> class.
	/// </summary>
	/// <param name="id">The unique identifier for the card.</param>
	/// <param name="deckId">The unique identifier for the deck.</param>
	/// <param name="originalWord">The original word.</param>
	/// <param name="translatedWord">The translated word.</param>
	/// <param name="correctReviewStreak">The correct review streak count.</param>
	/// <param name="nextReviewDate">The date for the next review.</param>
	/// <param name="previousCorrectReview">The date of the previous correct review.</param>
	/// <exception cref="ArgumentException">Thrown when originalWord or translatedWord is null or whitespace.</exception>
	public CardDto(Guid id, Guid deckId, string originalWord, string translatedWord, int correctReviewStreak, DateTime nextReviewDate, DateTime previousCorrectReview)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(originalWord);
		ArgumentException.ThrowIfNullOrWhiteSpace(translatedWord);

		Id = id;
		DeckId = deckId;
		OriginalWord = originalWord;
		TranslatedWord = translatedWord;
		CorrectReviewStreak = correctReviewStreak;
		NextReviewDate = nextReviewDate;
		PreviousCorrectReview = previousCorrectReview;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CardDto"/> class.
	/// </summary>
	/// <param name="id">The unique identifier for the card.</param>
	/// <param name="deckId">The unique identifier for the deck.</param>
	/// <param name="originalWord">The original word.</param>
	/// <param name="translatedWord">The translated word.</param>
	/// <exception cref="ArgumentException">Thrown when originalWord or translatedWord is null or whitespace.</exception>
	public CardDto(Guid id, Guid deckId, string originalWord, string translatedWord) : this(id, deckId, originalWord, translatedWord, 0, default(DateTime), default(DateTime))
	{
	}
}