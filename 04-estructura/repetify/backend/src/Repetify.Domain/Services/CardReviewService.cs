using Repetify.Domain.Abstractions;
using Repetify.Domain.Abstractions.Services;
using Repetify.Domain.Entities;

namespace Repetify.Domain.Services;

/// <summary>
/// Service for handling card reviews.
/// </summary>
public class CardReviewService : ICardReviewService
{
	private readonly IClock _clock;

	/// <summary>
	/// Initializes a new instance of the <see cref="CardReviewService"/> class.
	/// </summary>
	/// <param name="clock">The clock service to provide current time.</param>
	public CardReviewService(IClock clock)
	{
		_clock = clock;
	}

	/// <inheritdoc/>
	/// <summary>
	/// Updates the review status of the card based on whether the review was correct.
	/// </summary>
	/// <param name="card">The card being reviewed.</param>
	/// <param name="isCorrect">Indicates whether the review was correct.</param>
	public void UpdateReview(Card card, bool isCorrect)
	{
		ArgumentNullException.ThrowIfNull(card);

		if (isCorrect)
		{
			card.SetCorrectReviewStreak(card.CorrectReviewStreak + 1);
			card.SetPreviousCorrectReview(_clock.UtcNow, _clock.UtcNow);

			// Adjust next review date based on streak
			card.SetNextReviewDate(CalculateNextReviewDate(card), _clock.UtcNow);
		}
		else
		{
			card.SetCorrectReviewStreak(0);
			card.SetNextReviewDate(_clock.UtcNow.AddDays(1), _clock.UtcNow); // Reset to review tomorrow
		}
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
