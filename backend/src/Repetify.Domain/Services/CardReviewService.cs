using Repetify.Crosscutting;
using Repetify.Crosscutting.Abstractions;
using Repetify.Crosscutting.Time;
using Repetify.Domain.Abstractions.Services;
using Repetify.Domain.Entities;

namespace Repetify.Domain.Services;

/// <summary>
/// Service for handling card reviews.
/// </summary>
public sealed class CardReviewService : ICardReviewService, IDisposable
{
	private const double MinEaseFactorInSM2 = 1.3;
	private readonly IClock _clock;

	/// <summary>
	/// Initializes a new instance of the <see cref="CardReviewService"/> class.
	/// </summary>
	/// <param name="clock">The clock service to provide current time.</param>
	public CardReviewService(IClock clock)
	{
		_clock = clock;
		Clock.Set(clock);
	}

	/// <inheritdoc/>
	public Result UpdateReview(Card card, int quality)
	{
		if (ResultValidator.ValidateNotNull(card, out var result))
		{
			return result;
		}

		if (!ResultValidator.ValidateInRange(quality, 1, 5, out result))
		{
			return ResultFactory.InvalidArgument("The quality should be between 0 and 5.");
		}

		if (quality >= 3)
		{
			// Response considered "correct".
			card.SetRepetitions(card.Repetitions + 1);

			int interval = card.Repetitions switch
			{
				1 => 1,
				2 => 6,
				_ => (int)Math.Round(card.Interval * card.EaseFactor)
			};

			card.SetInterval(interval);

			double ef = card.EaseFactor + (0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02));
			if (ef < MinEaseFactorInSM2)
			{
				ef = MinEaseFactorInSM2;
			}

			card.SetEaseFactor(ef);
			card.SetCorrectReviewStreak(card.CorrectReviewStreak + 1);
			card.SetPreviousCorrectReview(_clock.UtcNow);
			card.SetNextReviewDate(_clock.UtcNow.AddDays(interval));
		}
		else
		{
			// Incorrect response
			card.SetRepetitions(0);
			card.SetInterval(1);

			double ef = card.EaseFactor - 0.2;
			if (ef < MinEaseFactorInSM2)
			{
				ef = MinEaseFactorInSM2;
			}

			card.SetEaseFactor(ef);

			card.SetCorrectReviewStreak(0);
			card.SetNextReviewDate(_clock.UtcNow.AddDays(1));
		}

		return ResultFactory.Success();
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		Clock.Reset();
	}
	
	/// <summary>
	/// Calculates the next review date based on the current review streak.
	/// </summary>
	/// <param name="card">The card being reviewed.</param>
	/// <returns>The next review date.</returns>
	private DateTime CalculateNextReviewDate(Card card)
	{
		// Simple algorithm to space reviews based on streak and difficulty
		int interval = card.CorrectReviewStreak;
		return _clock.UtcNow.AddDays(interval);
	}
}
