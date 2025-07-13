using Moq;

using Repetify.Crosscutting;
using Repetify.Crosscutting.Abstractions;
using Repetify.Crosscutting.Extensions;
using Repetify.Crosscutting.Time;
using Repetify.Domain.Entities;
using Repetify.Testing.Extensions;

namespace Repetify.Domain.UnitTests.Entities;

public sealed class CardTests : IDisposable
{
	private readonly DateTime _fixedNow = new(2025, 7, 5, 12, 0, 0, DateTimeKind.Utc);
	
	public CardTests()
	{
		var clockMock = new Mock<IClock>();
		clockMock.SetupGet(p => p.UtcNow).Returns(_fixedNow);
		Clock.Set(clockMock.Object);
	}

	[Fact]
	public void Card_Initializes_WhenValidValuesAreProvided()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var front = "Hello";
		var back = "Hola";

		// Act
		var card = Card.Create(deckId, front, back, 0, 0, 2.5, 1, _fixedNow.AddDays(1), _fixedNow).EnsureSuccess();

		// Assert
		Assert.NotNull(card);
		Assert.NotEqual(Guid.Empty, card.Id);
		Assert.Equal(deckId, card.DeckId);
		Assert.Equal(front, card.Front);
		Assert.Equal(back, card.Back);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenfrontIsNull()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		string front = null!;
		var back = "Hola";

		// Act
		var cardResult = Card.Create(deckId, front, back, 0, 0, 2.5, 1, _fixedNow.AddDays(1), _fixedNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenfrontIsEmpty()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var front = string.Empty;
		var back = "Hola";

		// Act
		var cardResult = Card.Create(deckId, front, back, 0, 0, 2.5, 1, _fixedNow.AddDays(1), _fixedNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenfrontIsWhitespace()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var front = "   ";
		var back = "Hola";

		// Act
		var cardResult = Card.Create(deckId, front, back, 0, 0, 2.5, 1, _fixedNow.AddDays(1), _fixedNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenbackIsNull()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var front = "Hello";
		string back = null!;

		// Act
		var cardResult = Card.Create(deckId, front, back, 0, 0, 2.5, 1, _fixedNow.AddDays(1), _fixedNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenbackIsEmpty()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var front = "Hello";
		var back = string.Empty;

		// Act
		var cardResult = Card.Create(deckId, front, back, 0, 0, 2.5, 1, _fixedNow.AddDays(1), _fixedNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenbackIsWhitespace()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var front = "Hello";
		var back = "   ";

		// Act
		var cardResult = Card.Create(deckId, front, back, 0, 0, 2.5, 1, _fixedNow.AddDays(1), _fixedNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenNextReviewDateIsInThePast()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var front = "Hello";
		var back = "Hola";
		var nextReviewDate = _fixedNow.AddDays(-1);

		// Act
		var cardResult = Card.Create(deckId, front, back, 0, 0, 2.5, 1, nextReviewDate, _fixedNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenPreviousCorrectReviewIsInTheFuture()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var front = "Hello";
		var back = "Hola";
		var previousCorrectReview = _fixedNow.AddDays(1);

		// Act
		var cardResult = Card.Create(deckId, front, back, 0, 0, 2.5, 1, _fixedNow.AddDays(1), previousCorrectReview);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenReviewStreakIsNegative()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var front = "Hello";
		var back = "Hola";

		// Act
		var cardResult = Card.Create(deckId, front, back, -1, 0, 2.5, 1, _fixedNow.AddDays(1), _fixedNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void SetNextReviewDate_UpdatesNextReviewDate()
	{
		// Arrange
		var cardResult = Card.Create(Guid.NewGuid(), "Hello", "Hola", 0, 0, 2.5, 1, _fixedNow.AddDays(1), _fixedNow);
		Assert.True(cardResult.IsSuccess);
		var card = cardResult.Value;
		var newReviewDate = _fixedNow.AddDays(2);

		// Act
		card.SetNextReviewDate(newReviewDate);

		// Assert
		Assert.Equal(newReviewDate, card.NextReviewDate);
	}

	[Fact]
	public void SetNextReviewDate_ReturnsInvalidARgumentResult_WhenDateIsInThePast()
	{
		// Arrange
		var card = Card.Create(Guid.NewGuid(), "Hello", "Hola", 0, 1, 2.5, 1, _fixedNow.AddDays(1), _fixedNow).AssertIsSuccess();
		var pastDate = _fixedNow.AddDays(-1);

		// Act & Assert
		var errorResult = card.SetNextReviewDate(pastDate);
		Assert.False(errorResult.IsSuccess);
		Assert.Equal(Crosscutting.ResultStatus.InvalidArguments, errorResult.Status);
	}

	[Fact]
	public void SetPreviousCorrectReview_UpdatesPreviousCorrectReview()
	{
		// Arrange
		var cardResult = Card.Create(Guid.NewGuid(), "Hello", "Hola", 0, 0, 2.5, 1, _fixedNow.AddDays(1), _fixedNow);
		Assert.True(cardResult.IsSuccess);
		var card = cardResult.Value;
		var newPreviousReviewDate = _fixedNow.AddDays(-1);

		// Act
		card.SetPreviousCorrectReview(newPreviousReviewDate);

		// Assert
		Assert.Equal(newPreviousReviewDate, card.PreviousCorrectReview);
	}

	[Fact]
	public void SetPreviousCorrectReview_ReturnsUnsuccessfulResult_WhenDateIsInTheFuture()
	{
		// Arrange
		var card = Card.Create(Guid.NewGuid(), "Hello", "Hola", 0, 1, 2.5, 1, _fixedNow.AddDays(1), _fixedNow).EnsureSuccess();
		var futureDate = _fixedNow.AddDays(1);

		// Act & Assert
		var errorResult = card.SetPreviousCorrectReview(futureDate);
		Assert.False(errorResult.IsSuccess);
		Assert.Equal(ResultStatus.InvalidArguments, errorResult.Status);
	}

	[Fact]
	public void SetCorrectReviewStreak_UpdatesCorrectReviewStreak()
	{
		// Arrange
		var cardResult = Card.Create(Guid.NewGuid(), "Hello", "Hola", 0, 0, 2.5, 1, _fixedNow.AddDays(1), _fixedNow);
		Assert.True(cardResult.IsSuccess);
		var card = cardResult.Value;
		var newStreak = 5;

		// Act
		card.SetCorrectReviewStreak(newStreak);

		// Assert
		Assert.Equal(newStreak, card.CorrectReviewStreak);
	}

	[Fact]
	public void SetCorrectReviewStreak_ReturnsUnsuccessfulResult_WhenStreakIsNegative()
	{
		// Arrange
		var cardResult = Card.Create(Guid.NewGuid(), "Hello", "Hola", 0, 0, 2.5, 1, _fixedNow.AddDays(1), _fixedNow);
		Assert.True(cardResult.IsSuccess);
		var card = cardResult.Value;

		// Act & Assert
		Assert.False(card.SetCorrectReviewStreak(-1).IsSuccess);
	}

	public void Dispose()
	{
		Clock.Reset();
	}

	[Fact]
	public void SetInterval_Should_Set_Valid_Interval()
	{
		var card = CreateValidCard();
		var result = card.SetInterval(5);

		Assert.True(result.IsSuccess);
		Assert.Equal(5, card.Interval);
	}

	[Fact]
	public void SetInterval_Should_Fail_For_Negative_Interval()
	{
		var card = CreateValidCard();
		var result = card.SetInterval(-1);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void SetRepetitions_Should_Set_Valid_Value()
	{
		var card = CreateValidCard();
		var result = card.SetRepetitions(3);

		Assert.True(result.IsSuccess);
		Assert.Equal(3, card.Repetitions);
	}

	[Fact]
	public void SetRepetitions_Should_Fail_For_Negative_Value()
	{
		var card = CreateValidCard();
		var result = card.SetRepetitions(-1);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void SetEaseFactor_Should_Set_Valid_Value()
	{
		var card = CreateValidCard();
		var result = card.SetEaseFactor(2.6);

		Assert.True(result.IsSuccess);
		Assert.Equal(2.6, card.EaseFactor);
	}

	[Fact]
	public void SetEaseFactor_Should_Fail_If_Less_Than_Minimum()
	{
		var card = CreateValidCard();
		var result = card.SetEaseFactor(2.4);

		Assert.False(result.IsSuccess);
	}

	[Theory]
	[InlineData(5, 0, 1)]
	[InlineData(5, 1, 6)]
	[InlineData(5, 2, 15)]
	public void ApplyReview_Should_Apply_Correct_Logic_For_Correct_Answer(int quality, int repetitionsBefore, int expectedInterval)
	{
		var initialInterval = repetitionsBefore switch
		{
			1 => 1,
			2 => 6,
			3 => 15,
			_ => 1
		};

		var card = Card.Create(
			Guid.NewGuid(),
			"Front",
			"Back",
			correctReviewStreak: 1,
			repetitions: repetitionsBefore,
			easeFactor: 2.5,
			interval: initialInterval,
			nextReviewDate: _fixedNow.AddDays(1),
			previousCorrectReview: _fixedNow).AssertIsSuccess();

		var result = card.ApplyReview(quality);

		Assert.True(result.IsSuccess);
		Assert.Equal(repetitionsBefore + 1, card.Repetitions);
		Assert.Equal(expectedInterval, card.Interval);
		Assert.True(card.EaseFactor >= 2.5);
		Assert.Equal(_fixedNow.Date, card.PreviousCorrectReview.Date);
		Assert.Equal(_fixedNow.AddDays(expectedInterval).Date, card.NextReviewDate.Date);
	}

	[Fact]
	public void ApplyReview_Should_Reset_On_Incorrect_Answer()
	{
		var card = Card.Create(
			Guid.NewGuid(),
			"Front",
			"Back",
			correctReviewStreak: 2,
			repetitions: 2,
			easeFactor: 2.5,
			interval: 6,
			nextReviewDate: _fixedNow.AddDays(1),
			previousCorrectReview: _fixedNow).Value!;

		var result = card.ApplyReview(2);

		Assert.True(result.IsSuccess);
		Assert.Equal(0, card.Repetitions);
		Assert.Equal(0, card.CorrectReviewStreak);
		Assert.Equal(1, card.Interval);
		Assert.True(card.EaseFactor >= 2.5);
		Assert.Equal(_fixedNow.AddDays(1).Date, card.NextReviewDate.Date);
	}

	[Fact]
	public void ApplyReview_Should_Fail_If_Quality_Is_Out_Of_Range()
	{
		var card = CreateValidCard();
		var result = card.ApplyReview(0);

		Assert.False(result.IsSuccess);
	}

	private Card CreateValidCard()
	{
		return Card.Create(
			Guid.NewGuid(),
			"Front",
			"Back",
			correctReviewStreak: 1,
			repetitions: 1,
			easeFactor: 2.5,
			interval: 1,
			nextReviewDate: _fixedNow.AddDays(1),
			previousCorrectReview: _fixedNow).Value!;
	}


}
