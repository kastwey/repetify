using Moq;

using Repetify.Domain.Abstractions;
using Repetify.Domain.Entities;
using Repetify.Domain.Services;

namespace Repetify.Domain.Tests.Services;

public class CardReviewServiceTests
{

	private readonly Mock<IClock> _clockMock;

	public CardReviewServiceTests()
	{
		_clockMock = new Mock<IClock>();
		_clockMock.Setup(c => c.UtcNow).Returns(new DateTime(2025, 1, 1, 0, 0, 0));
	}

	[Fact]
	public void UpdateReview_Should_Increment_Streak_And_Adjust_NextReviewDate_When_Correct()
	{
		// Arrange
		var card = new Card(Guid.NewGuid(), Guid.NewGuid(), "Hello", "Hola");
		var cardReviewService = new CardReviewService(_clockMock.Object);

		// Act
		cardReviewService.UpdateReview(card, true);

		// Assert
		Assert.Equal(1, card.CorrectReviewStreak);
		Assert.Equal(card.NextReviewDate, _clockMock.Object.UtcNow.AddDays(1));
		Assert.Equal(_clockMock.Object.UtcNow, card.PreviousCorrectReview);
	}

	[Fact]
	public void UpdateReview_Should_Reset_Streak_And_Set_NextReviewDate_When_Incorrect()
	{
		// Arrange
		var card = new Card(Guid.NewGuid(), Guid.NewGuid(), "Hello", "Hola");
		var cardReviewService = new CardReviewService(_clockMock.Object);

		cardReviewService.UpdateReview(card, true);

		// Act
		cardReviewService.UpdateReview(card, false);

		// Assert
		Assert.Equal(0, card.CorrectReviewStreak);
		Assert.Equal(card.NextReviewDate, _clockMock.Object.UtcNow.AddDays(1));
	}

	[Fact]
	public void UpdateReview_Should_Use_Streak_For_NextReviewDate_Calculation()
	{
		// Arrange
		var card = new Card(Guid.NewGuid(), Guid.NewGuid(), "Hello", "Hola");
		var cardReviewService = new CardReviewService(_clockMock.Object);
		var today = _clockMock.Object.UtcNow;

		// Act
		cardReviewService.UpdateReview(card, true); // Streak = 1
		var nextReview1 = card.NextReviewDate;

		cardReviewService.UpdateReview(card, true); // Streak = 2
		var nextReview2 = card.NextReviewDate;

		// Assert
		Assert.Equal(today.AddDays(1), nextReview1);
		Assert.Equal(today.AddDays(2), nextReview2);
	}
}
