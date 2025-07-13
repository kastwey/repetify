using Repetify.Domain.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Extensions.Mappers;
using Repetify.Testing.Extensions;

namespace Repetify.Infrastructure.Persistence.EfCore.UnitTests.Extensions.Mappers;

public class CardExtensionsTests
{
	[Fact]
	public void ToEntity_ShouldMapCorrectly()
	{
		// Arrange
		var card = Card.Create(
			id: Guid.NewGuid(),
			deckId: Guid.NewGuid(),
			front: "Hola",
			back: "Hello",
			correctReviewStreak: 5,
			nextReviewDate: DateTime.UtcNow.AddDays(1),
			previousCorrectReview: DateTime.UtcNow.AddDays(-1)
		).AssertIsSuccess();

		// Act
		var entity = card.ToDataEntity();

		// Assert
		Assert.NotNull(entity);
		Assert.Equal(card.Id, entity.Id);
		Assert.Equal(card.DeckId, entity.DeckId);
		Assert.Equal(card.Front, entity.Front);
		Assert.Equal(card.Back, entity.Back);
		Assert.Equal(card.CorrectReviewStreak, entity.CorrectReviewStreak);
		Assert.Equal(card.Repetitions, entity.Repetitions);
		Assert.Equal(card.Interval, entity.Interval);
		Assert.Equal(card.EaseFactor, entity.EaseFactor);
		Assert.Equal(card.NextReviewDate, entity.NextReviewDate);
		Assert.Equal(card.PreviousCorrectReview, entity.PreviousCorrectReview);
	}

	[Fact]
	public void ToDomain_ShouldMapCorrectly()
	{
		// Arrange
		var cardEntity = new CardEntity
		{
			Id = Guid.NewGuid(),
			DeckId = Guid.NewGuid(),
			Front = "Bonjour",
			Back = "Hello",
			CorrectReviewStreak = 3,
			Repetitions = 4,
			Interval = 5,
			EaseFactor = 6,
			NextReviewDate = DateTime.UtcNow.AddDays(2),
			PreviousCorrectReview = DateTime.UtcNow.AddDays(-3)
		};

		// Act
		var domain = cardEntity.ToDomain().AssertIsSuccess();

		// Assert
		Assert.NotNull(domain);
		Assert.Equal(cardEntity.Id, domain.Id);
		Assert.Equal(cardEntity.Front, domain.Front);
		Assert.Equal(cardEntity.Back, domain.Back);
		Assert.Equal(cardEntity.CorrectReviewStreak, domain.CorrectReviewStreak);
		Assert.Equal(cardEntity.Repetitions, domain.Repetitions);
		Assert.Equal(cardEntity.EaseFactor, domain.EaseFactor);
		Assert.Equal(cardEntity.Interval, domain.Interval);
		Assert.Equal(cardEntity.NextReviewDate, domain.NextReviewDate);
		Assert.Equal(cardEntity.PreviousCorrectReview, domain.PreviousCorrectReview);
	}

	[Fact]
	public void ToEntity_ShouldThrowArgumentNullException_WhenCardIsNull()
	{
		// Arrange
		Card? nullCard = null;
		var deckId = Guid.NewGuid();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => nullCard!.ToDataEntity());
	}

	[Fact]
	public void ToDomain_ShouldThrowArgumentNullException_WhenCardEntityIsNull()
	{
		// Arrange
		CardEntity? nullEntity = null;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => nullEntity!.ToDomain());
	}


	[Fact]
	public void UpdateFromDomain_ShouldUpdateEntityCorrectly()
	{
		// Arrange  
		var cardEntity = new CardEntity
		{
			Id = Guid.NewGuid(),
			DeckId = Guid.NewGuid(),
			Front = "Hola",
			Back = "Hello",
			CorrectReviewStreak = 2,
			NextReviewDate = DateTime.UtcNow.AddDays(1),
			PreviousCorrectReview = DateTime.UtcNow.AddDays(-1)
		};

		var updatedCard = Card.Create(
			id: cardEntity.Id,
			deckId: cardEntity.DeckId,
			front: "Bonjour",
			back: "Hi",
			correctReviewStreak: 5,
			nextReviewDate: DateTime.UtcNow.AddDays(3),
			previousCorrectReview: DateTime.UtcNow.AddDays(-2)
		).AssertIsSuccess();

		// Act  
		cardEntity.UpdateFromDomain(updatedCard);

		// Assert  
		Assert.Equal(updatedCard.Id, cardEntity.Id);
		Assert.Equal(updatedCard.DeckId, cardEntity.DeckId);
		Assert.Equal(updatedCard.Front, cardEntity.Front);
		Assert.Equal(updatedCard.Back, cardEntity.Back);
		Assert.Equal(updatedCard.CorrectReviewStreak, cardEntity.CorrectReviewStreak);
		Assert.Equal(updatedCard.NextReviewDate, cardEntity.NextReviewDate);
		Assert.Equal(updatedCard.PreviousCorrectReview, cardEntity.PreviousCorrectReview);
	}

	[Theory]
	[InlineData(true, false)]
	[InlineData(false, true)]
	public void UpdateFromDomain_ShouldThrowArgumentNullException_WhenInputIsNull(bool dataEntityIsNull, bool domainEntityIsNull)
	{
		// Arrange
		CardEntity? cardEntity = dataEntityIsNull ? null : new CardEntity
		{
			Id = Guid.NewGuid(),
			DeckId = Guid.NewGuid(),
			Front = "Hola",
			Back = "Hello",
			CorrectReviewStreak = 2,
			NextReviewDate = DateTime.UtcNow.AddDays(1),
			PreviousCorrectReview = DateTime.UtcNow.AddDays(-1)
		};

		Card? updatedCard = domainEntityIsNull ? null : Card.Create(
			id: Guid.NewGuid(),
			deckId: Guid.NewGuid(),
			front: "Bonjour",
			back: "Hi",
			correctReviewStreak: 5,
			nextReviewDate: DateTime.UtcNow.AddDays(3),
			previousCorrectReview: DateTime.UtcNow.AddDays(-2)
		).AssertIsSuccess();

		// Act & Assert
		var exception = Assert.Throws<ArgumentNullException>(() => cardEntity!.UpdateFromDomain(updatedCard!));
		Assert.Equal(dataEntityIsNull ?  "cardEntity" : "card", exception.ParamName);
	}
}