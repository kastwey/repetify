using Repetify.Application.Dtos;
using Repetify.Application.Extensions.Mappers;
using Repetify.Domain.Entities;

using Xunit;

namespace Repetify.Application.UnitTests.Extensions.Mappers;

public class CardExtensionTests
{
	#region ToDto(Card)

	[Fact]
	public void ToDto_ShouldReturnCardDto_WhenCardIsValid()
	{
		// Arrange
		var card = new Card(
			id: Guid.NewGuid(),
			deckId: Guid.NewGuid(),
			originalWord: "Hello",
			translatedWord: "Hola",
			correctReviewStreak: 2,
			nextReviewDate: DateTime.UtcNow.AddDays(1),
			previousCorrectReview: DateTime.UtcNow.AddDays(-1)
		);

		// Act
		var result = card.ToDto();

		// Assert
		Assert.NotNull(result);
		Assert.Equal(card.Id, result.Id);
		Assert.Equal(card.DeckId, result.DeckId);
		Assert.Equal(card.OriginalWord, result.OriginalWord);
		Assert.Equal(card.TranslatedWord, result.TranslatedWord);
		Assert.Equal(card.CorrectReviewStreak, result.CorrectReviewStreak);
		Assert.Equal(card.NextReviewDate, result.NextReviewDate);
		Assert.Equal(card.PreviousCorrectReview, result.PreviousCorrectReview);
	}

	[Fact]
	public void ToDto_ShouldThrowArgumentNullException_WhenCardIsNull()
	{
		// Arrange
		Card card = null!;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => card.ToDto());
	}

	#endregion

	#region ToDtoList(IEnumerable<Card>)

	[Fact]
	public void ToDtoList_ShouldReturnListOfCardDto_WhenListIsNotEmpty()
	{
		// Arrange
		var cards = new List<Card>
		{
			new(Guid.NewGuid(), Guid.NewGuid(), "Word1", "Traduccion1", 1, DateTime.UtcNow.AddDays(2), DateTime.UtcNow),
			new(Guid.NewGuid(), Guid.NewGuid(), "Word2", "Traduccion2", 0, DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddDays(-1))
		};

		// Act
		var result = cards.ToDtoList().ToList();

		// Assert
		Assert.NotNull(result);
		Assert.Equal(cards.Count, result.Count);

		for (int i = 0; i < cards.Count; i++)
		{
			Assert.Equal(cards[i].Id, result[i].Id);
			Assert.Equal(cards[i].DeckId, result[i].DeckId);
			Assert.Equal(cards[i].OriginalWord, result[i].OriginalWord);
			Assert.Equal(cards[i].TranslatedWord, result[i].TranslatedWord);
			Assert.Equal(cards[i].CorrectReviewStreak, result[i].CorrectReviewStreak);
			Assert.Equal(cards[i].NextReviewDate, result[i].NextReviewDate);
			Assert.Equal(cards[i].PreviousCorrectReview, result[i].PreviousCorrectReview);
		}
	}

	[Fact]
	public void ToDtoList_ShouldReturnEmptyList_WhenListIsEmpty()
	{
		// Arrange
		var emptyCards = new List<Card>();

		// Act
		var result = emptyCards.ToDtoList().ToList();

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result);
	}

	[Fact]
	public void ToDtoList_ShouldThrowArgumentNull_WhenListIsNull()
	{
		// Arrange
		IEnumerable<Card> cards = null!;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => cards.ToDtoList());
	}

	#endregion

	#region ToEntity(AddOrUpdateCardDto, Guid, Guid?)

	[Fact]
	public void ToEntity_ShouldReturnCard_WhenAddOrUpdateCardDtoIsValid()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var cardId = Guid.NewGuid();
		var dto = new AddOrUpdateCardDto { OriginalWord = "Hola", TranslatedWord = "Hello" };

		// Act
		var result = dto.ToEntity(deckId, cardId);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(deckId, result.DeckId);
		Assert.Equal(cardId, result.Id);
		Assert.Equal(dto.OriginalWord, result.OriginalWord);
		Assert.Equal(dto.TranslatedWord, result.TranslatedWord);
	}

	[Fact]
	public void ToEntity_ShouldThrowArgumentNullException_WhenAddOrUpdateCardDtoIsNull()
	{
		// Arrange
		AddOrUpdateCardDto dto = null!;
		var deckId = Guid.NewGuid();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => dto.ToEntity(deckId, Guid.NewGuid()));
	}

	#endregion
}
