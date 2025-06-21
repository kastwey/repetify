using Repetify.Domain.Entities;

namespace Repetify.Domain.UnitTests.Entities;

public class CardTests
{
	[Fact]
	public void Card_Initializes_WhenValidValuesAreProvided()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "Hello";
		var translatedWord = "Hola";

		// Act
		var card = new Card(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow);

		// Assert
		Assert.NotNull(card);
		Assert.NotEqual(Guid.Empty, card.Id);
		Assert.Equal(deckId, card.DeckId);
		Assert.Equal(originalWord, card.OriginalWord);
		Assert.Equal(translatedWord, card.TranslatedWord);
	}

	[Fact]
	public void Card_ThrowsArgumentNullException_WhenOriginalWordIsNull()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		string originalWord = null!;
		var translatedWord = "Hola";

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new Card(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow));
	}

	[Fact]
	public void Card_ThrowsArgumentException_WhenOriginalWordIsEmpty()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = string.Empty;
		var translatedWord = "Hola";

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new Card(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow));
	}

	[Fact]
	public void Card_ThrowsArgumentException_WhenOriginalWordIsWhitespace()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "   ";
		var translatedWord = "Hola";

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new Card(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow));
	}




	[Fact]
	public void Card_ThrowsArgumentNullException_WhenTranslatedWordIsNull()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "Hello";
		string translatedWord = null!;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new Card(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow));
	}

	[Fact]
	public void Card_ThrowsArgumentException_WhenTranslatedWordIsEmpty()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "Hello";
		var translatedWord = string.Empty;

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new Card(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow));
	}

	[Fact]
	public void Card_ThrowsArgumentException_WhenTranslatedWordIsWhitespace()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "Hello";
		var translatedWord = "   ";

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new Card(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), DateTime.UtcNow));
	}

	[Fact]
	public void Card_ThrowsArgumentOutOfRangeException_WhenNextReviewDateIsInThePast()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "Hello";
		var translatedWord = "Hola";
		var nextReviewDate = DateTime.UtcNow.AddDays(-1);
		// Act & Assert
		Assert.Throws<ArgumentOutOfRangeException>(() => new Card(deckId, originalWord, translatedWord, 0, nextReviewDate, DateTime.UtcNow));
	}

	[Fact]
	public void Card_ThrowsArgumentOutOfRangeException_WhenPreviousCorrectReviewIsInTheFuture()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "Hello";
		var translatedWord = "Hola";
		var previousCorrectReview = DateTime.UtcNow.AddDays(1);
		// Act & Assert
		Assert.Throws<ArgumentOutOfRangeException>(() => new Card(deckId, originalWord, translatedWord, 0, DateTime.UtcNow.AddDays(1), previousCorrectReview));
	}

	[Fact]
	public void Card_ThrowsArgumentOutOfRangeException_WhenReviewStreakIsNegative()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var originalWord = "Hello";
		var translatedWord = "Hola";

		// Act & Assert
		Assert.Throws<ArgumentOutOfRangeException>(() => new Card(deckId, originalWord, translatedWord, -1, DateTime.UtcNow.AddDays(1), DateTime.UtcNow));
	}

	[Fact]
	public void SetNextReviewDate_UpdatesNextReviewDate()
	{
		// Arrange
		var card = new Card(Guid.NewGuid(), Guid.NewGuid(), "Hello", "Hola");
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
		var card = new Card(Guid.NewGuid(), Guid.NewGuid(), "Hello", "Hola");
		var pastDate = DateTime.UtcNow.AddDays(-1);
		// Act & Assert
		Assert.Throws<ArgumentOutOfRangeException>(() => card.SetNextReviewDate(pastDate));
	}	

	[Fact]
	public void SetPreviousCorrectReview_UpdatesPreviousCorrectReview()
	{
		// Arrange
		var card = new Card(Guid.NewGuid(), Guid.NewGuid(), "Hello", "Hola");
		var newPreviousReviewDate = DateTime.UtcNow.AddDays(-1);

		// Act
		card.SetPreviousCorrectReview(newPreviousReviewDate);

		// Assert
		Assert.Equal(newPreviousReviewDate, card.PreviousCorrectReview);
	}

	[Fact]
	public void SetPreviousCorrectReview_ThrowsArgumentOutOfRangeException_WhenDateIsInTheFuture()
	{
		// Arrange
		var card = new Card(Guid.NewGuid(), Guid.NewGuid(), "Hello", "Hola");
		var futureDate = DateTime.UtcNow.AddDays(1);
		// Act & Assert
		Assert.Throws<ArgumentOutOfRangeException>(() => card.SetPreviousCorrectReview(futureDate));
	}

	[Fact]
	public void SetCorrectReviewStreak_UpdatesCorrectReviewStreak()
	{
		// Arrange
		var card = new Card(Guid.NewGuid(), Guid.NewGuid(), "Hello", "Hola");
		var newStreak = 5;

		// Act
		card.SetCorrectReviewStreak(newStreak);

		// Assert
		Assert.Equal(newStreak, card.CorrectReviewStreak);
	}

	[Fact]
	public void SetCorrectReviewStreak_ThrowsArgumentOutOfRangeException_WhenStreakIsNegative()
	{
		// Arrange
		var card = new Card(Guid.NewGuid(), Guid.NewGuid(), "Hello", "Hola");

		// Act & Assert
		Assert.Throws<ArgumentOutOfRangeException>(() => card.SetCorrectReviewStreak(-1));
	}
}
