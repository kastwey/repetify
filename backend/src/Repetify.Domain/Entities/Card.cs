using Repetify.Crosscutting;
using Repetify.Crosscutting.Time;

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

	private Card(Guid id, Guid deckId, string originalWord, string translatedWord, int correctReviewStreak, DateTime nextReviewDate, DateTime previousCorrectReview)
	{
		Id = id;
		DeckId = deckId;
		OriginalWord = originalWord;
		TranslatedWord = translatedWord;
		CorrectReviewStreak = correctReviewStreak;
		NextReviewDate = nextReviewDate;
		PreviousCorrectReview = previousCorrectReview;
	}

	public static Result<Card> Create(
		Guid? id,
		Guid deckId,
		string originalWord,
		string translatedWord,
		int correctReviewStreak = 0,
		DateTime? nextReviewDate = null,
		DateTime? previousCorrectReview = null) =>
		Create(id, deckId, originalWord, translatedWord, correctReviewStreak, nextReviewDate, previousCorrectReview, true);


		/// <summary>
	/// Attempts to create a new Card instance, returning a Result<Card> indicating success or failure.
	/// </summary>
	public static Result<Card> Create(
		Guid deckId,
		string originalWord,
		string translatedWord,
		int correctReviewStreak = 0,
		DateTime? nextReviewDate = null,
		DateTime? previousCorrectReview = null)
	{
		return Create(null, deckId, originalWord, translatedWord, correctReviewStreak, nextReviewDate, previousCorrectReview);
	}

	/// <summary>
	/// Sets the next review date for the card.
	/// </summary>
	/// <param name="nextReviewDate">The date and time when the card is next due for review.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="nextReviewDate"/> is in the past relative to <paramref name="currentDate"/>.</exception>
	public void SetNextReviewDate(DateTime nextReviewDate)
	{
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
		currentDate ??= Clock.Current.UtcNow;

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

	internal static Result<Card> RehidrateFromPersistence(
		Guid? id,
		Guid deckId,
		string originalWord,
		string translatedWord,
		int correctReviewStreak = 0,
		DateTime? nextReviewDate = null,
		DateTime? previousCorrectReview = null) =>
		Create(id, deckId, originalWord, translatedWord, correctReviewStreak, nextReviewDate, previousCorrectReview, false);

	/// <summary>
	/// Attempts to create a new Card instance, returning a Result<Card> indicating success or failure.
	/// </summary>
	private static Result<Card> Create(
		Guid? id,
		Guid deckId,
		string originalWord,
		string translatedWord,
		int correctReviewStreak = 0,
		DateTime? nextReviewDate = null,
		DateTime? previousCorrectReview = null,
		bool validateDates = true)
	{
		var errors = new List<string>();

		if (string.IsNullOrWhiteSpace(originalWord))
		{
			errors.Add("Original word cannot be null or whitespace.");
		}

		if (string.IsNullOrWhiteSpace(translatedWord))
		{
			errors.Add("Translated word cannot be null or whitespace.");
		}

		if (correctReviewStreak < 0)
		{
			errors.Add("Correct review streak cannot be negative.");
		}

		var now = Clock.Current.UtcNow;
		var nextReview = nextReviewDate ?? now.AddDays(1);
		var prevCorrect = previousCorrectReview ?? DateTime.MinValue;

		if (validateDates)
		{
			if (nextReview < now)
			{
				errors.Add("Next review date cannot be in the past.");
			}

			if (prevCorrect > now)
			{
				errors.Add("Previous correct review date cannot be in the future.");
			}
		}

		if (errors.Count > 0)
		{
			return ResultFactory.BusinessRuleViolated<Card>(errors.ToArray());
		}

		var card = new Card(
			id ?? Guid.NewGuid(),
			deckId,
			originalWord,
			translatedWord,
			correctReviewStreak,
			nextReview,
			prevCorrect
		);

		return ResultFactory.Success(card);
	}
}
