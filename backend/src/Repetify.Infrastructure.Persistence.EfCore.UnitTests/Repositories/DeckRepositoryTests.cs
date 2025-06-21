using Microsoft.EntityFrameworkCore;

using Repetify.Crosscutting;
using Repetify.Domain.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Context;
using Repetify.Infrastructure.Persistence.EfCore.Repositories;
using Repetify.Infrastructure.Persistence.EfCore.UnitTests.Helpers;


namespace Repetify.Infrastructure.Persistence.EfCore.UnitTests.Repositories;

public class DeckRepositoryTests
{
	[Fact]
	public async Task AddDeckAsync_ShouldAddDeckSuccessfully()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = new Deck(Guid.NewGuid(), "Test Deck", "Test Description", Guid.NewGuid(), "EN", "ES");

		// Act
		EnsureSuccess(await repository.AddDeckAsync(deck));
		await repository.SaveChangesAsync();
		
		var storedDeck = await dbContext.Decks.FirstOrDefaultAsync(d => d.Id == deck.Id);

		// Assert
		Assert.NotNull(storedDeck);
		Assert.Equal(deck.Name, storedDeck.Name);
		Assert.Equal(deck.OriginalLanguage, storedDeck.OriginalLanguage);
	}

	[Fact]
	public async Task EditDeckAsync_ShouldUpdateDeckSuccessfully()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = new Deck(Guid.NewGuid(), "Original Name", "Original Description", Guid.NewGuid(), "EN", "ES");
		EnsureSuccess(await repository.AddDeckAsync(deck));
		await repository.SaveChangesAsync();

		// Act
		deck = new Deck(deck.Id, "Updated Name", "Updated Description", deck.UserId, deck.OriginalLanguage, deck.TranslatedLanguage);
		EnsureSuccess(await repository.UpdateDeckAsync(deck));
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

		var deck = new Deck(Guid.NewGuid(), "Deck to delete", "Test", Guid.NewGuid(), "EN", "FR");
		EnsureSuccess(await repository.AddDeckAsync(deck));
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

		var deck = new Deck(Guid.NewGuid(), "Spanish Deck", "Learning Spanish", Guid.NewGuid(), "EN", "ES");
		EnsureSuccess(await repository.AddDeckAsync(deck));
		await repository.SaveChangesAsync();

		// Act
		var result = await repository.GetDeckByIdAsync(deck.Id);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsSuccess);
		Assert.Equal(deck.Name, result.Value!.Name);
		Assert.Equal(deck.OriginalLanguage, result.Value!.OriginalLanguage);
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
		var deck1 = new Deck(Guid.NewGuid(), "Deck 1", "Description 1", userId, "EN", "FR");
		var deck2 = new Deck(Guid.NewGuid(), "Deck 2", "Description 2", userId, "EN", "ES");

		EnsureSuccess(await repository.AddDeckAsync(deck1));
		EnsureSuccess(await repository.AddDeckAsync(deck2));
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

		var deck1 = new Deck(Guid.NewGuid(), "Deck 1", "Description 1", userId1, "EN", "FR");
		var deck2 = new Deck(Guid.NewGuid(), "Deck 2", "Description 2", userId2, "EN", "ES");

		EnsureSuccess(await repository.AddDeckAsync(deck1));
		EnsureSuccess(await repository.AddDeckAsync(deck2));
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
			new Deck(Guid.NewGuid(), "Deck A", "Desc A", userId, "EN", "DE"),
			new Deck(Guid.NewGuid(), "Deck B", "Desc B", userId, "EN", "IT"),
			new Deck(Guid.NewGuid(), "Deck C", "Desc C", userId, "EN", "PT")
		};

		foreach (var deck in decks)
		{
			EnsureSuccess(await repository.AddDeckAsync(deck));
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

		var deck = new Deck(Guid.NewGuid(), "Test Deck", "Test Description", Guid.NewGuid(), "EN", "ES");
		EnsureSuccess(await repository.AddDeckAsync(deck));
		await repository.SaveChangesAsync();

		var card1 = new Card(Guid.NewGuid(), deck.Id, "Word1", "Translation1");
		var card2 = new Card(Guid.NewGuid(), deck.Id, "Word2", "Translation2");

		EnsureSuccess(await repository.AddCardAsync(card1));
		EnsureSuccess(await repository.AddCardAsync(card2));
		await repository.SaveChangesAsync();

		// Act
		var cardsResult = await repository.GetCardsAsync(deck.Id, page: 1, pageSize: 1);

		// Assert
		Assert.True(cardsResult.IsSuccess);
		Assert.Single(cardsResult.Value!);
		Assert.Equal("Word1", cardsResult.Value!.First().OriginalWord);
	}

	[Fact]
	public async Task GetCardCountAsync_ShouldReturnCorrectCount()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = new Deck(Guid.NewGuid(), "Test Deck", "Test Description", Guid.NewGuid(), "EN", "ES");
		EnsureSuccess(await repository.AddDeckAsync(deck));
		await repository.SaveChangesAsync();

		EnsureSuccess(await repository.AddCardAsync(new Card(Guid.NewGuid(), deck.Id, "Word1", "Translation1")));
		EnsureSuccess(await repository.AddCardAsync(new Card(Guid.NewGuid(), deck.Id, "Word2", "Translation2")));
		await repository.SaveChangesAsync();

		// Act
		var countResult = await repository.GetCardCountAsync(deck.Id);
		EnsureSuccess(countResult);

		// Assert
		Assert.Equal(2, countResult.Value);
	}

	[Fact]
	public async Task AddCardAsync_ShouldAddCardToDeck()
	{
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = new Deck(Guid.NewGuid(), "Vocabulary", "Spanish words", Guid.NewGuid(), "EN", "ES");
		EnsureSuccess(await repository.AddDeckAsync(deck));
		await repository.SaveChangesAsync();

		var card = new Card(Guid.NewGuid(), deck.Id, "Hola", "Hello");

		EnsureSuccess(await repository.AddCardAsync(card));
		await repository.SaveChangesAsync();

		var storedCard = await dbContext.Cards.FirstOrDefaultAsync(c => c.OriginalWord == "Hola");
		Assert.NotNull(storedCard);
		Assert.Equal("Hello", storedCard.TranslatedWord);
	}

	[Fact]
	public async Task EditCardAsync_ShouldUpdateCardSuccessfully()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = new Deck(Guid.NewGuid(), "Test Deck", "Test Description", Guid.NewGuid(), "EN", "ES");
		EnsureSuccess(await repository.AddDeckAsync(deck));
		await repository.SaveChangesAsync();

		var card = new Card(Guid.NewGuid(), deck.Id, "Original Word", "Original Translation");
		EnsureSuccess(await repository.AddCardAsync(card));
		await repository.SaveChangesAsync();

		// Act
		card = new Card(card.Id, card.DeckId, "Updated Word", "Updated Translation", card.CorrectReviewStreak, card.NextReviewDate, card.PreviousCorrectReview);
		EnsureSuccess(await repository.UpdateCardAsync(card));
		await repository.SaveChangesAsync();

		// Assert
		var updatedCard = await dbContext.Cards.FirstOrDefaultAsync(c => c.Id == card.Id);
		Assert.NotNull(updatedCard);
		Assert.Equal("Updated Word", updatedCard.OriginalWord);
		Assert.Equal("Updated Translation", updatedCard.TranslatedWord);
	}

	[Fact]
	public async Task DeleteCardAsync_ShouldReturnTrue_WhenCardExists()
	{
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var deck = new Deck(Guid.NewGuid(), "Languages", "Learning French", Guid.NewGuid(), "EN", "FR");
		EnsureSuccess(await repository.AddDeckAsync(deck));
		await repository.SaveChangesAsync();

		var card = new Card(Guid.NewGuid(), deck.Id, "Chat", "Cat");
		EnsureSuccess(await repository.AddCardAsync(card));
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

		var deck = new Deck(Guid.NewGuid(), "Practice", "Deck for tests", Guid.NewGuid(), "EN", "IT");
		EnsureSuccess(await repository.AddDeckAsync(deck));
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

		var deck = new Deck(Guid.NewGuid(), "Words", "Deck description", Guid.NewGuid(), "EN", "DE");
		EnsureSuccess(await repository.AddDeckAsync(deck));
		await repository.SaveChangesAsync();

		// Act
		var card = new Card(Guid.NewGuid(), deck.Id, "Auto", "Car");
		EnsureSuccess(await repository.AddCardAsync(card));
		await repository.SaveChangesAsync();

		var retrievedCardResult = await repository.GetCardByIdAsync(deck.Id, card.Id);

		// Assert
		Assert.NotNull(retrievedCardResult);
		Assert.True(retrievedCardResult.IsSuccess);
		Assert.Equal("Auto", retrievedCardResult.Value!.OriginalWord);
	}

	[Fact]
	public async Task DeckNameExistsForUser_ShouldReturnTrue_WhenDeckNameExistsForUser()
	{
		// Arrange
		using var dbContext = TestsHelper.CreateInMemoryDbContext();
		var repository = new DeckRepository(dbContext);

		var userId = Guid.NewGuid();
		var name = "should be unique";
		var deck = new Deck(Guid.NewGuid(), name, "Description", userId, "EN", "ES");
		var deckToTest = new Deck(Guid.NewGuid(), name, "Description", userId, "EN", "ES");
		EnsureSuccess(await repository.AddDeckAsync(deck));
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
		var deck = new Deck(Guid.NewGuid(), name, "Description", Guid.NewGuid(), "EN", "ES");
		var deckToTest = new Deck(Guid.NewGuid(), name, "Description", Guid.NewGuid(), "EN", "ES");
		EnsureSuccess(await repository.AddDeckAsync(deck));
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
		var deck = new Deck(Guid.NewGuid(), "Existing Deck", "Description", userId, "EN", "ES");
		EnsureSuccess(await repository.AddDeckAsync(deck));
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
		var deck = new Deck(Guid.NewGuid(), "Existing Deck", "Description", userId, "EN", "ES");
		EnsureSuccess(await repository.AddDeckAsync(deck));
		await repository.SaveChangesAsync();

		// Act
		var existsResult = await repository.DeckNameExistsForUserAsync(deck.Id, deck.Name, userId);

		// Assert
		Assert.True(existsResult.IsSuccess);
		Assert.False(existsResult.Value);
	}

	private static void EnsureSuccess(IResult result)
	{
		Assert.True(result.IsSuccess, result.ErrorMessage);
	}
}
