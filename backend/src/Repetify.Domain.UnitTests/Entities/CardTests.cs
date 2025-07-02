using Moq;

using Repetify.Crosscutting;
using Repetify.Crosscutting.Abstractions;
using Repetify.Crosscutting.Time;
using Repetify.Domain.Entities;

namespace Repetify.Domain.UnitTests.Entities;

public sealed class CardTests : IDisposable
{

	public CardTests()
	{
		var clockMock = new Mock<IClock>();
		clockMock.SetupGet(p => p.UtcNow).Returns(new DateTime(2025, 1, 1, 0, 0, 0));
		Clock.Set(clockMock.Object);
	}

	[Fact]
	public void Card_Initializes_WhenValidValuesAreProvided()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "Hello";
		var translatedWord = "Hola";

		// Act
		var cardResult = Card.Create(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);

		// Assert
		Assert.True(cardResult.IsSuccess);
		var card = cardResult.Value;
		Assert.NotNull(card);
		Assert.NotEqual(Guid.Empty, card.Id);
		Assert.Equal(deckId, card.DeckId);
		Assert.Equal(originalWord, card.Front);
		Assert.Equal(translatedWord, card.Back);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenOriginalWordIsNull()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		string originalWord = null!;
		var translatedWord = "Hola";

		// Act
		var cardResult = Card.Create(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenOriginalWordIsEmpty()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = string.Empty;
		var translatedWord = "Hola";

		// Act
		var cardResult = Card.Create(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenOriginalWordIsWhitespace()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "   ";
		var translatedWord = "Hola";

		// Act
		var cardResult = Card.Create(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenTranslatedWordIsNull()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "Hello";
		string translatedWord = null!;

		// Act
		var cardResult = Card.Create(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenTranslatedWordIsEmpty()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "Hello";
		var translatedWord = string.Empty;

		// Act
		var cardResult = Card.Create(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenTranslatedWordIsWhitespace()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "Hello";
		var translatedWord = "   ";

		// Act
		var cardResult = Card.Create(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenNextReviewDateIsInThePast()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "Hello";
		var translatedWord = "Hola";
		var nextReviewDate = DateTime.UtcNow.AddDays(-1);

		// Act
		var cardResult = Card.Create(deckId, originalWord, translatedWord, 0, nextReviewDate, DateTime.UtcNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenPreviousCorrectReviewIsInTheFuture()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "Hello";
		var translatedWord = "Hola";
		var previousCorrectReview = DateTime.UtcNow.AddDays(1);

		// Act
		var cardResult = Card.Create(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), previousCorrectReview);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void Card_ReturnsFailure_WhenReviewStreakIsNegative()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "Hello";
		var translatedWord = "Hola";

		// Act
		var cardResult = Card.Create(deckId, originalWord, translatedWord, -1, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);

		// Assert
		Assert.False(cardResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, cardResult.Status);
	}

	[Fact]
	public void SetNextReviewDate_UpdatesNextReviewDate()
	{
		// Arrange
		var cardResult = Card.Create(Guid.NewGuid(), "Hello", "Hola", 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);
		Assert.True(cardResult.IsSuccess);
		var card = cardResult.Value;
		var newReviewDate = DateTime.UtcNow.AddDays(2);

		// Act
		card.SetNextReviewDate(newReviewDate);

		// Assert
		Assert.Equal(newReviewDate, card.NextReviewDate);
	}

	[Fact]
	public void SetNextReviewDate_ThrowsArgumentOutOfRangeException_WhenDateIsInThePast()
	{
		// Arrange
		var cardResult = Card.Create(Guid.NewGuid(), "Hello", "Hola", 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);
		Assert.True(cardResult.IsSuccess);
		var card = cardResult.Value;
		var pastDate = DateTime.UtcNow.AddDays(-1);

		// Act & Assert
		Assert.Throws<ArgumentOutOfRangeException>(() => card.SetNextReviewDate(pastDate));
	}

	[Fact]
	public void SetPreviousCorrectReview_UpdatesPreviousCorrectReview()
	{
		// Arrange
		var cardResult = Card.Create(Guid.NewGuid(), "Hello", "Hola", 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);
		Assert.True(cardResult.IsSuccess);
		var card = cardResult.Value;
		var newPreviousReviewDate = DateTime.UtcNow.AddDays(-1);

		// Act
		card.SetPreviousCorrectReview(newPreviousReviewDate);

		// Assert
		Assert.Equal(newPreviousReviewDate, card.PreviousCorrectReview);
	}

	[Fact]
	public void SetPreviousCorrectReview_ReturnsUnsuccessfulResult_WhenDateIsInTheFuture()
	{
		// Arrange
		var cardResult = Card.Create(Guid.NewGuid(), "Hello", "Hola", 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);
		Assert.True(cardResult.IsSuccess);
		var card = cardResult.Value;
		var futureDate = DateTime.UtcNow.AddDays(1);

		// Act & Assert
		Assert.Throws<ArgumentOutOfRangeException>(() => card.SetPreviousCorrectReview(futureDate));
	}

	[Fact]
	public void SetCorrectReviewStreak_UpdatesCorrectReviewStreak()
	{
		// Arrange
		var cardResult = Card.Create(Guid.NewGuid(), "Hello", "Hola", 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);
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
		var cardResult = Card.Create(Guid.NewGuid(), "Hello", "Hola", 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);
		Assert.True(cardResult.IsSuccess);
		var card = cardResult.Value;

		// Act & Assert
		Assert.False(card.SetCorrectReviewStreak(-1).IsSuccess);
	}

	public void Dispose()
	{
		Clock.Reset();
	}
}
