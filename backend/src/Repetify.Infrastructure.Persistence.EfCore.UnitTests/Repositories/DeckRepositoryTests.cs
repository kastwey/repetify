using Microsoft.EntityFrameworkCore;

using Repetify.Crosscutting;
using Repetify.Domain.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Context;
using Repetify.Infrastructure.Persistence.EfCore.Repositories;
using Repetify.Infrastructure.Persistence.EfCore.UnitTests.Helpers;
using Repetify.Testing.Extensions;


namespace Repetify.Infrastructure.Persistence.EfCore.UnitTests.Repositories;

public class DeckRepositoryTests
{
	[Fact]
	public async Task AddDeckAsync_ShouldAddDeckSuccessfully()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = Deck.Create(Guid.NewGuid(), "Test Deck", "Test Description", Guid.NewGuid()).AssertIsSuccess();

		// Act
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();
		
		var storedDeck = await dbContext.Decks.FirstOrDefaultAsync(d => d.Id == deck.Id);

		// Assert
		Assert.NotNull(storedDeck);
		Assert.Equal(deck.Name, storedDeck.Name);
	}

	[Fact]
	public async Task EditDeckAsync_ShouldUpdateDeckSuccessfully()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = Deck.Create(Guid.NewGuid(), "Original Name", "Original Description", Guid.NewGuid()).AssertIsSuccess();
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Act
		deck = Deck.Create(deck.Id, "Updated Name", "Updated Description", deck.UserId).AssertIsSuccess();
		(await repository.UpdateDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Assert
		var updatedDeck = await dbContext.Decks.FirstOrDefaultAsync(d => d.Id == deck.Id);
		Assert.NotNull(updatedDeck);
		Assert.Equal("Updated Name", updatedDeck.Name);
		Assert.Equal("Updated Description", updatedDeck.Description);
	}

	[Fact]
	public async Task DeleteDeckAsync_ShouldReturnTrue_WhenDeckExists()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = Deck.Create(Guid.NewGuid(), "Deck to delete", "Test", Guid.NewGuid()).AssertIsSuccess();
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Act
		var result = await repository.DeleteDeckAsync(deck.Id);
		await repository.SaveChangesAsync();

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Null(await dbContext.Decks.FirstOrDefaultAsync(d => d.Id == deck.Id));
	}

	[Fact]
	public async Task DeleteDeckAsync_ShouldReturnFalse_WhenDeckDoesNotExist()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		// Act
		var result = await repository.DeleteDeckAsync(Guid.NewGuid());

		// Assert
		Assert.False(result.IsSuccess);
	}


	[Fact]
	public async Task GetDeckByIdAsync_ShouldReturnDeck_WhenExists()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = Deck.Create(Guid.NewGuid(), "Spanish Deck", "Learning Spanish", Guid.NewGuid()).AssertIsSuccess();
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Act
		var result = await repository.GetDeckByIdAsync(deck.Id);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsSuccess);
		Assert.Equal(deck.Name, result.Value!.Name);
	}

	[Fact]
	public async Task GetDeckByIdAsync_ShouldReturnNull_WhenDeckDoesNotExist()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		// Act
		var result = await repository.GetDeckByIdAsync(Guid.NewGuid());

		// Assert
		Assert.Equal(ResultStatus.NotFound, result.Status);
		Assert.Null(result.Value);
	}

	[Fact]
	public async Task GetDecksByUserIdAsync_ShouldReturnDecks_WhenUserHasDecks()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var userId = Guid.NewGuid();
		var deck1 = Deck.Create(Guid.NewGuid(), "Deck 1", "Description 1", userId).AssertIsSuccess();
		var deck2 = Deck.Create(Guid.NewGuid(), "Deck 2", "Description 2", userId).AssertIsSuccess();

		(await repository.AddDeckAsync(deck1)).AssertIsSuccess();
		(await repository.AddDeckAsync(deck2)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Act
		var decksResult = await repository.GetDecksByUserIdAsync(userId);

		// Assert
		Assert.True(decksResult.IsSuccess);

		Assert.NotNull(decksResult);
		Assert.Equal(2, decksResult.Value!.Count());
		Assert.Contains(decksResult.Value!, d => d.Name == "Deck 1");
		Assert.Contains(decksResult.Value!, d => d.Name == "Deck 2");
	}

	[Fact]
	public async Task GetDecksByUserIdAsync_ShouldReturnEmptyList_WhenUserHasNoDecks()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var userId = Guid.NewGuid();

		// Act
		var decksResult = await repository.GetDecksByUserIdAsync(userId);

		// Assert
		Assert.NotNull(decksResult);
		Assert.True(decksResult.IsSuccess);
		Assert.Empty(decksResult.Value!);
	}

	[Fact]
	public async Task GetDecksByUserIdAsync_ShouldReturnOnlyUserDecks()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var userId1 = Guid.NewGuid();
		var userId2 = Guid.NewGuid();

		var deck1 = Deck.Create(Guid.NewGuid(), "Deck 1", "Description 1", userId1).AssertIsSuccess();
		var deck2 = Deck.Create(Guid.NewGuid(), "Deck 2", "Description 2", userId2).AssertIsSuccess();

		(await repository.AddDeckAsync(deck1)).AssertIsSuccess();
		(await repository.AddDeckAsync(deck2)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Act
		var decksResult = await repository.GetDecksByUserIdAsync(userId1);

		// Assert
		Assert.NotNull(decksResult);
		Assert.True(decksResult.IsSuccess);
		Assert.Single(decksResult.Value!);
		Assert.Equal("Deck 1", decksResult.Value!.First().Name);
		Assert.DoesNotContain(decksResult.Value!, d => d.UserId == userId2);
	}

	[Fact]
	public async Task GetDecksByUserIdAsync_ShouldHandleMultipleDecksPerUser()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var userId = Guid.NewGuid();

		var decks = new List<Deck>
		{
			Deck.Create(Guid.NewGuid(), "Deck A", "Desc A", userId).AssertIsSuccess(),
			Deck.Create(Guid.NewGuid(), "Deck B", "Desc B", userId).AssertIsSuccess(),
			Deck.Create(Guid.NewGuid(), "Deck C", "Desc C", userId).AssertIsSuccess()
		};

		foreach (var deck in decks)
		{
			(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		}
		await repository.SaveChangesAsync();

		// Act
		var resultDecks = await repository.GetDecksByUserIdAsync(userId);

		// Assert
		Assert.NotNull(resultDecks);
		Assert.Equal(3, resultDecks.Value!.Count());
		Assert.Contains(resultDecks.Value!, d => d.Name == "Deck A");
		Assert.Contains(resultDecks.Value!, d => d.Name == "Deck B");
		Assert.Contains(resultDecks.Value!, d => d.Name == "Deck C");
	}

	[Fact]
	public async Task GetCardsAsync_ShouldReturnPaginatedResults()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = Deck.Create(Guid.NewGuid(), "Test Deck", "Test Description", Guid.NewGuid()).AssertIsSuccess();
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		var card1 = Card.Create(Guid.NewGuid(), deck.Id, "Word1", "Translation1").AssertIsSuccess();
		var card2 = Card.Create(Guid.NewGuid(), deck.Id, "Word2", "Translation2").AssertIsSuccess();

		(await repository.AddCardAsync(card1)).AssertIsSuccess();
		(await repository.AddCardAsync(card2)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Act
		var cardsResult = await repository.GetCardsAsync(deck.Id, page: 1, pageSize: 1);

		// Assert
		Assert.True(cardsResult.IsSuccess);
		Assert.Single(cardsResult.Value!);
		Assert.Equal("Word1", cardsResult.Value!.First().Front);
	}

	[Fact]
	public async Task GetCardCountAsync_ShouldReturnCorrectCount()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = Deck.Create(Guid.NewGuid(), "Test Deck", "Test Description", Guid.NewGuid()).AssertIsSuccess();
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		(await repository.AddCardAsync(Card.Create(Guid.NewGuid(), deck.Id, "Word1", "Translation1").AssertIsSuccess())).AssertIsSuccess();
		(await repository.AddCardAsync(Card.Create(Guid.NewGuid(), deck.Id, "Word2", "Translation2").AssertIsSuccess())).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Act
		var countResult = await repository.GetCardCountAsync(deck.Id);
		(countResult).AssertIsSuccess();

		// Assert
		Assert.Equal(2, countResult.Value);
	}

	[Fact]
	public async Task AddCardAsync_ShouldAddCardToDeck()
	{
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = Deck.Create(Guid.NewGuid(), "Vocabulary", "Spanish words", Guid.NewGuid()).AssertIsSuccess();
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		var card = Card.Create(Guid.NewGuid(), deck.Id, "Hola", "Hello").AssertIsSuccess();

		(await repository.AddCardAsync(card)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		var storedCard = await dbContext.Cards.FirstOrDefaultAsync(c => c.Front == "Hola");
		Assert.NotNull(storedCard);
		Assert.Equal("Hello", storedCard.Back);
	}

	[Fact]
	public async Task EditCardAsync_ShouldUpdateCardSuccessfully()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = Deck.Create(Guid.NewGuid(), "Test Deck", "Test Description", Guid.NewGuid()).AssertIsSuccess();
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		var card = Card.Create(Guid.NewGuid(), deck.Id, "Original Word", "Original Translation").AssertIsSuccess();
		(await repository.AddCardAsync(card)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Act
		card = Card.Create(card.Id, card.DeckId, "Updated Word", "Updated Translation", card.CorrectReviewStreak, card.Repetitions, card.EaseFactor,  card.Interval, card.NextReviewDate, card.PreviousCorrectReview).AssertIsSuccess();
		(await repository.UpdateCardAsync(card)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Assert
		var updatedCard = await dbContext.Cards.FirstOrDefaultAsync(c => c.Id == card.Id);
		Assert.NotNull(updatedCard);
		Assert.Equal("Updated Word", updatedCard.Front);
		Assert.Equal("Updated Translation", updatedCard.Back);
	}

	[Fact]
	public async Task DeleteCardAsync_ShouldReturnTrue_WhenCardExists()
	{
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = Deck.Create(Guid.NewGuid(), "Languages", "Learning French", Guid.NewGuid()).AssertIsSuccess();
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		var card = Card.Create(Guid.NewGuid(), (Guid)deck.Id, "Chat", "Cat").AssertIsSuccess();
		(await repository.AddCardAsync(card)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		var result = await repository.DeleteCardAsync(deck.Id, card.Id);
		await repository.SaveChangesAsync();

		Assert.True(result.IsSuccess);
		var deletedCard = await dbContext.Cards.FirstOrDefaultAsync(c => c.Id == card.Id);
		Assert.Null(deletedCard);
	}

	[Fact]
	public async Task DeleteCardAsync_ShouldReturnFalse_WhenCardDoesNotExist()
	{
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = Deck.Create(Guid.NewGuid(), "Practice", "Deck for tests", Guid.NewGuid()).AssertIsSuccess();
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		var result = await repository.DeleteCardAsync(deck.Id, Guid.NewGuid());

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task GetCardByIdAsync_ShouldReturnCard_WhenExists()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = Deck.Create(Guid.NewGuid(), "Words", "Deck description", Guid.NewGuid()).AssertIsSuccess();
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Act
		var card = Card.Create(Guid.NewGuid(), (Guid)deck.Id, "Auto", "Car").AssertIsSuccess();
		(await repository.AddCardAsync(card)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		var retrievedCardResult = await repository.GetCardByIdAsync(deck.Id, card.Id);

		// Assert
		Assert.NotNull(retrievedCardResult);
		Assert.True(retrievedCardResult.IsSuccess);
		Assert.Equal("Auto", retrievedCardResult.Value!.Front);
	}

	[Fact]
	public async Task DeckNameExistsForUser_ShouldReturnTrue_WhenDeckNameExistsForUser()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var userId = Guid.NewGuid();
		var name = "should be unique";
		var deck = Deck.Create(Guid.NewGuid(), name, "Description", userId).AssertIsSuccess();
		var deckToTest = Deck.Create(Guid.NewGuid(), name, "Description", userId).AssertIsSuccess();
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Act
		var existsResult = await repository.DeckNameExistsForUserAsync(deckToTest.Id, name, userId);

		// Assert
		Assert.True(existsResult.IsSuccess);
		Assert.True(existsResult.Value);
	}

	[Fact]
	public async Task DeckNameExistsForUser_ShouldReturnFalse_WhenDeckNameExistsForOtherUser()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var name = "should be unique";
		var deck = Deck.Create(Guid.NewGuid(), name, "Description", Guid.NewGuid()).AssertIsSuccess();
		var deckToTest = Deck.Create(Guid.NewGuid(), name, "Description", Guid.NewGuid()).AssertIsSuccess();
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Act
		var existsResult = await repository.DeckNameExistsForUserAsync(deckToTest.Id, name, deckToTest.UserId);

		// Assert
		Assert.True(existsResult.IsSuccess);
		Assert.False(existsResult.Value);
	}

	[Fact]
	public async Task DeckNameExistsForUser_ShouldReturnFalse_WhenDeckNameDoesNotExistForUser()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var userId = Guid.NewGuid();
		var repository = new DeckRepository(dbContext);
		var deck = Deck.Create(Guid.NewGuid(), "Existing Deck", "Description", userId).AssertIsSuccess();
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Act
		var existsResult = await repository.DeckNameExistsForUserAsync(deck.Id, "Non-Existing Deck", userId);

		// Assert
		Assert.True(existsResult.IsSuccess);
		Assert.False(existsResult.Value);
	}

	[Fact]
	public async Task DeckNameExistsForUser_ShouldReturnFalse_WhenDeckNameExistsForSameDeck()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var userId = Guid.NewGuid();
		var deck = Deck.Create(Guid.NewGuid(), "Existing Deck", "Description", userId).AssertIsSuccess();
		(await repository.AddDeckAsync(deck)).AssertIsSuccess();
		await repository.SaveChangesAsync();

		// Act
		var existsResult = await repository.DeckNameExistsForUserAsync(deck.Id, deck.Name, userId);

		// Assert
		Assert.True(existsResult.IsSuccess);
		Assert.False(existsResult.Value);
	}

}
