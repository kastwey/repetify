using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repetify.Domain.Entities;

/// <summary>
/// Represents a card entity used for language learning, containing information about the original and translated words, review streaks, and review dates.
/// </summary>
public class Card
{
	/// <summary>
	/// Gets the unique identifier of the card.
	/// </summary>
	public Guid Id { get; private set; }

	/// <summary>
	/// Gets the unique identifier of the deck to which the card belongs.
	/// </summary>
	public Guid DeckId { get; private set; }

	/// <summary>
	/// Gets the original word on the card.
	/// </summary>
	public string OriginalWord { get; private set; }

	/// <summary>
	/// Gets the translated word on the card.
	/// </summary>
	public string TranslatedWord { get; private set; }

	/// <summary>
	/// Gets the number of consecutive correct reviews for the card.
	/// </summary>
	public int CorrectReviewStreak { get; private set; }

	/// <summary>
	/// Gets the date and time when the card is next due for review.
	/// </summary>
	public DateTime NextReviewDate { get; private set; }

	/// <summary>
	/// Gets the date and time of the last correct review for the card.
	/// </summary>
	public DateTime PreviousCorrectReview { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Card"/> class with the specified parameters.
	/// </summary>
	/// <param name="id">The unique identifier of the card. If null, a new GUID will be generated.</param>
	/// <param name="deckId">The unique identifier of the deck to which the card belongs.</param>
	/// <param name="originalWord">The original word on the card.</param>
	/// <param name="translatedWord">The translated word on the card.</param>
	public Card(Guid? id, Guid deckId, string originalWord, string translatedWord)
		: this(id ?? Guid.NewGuid(), deckId, originalWord, translatedWord, correctReviewStreak: 0, nextReviewDate: DateTime.UtcNow.AddDays(1), previousCorrectReview: DateTime.MinValue)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Card"/> class with the specified parameters.
	/// </summary>
	/// <param name="deckId">The unique identifier of the deck to which the card belongs.</param>
	/// <param name="originalWord">The original word on the card.</param>
	/// <param name="translatedWord">The translated word on the card.</param>
	/// <param name="correctReviewStreak">The number of consecutive correct reviews for the card.</param>
	/// <param name="nextReviewDate">The date and time when the card is next due for review.</param>
	/// <param name="previousCorrectReview">The date and time of the last correct review for the card.</param>
	public Card(Guid deckId, string originalWord, string translatedWord, int correctReviewStreak, DateTime nextReviewDate, DateTime previousCorrectReview)
		: this(Guid.NewGuid(), deckId, originalWord, translatedWord, correctReviewStreak, nextReviewDate, previousCorrectReview)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Card"/> class with the specified parameters.
	/// </summary>
	/// <param name="id">The unique identifier of the card.</param>
	/// <param name="deckId">The unique identifier of the deck to which the card belongs.</param>
	/// <param name="originalWord">The original word on the card.</param>
	/// <param name="translatedWord">The translated word on the card.</param>
	/// <param name="correctReviewStreak">The number of consecutive correct reviews for the card.</param>
	/// <param name="nextReviewDate">The date and time when the card is next due for review.</param>
	/// <param name="previousCorrectReview">The date and time of the last correct review for the card.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="originalWord"/> or <paramref name="translatedWord"/> is null or whitespace.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="correctReviewStreak"/> is negative, <paramref name="nextReviewDate"/> is in the past, or <paramref name="previousCorrectReview"/> is in the future.</exception>
	public Card(Guid id, Guid deckId, string originalWord, string translatedWord, int correctReviewStreak, DateTime nextReviewDate, DateTime previousCorrectReview)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(originalWord);
		ArgumentException.ThrowIfNullOrWhiteSpace(translatedWord);
		ArgumentOutOfRangeException.ThrowIfNegative(correctReviewStreak);

		if (nextReviewDate < DateTime.UtcNow)
		{
			throw new ArgumentOutOfRangeException(nameof(nextReviewDate), "Next review date cannot be in the past.");
		}

		if (previousCorrectReview > DateTime.UtcNow)
		{
			throw new ArgumentOutOfRangeException(nameof(previousCorrectReview), "Previous correct review date cannot be in the future.");
		}

		Id = id;
		DeckId = deckId;
		OriginalWord = originalWord;
		TranslatedWord = translatedWord;
		NextReviewDate = nextReviewDate;
		PreviousCorrectReview = previousCorrectReview;
	}

	/// <summary>
	/// Sets the next review date for the card.
	/// </summary>
	/// <param name="nextReviewDate">The date and time when the card is next due for review.</param>
	/// <param name="currentDate">The current date and time, used for validation. Defaults to the current UTC time if not provided.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="nextReviewDate"/> is in the past relative to <paramref name="currentDate"/>.</exception>
	public void SetNextReviewDate(DateTime nextReviewDate, DateTime? currentDate = null)
	{
		currentDate ??= DateTime.UtcNow;

		if (nextReviewDate < currentDate)
		{
			throw new ArgumentOutOfRangeException(nameof(nextReviewDate), "Next review date cannot be in the past.");
		}

		NextReviewDate = nextReviewDate;
	}

	/// <summary>
	/// Sets the date and time of the last correct review for the card.
	/// </summary>
	/// <param name="previousCorrectReview">The date and time of the last correct review.</param>
	/// <param name="currentDate">The current date and time, used for validation. Defaults to the current UTC time if not provided.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="previousCorrectReview"/> is in the future relative to <paramref name="currentDate"/>.</exception>
	public void SetPreviousCorrectReview(DateTime previousCorrectReview, DateTime? currentDate = null)
	{
		currentDate ??= DateTime.UtcNow;
		
		if (previousCorrectReview > currentDate)
		{
			throw new ArgumentOutOfRangeException(nameof(previousCorrectReview), "Previous correct review date cannot be in the future.");
		}

		PreviousCorrectReview = previousCorrectReview;
	}

	/// <summary>
	/// Sets the number of consecutive correct reviews for the card.
	/// </summary>
	/// <param name="correctReviewStreak">The number of consecutive correct reviews.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="correctReviewStreak"/> is negative.</exception>
	public void SetCorrectReviewStreak(int correctReviewStreak)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(correctReviewStreak);

		CorrectReviewStreak = correctReviewStreak;
	}
}