using Repetify.Crosscutting;
using Repetify.Crosscutting.Exceptions;
using Repetify.Crosscutting.Extensions;
using Repetify.Crosscutting.Time;

namespace Repetify.Domain.Entities;

/// <summary>
/// Represents a card entity used for language learning, containing information about the original and backs, review streaks, and review dates.
/// </summary>
public class Card
{
	private const double MinEaseFactorInSM2 = 2.5;

	/// <summary>
	/// Gets the unique identifier of the card.
	/// </summary>
	public Guid Id { get; private set; }

	/// <summary>
	/// Gets the unique identifier of the deck to which the card belongs.
	/// </summary>
	public Guid DeckId { get; private set; }

	/// <summary>
	/// Gets the front side of the card.
	/// </summary>
	public string Front { get; private set; }

	/// <summary>
	/// Gets the back side of the card.
	/// </summary>
	public string Back { get; private set; }

	/// <summary>
	/// Gets the number of consecutive correct reviews for the card.
	/// </summary>
	public int CorrectReviewStreak { get; private set; }

	/// <summary>
	/// The number of consecutive successful reviews with a quality rating of 3 or higher,
	/// as defined by the SM-2 spaced repetition algorithm. This value is used to determine
	/// the interval until the next review. It resets to 0 when the quality is less than 3.
	/// </summary>
	public int Repetitions { get; private set; }

	/// <summary>
	/// Gets the ease factor used for scheduling the card's reviews.
	/// </summary>
	public double EaseFactor { get; private set; }
	
	/// <summary>
	/// The number of days between the last successful review and the next one,
	/// as determined by the SM-2 algorithm. This interval increases or resets 
	/// based on the quality of the user's responses.
	/// </summary>
	public int Interval { get; private set; }

	/// <summary>
	/// Gets the date and time when the card is next due for review.
	/// </summary>
	public DateTime NextReviewDate { get; private set; }

	/// <summary>
	/// Gets the date and time of the last correct review for the card.
	/// </summary>
	public DateTime PreviousCorrectReview { get; private set; }

	private Card(Guid id, Guid deckId, string front, string back, int correctReviewStreak, int repetitions, double easeFactor, int interval, DateTime nextReviewDate, DateTime previousCorrectReview)
	{
		Id = id;
		DeckId = deckId;
		Front = front;
		Back = back;
		CorrectReviewStreak = correctReviewStreak;
		Repetitions = repetitions;
		EaseFactor = easeFactor;
		Interval = interval;
		NextReviewDate = nextReviewDate;
		PreviousCorrectReview = previousCorrectReview;
	}

	/// <summary>
	/// Creates a new <see cref="Card"/> instance with the specified parameters, including the option to provide an explicit ID and to validate date constraints.
	/// </summary>
	/// <param name="id">The unique identifier of the card. If null, a new GUID will be generated.</param>
	/// <param name="deckId">The unique identifier of the deck to which the card belongs.</param>
	/// <param name="front">The front side of the card.</param>
	/// <param name="back">The back side of the card.</param>
	/// <param name="correctReviewStreak">The number of consecutive correct reviews for the card. Defaults to 0.</param>
	/// <param name="repetitions">The number of consecutive successful reviews with a quality rating of 3 or higher (SM-2 algorithm). Defaults to 0.</param>
	/// <param name="easeFactor">The ease factor used for scheduling the card's reviews. Defaults to 0.3.</param>
	/// <param name="interval">The number of days between the last successful review and the next one.</param>
	/// <param name="nextReviewDate">The date and time when the card is next due for review. If null, defaults to one day from now.</param>
	/// <param name="previousCorrectReview">The date and time of the last correct review for the card. If null, defaults to <see cref="DateTime.MinValue"/>.</param>
	/// <returns>A <see cref="Result{Card}"/> indicating success or failure of the operation, and containing the created <see cref="Card"/> if successful.</returns>
	public static Result<Card> Create(
					Guid? id,
					Guid deckId,
					string front,
					string back,
					int correctReviewStreak = 0,
					int repetitions = 0,
					double easeFactor = MinEaseFactorInSM2, // default value in SM2
					int interval = 1,
					DateTime? nextReviewDate = null,
					DateTime? previousCorrectReview = null) =>
					Create(id, deckId, front, back, correctReviewStreak, repetitions, easeFactor, interval, nextReviewDate, previousCorrectReview, true);

	/// <summary>
	/// Creates a new <see cref="Card"/> instance with the specified parameters.
	/// </summary>
	/// <param name="deckId">The unique identifier of the deck to which the card belongs.</param>
	/// <param name="front">The original word on the card.</param>
	/// <param name="back">The back on the card.</param>
	/// <param name="correctReviewStreak">The number of consecutive correct reviews for the card. Defaults to 0.</param>
	/// <param name="repetitions">The number of consecutive successful reviews with a quality rating of 3 or higher (SM-2 algorithm). Defaults to 0.</param>
	/// <param name="easeFactor">The ease factor used for scheduling the card's reviews. Defaults to 0.3.</param>
	/// <param name="interval">The number of days between the last successful review and the next one</param>
	/// <param name="nextReviewDate">The date and time when the card is next due for review. If null, defaults to one day from now.</param>
	/// <param name="previousCorrectReview">The date and time of the last correct review for the card. If null, defaults to <see cref="DateTime.MinValue"/>.</param>
	/// <returns>A <see cref="Result{Card}"/> indicating success or failure of the operation, and containing the created <see cref="Card"/> if successful.</returns>
	public static Result<Card> Create(
	Guid deckId,
	string front,
	string back,
	int correctReviewStreak = 0,
	int repetitions = 0,
	double easeFactor = MinEaseFactorInSM2, // default value in SM2
	int interval = 1,
	DateTime? nextReviewDate = null,
	DateTime? previousCorrectReview = null)
	{
		return Create(null, deckId, front, back, correctReviewStreak, repetitions, easeFactor, interval, nextReviewDate, previousCorrectReview);
	}

	/// <summary>
	/// Sets the number of consecutive successful reviews with a quality rating of 3 or higher (SM-2 algorithm).
	/// </summary>
	/// <param name="repetitions">The number of repetitions to set. Must be positive.</param>
	/// <returns>A <see cref="Result"/> indicating success or failure of the operation.</returns>
	public Result SetRepetitions(int repetitions)
	{
		if (!ResultValidator.ValidateNotNegative(repetitions, out var result))
		{
			return result;
		}

		Repetitions = repetitions;
		return ResultFactory.Success();
	}

	public Result SetEaseFactor(double easeFactor)
	{
		if (!ResultValidator.ValidateGreaterThanOrEqualTo(easeFactor, MinEaseFactorInSM2, out var result))
		{
			return result;
		}

		EaseFactor = easeFactor;
		return ResultFactory.Success();
	}

	/// <summary>
	/// Sets the interval (in days) between the last successful review and the next one for the card.
	/// </summary>
	/// <param name="interval">The interval in days. Must be non-negative.</param>
	/// <returns>A <see cref="Result"/> indicating success or failure of the operation.</returns>
	public Result SetInterval(int interval)
	{
		if (!ResultValidator.ValidateNotNegative(interval, out var result))
		{
			return result;
		}

		Interval = interval;
		return ResultFactory.Success();
	}

	/// <summary>
	/// Sets the next review date for the card.
	/// </summary>
	/// <param name="nextReviewDate">The date and time when the card is next due for review.</param>
	public Result SetNextReviewDate(DateTime nextReviewDate)
	{
		if (nextReviewDate < Clock.Current.UtcNow)
		{
			return ResultFactory.InvalidArgument($"The {nameof(nextReviewDate)} cannot be in the past.");
		}

		NextReviewDate = nextReviewDate;
		return ResultFactory.Success();
	}

	/// <summary>
	/// Sets the date and time of the last correct review for the card.
	/// </summary>
	/// <param name="previousCorrectReview">The date and time of the last correct review.</param>
	/// <param name="currentDate">The current date and time, used for validation. Defaults to the current UTC time if not provided.</param>
	public Result SetPreviousCorrectReview(DateTime previousCorrectReview)
	{
		if (previousCorrectReview > Clock.Current.UtcNow)
		{
			return ResultFactory.InvalidArgument("Previous correct review date cannot be in the future.");
		}

		PreviousCorrectReview = previousCorrectReview;
		return ResultFactory.Success();
	}

	/// <summary>
	/// Sets the number of consecutive correct reviews for the card.
	/// </summary>
	/// <param name="correctReviewStreak">The number of consecutive correct reviews.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="correctReviewStreak"/> is negative.</exception>
	public Result SetCorrectReviewStreak(int correctReviewStreak)
	{
		if (correctReviewStreak < 0)
		{
			return ResultFactory.InvalidArgument($"{nameof(correctReviewStreak)} cannot be negative.");
		}

		CorrectReviewStreak = correctReviewStreak;
		return ResultFactory.Success();
	}

	public Result ApplyReview(int quality)
	{
		try
		{
			if (!ResultValidator.ValidateInRange(quality, 1, 5, out var result))
			{
				return result;
			}

			var now = Clock.Current.UtcNow;

			if (quality >= 3)
			{
				// Correct response
				SetRepetitions(Repetitions + 1).EnsureSuccess();

				int interval = Repetitions switch
				{
					1 => 1,
					2 => 6,
					_ => (int)Math.Round(Interval * EaseFactor)
				};

				SetInterval(interval).EnsureSuccess();

				// Adjust ease factor using SM-2 formula
				double newEf = EaseFactor + (0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02));
				newEf = Math.Max(MinEaseFactorInSM2, newEf);
				SetEaseFactor(newEf).EnsureSuccess();

				SetCorrectReviewStreak(CorrectReviewStreak + 1).EnsureSuccess();
				SetPreviousCorrectReview(now).EnsureSuccess();
				SetNextReviewDate(now.AddDays(interval)).EnsureSuccess();
			}
			else
			{
				// Incorrect response
				SetRepetitions(0).EnsureSuccess();
				SetCorrectReviewStreak(0).EnsureSuccess();
				SetInterval(1).EnsureSuccess();

				double newEf = EaseFactor - 0.2;
				newEf = Math.Max(MinEaseFactorInSM2, newEf);
				SetEaseFactor(newEf).EnsureSuccess();

				SetNextReviewDate(now.AddDays(1)).EnsureSuccess();
			}

			return ResultFactory.Success();
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure(ex.Result);
		}
	}

	internal static Result<Card> RehidrateFromPersistence(
		Guid? id,
		Guid deckId,
		string front,
		string back,
		int correctReviewStreak = 0,
		int repetitions = 0,
		double easeFactor = MinEaseFactorInSM2,
		int interval = 0,
		DateTime? nextReviewDate = null,
		DateTime? previousCorrectReview = null) =>
		Create(id, deckId, front, back, correctReviewStreak, repetitions, easeFactor, interval, nextReviewDate, previousCorrectReview, false);

	/// <summary>
	/// Attempts to create a new Card instance, returning a Result<Card> indicating success or failure.
	/// </summary>
	private static Result<Card> Create(
		Guid? id,
		Guid deckId,
		string front,
		string back,
		int correctReviewStreak = 0,
		int repetitions = 0,
		double easeFactor = MinEaseFactorInSM2,
		int interval = 0,
		DateTime? nextReviewDate = null,
		DateTime? previousCorrectReview = null,
		bool validateDates = true)
	{
		var errors = new List<Result>();

		if (!ResultValidator.ValidateNotNullOrWhiteSpace(front, out var result))
		{
			errors.Add(result);
		}

		if (!ResultValidator.ValidateNotNullOrWhiteSpace(back, out result))
		{
			errors.Add(result);
		}

		if (!ResultValidator.ValidateNotNegative(correctReviewStreak, out result))
		{
			errors.Add(result);
		}

		if (!ResultValidator.ValidateNotNegative(repetitions, out result))
		{
			errors.Add(result);
		}

		if (!ResultValidator.ValidateGreaterThanOrEqualTo(easeFactor, MinEaseFactorInSM2, out result))
		{
			errors.Add(result);
		}

		if (!ResultValidator.ValidatePositive(interval, out result))
		{
			errors.Add(result);
		}

		var now = Clock.Current.UtcNow;
		var nextReview = nextReviewDate ?? now.AddDays(1);
		var prevCorrect = previousCorrectReview ?? DateTime.MinValue;

		if (validateDates)
		{
			if (nextReview < now)
			{
				errors.Add(ResultFactory.InvalidArgument("Next review date cannot be in the past."));
			}

			if (prevCorrect > now)
			{
				errors.Add(ResultFactory.InvalidArgument("Previous correct review date cannot be in the future."));
			}
		}

		if (errors.Count > 0)
		{
			return ResultFactory.BusinessRuleViolated<Card>(errors.Select(r => r.ErrorMessage!).ToArray());
		}

		var card = new Card(
			id ?? Guid.NewGuid(),
			deckId,
			front,
			back,
			correctReviewStreak,
			repetitions,
			easeFactor,
			interval,
			nextReview,
			prevCorrect
		);

		return ResultFactory.Success(card);
	}

}
