using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using Repetify.Application.Abstractions.Services;
using Repetify.Application.Dtos;
using Repetify.Application.Services;
using Repetify.Crosscutting;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Abstractions.Services;
using Repetify.Domain.Entities;

using Xunit;

namespace Repetify.Application.UnitTests.Services;

public class DeckAppServiceTests
{
	private readonly Mock<IDeckValidator> _deckValidatorMock = new();
	private readonly Mock<ICardReviewService> _cardReviewServiceMock = new();
	private readonly Mock<IDeckRepository> _deckRepositoryMock = new();
	private readonly DeckAppService _service;

	private readonly Guid _userId = Guid.NewGuid();
	private readonly Guid _deckId = Guid.NewGuid();
	private readonly Guid _cardId = Guid.NewGuid();

	public DeckAppServiceTests()
	{
		_service = new DeckAppService(_deckValidatorMock.Object, _cardReviewServiceMock.Object, _deckRepositoryMock.Object);
	}

	[Fact]
	public async Task AddDeckAsync_ReturnsSuccess_WhenValid()
	{
		var dto = new AddOrUpdateDeckDto { Name = "Deck", OriginalLanguage = "en", TranslatedLanguage = "es" };
		_deckValidatorMock.Setup(v => v.EnsureIsValid(It.IsAny<Deck>())).ReturnsAsync(ResultFactory.Success());
		_deckRepositoryMock.Setup(r => r.AddDeckAsync(It.IsAny<Deck>())).ReturnsAsync(ResultFactory.Success());
		_deckRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

		var result = await _service.AddDeckAsync(dto, _userId);

		Assert.True(result.IsSuccess);
		Assert.NotEqual(Guid.Empty, result.Value);
	}

	[Fact]
	public async Task AddDeckAsync_ReturnsFailure_WhenValidationFails()
	{
		var dto = new AddOrUpdateDeckDto { Name = "Deck", OriginalLanguage = "en", TranslatedLanguage = "es" };
		_deckValidatorMock.Setup(v => v.EnsureIsValid(It.IsAny<Deck>())).ReturnsAsync(ResultFactory.InvalidArgument("error"));

		var result = await _service.AddDeckAsync(dto, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.InvalidArguments, result.Status);
	}

	[Fact]
	public async Task AddDeckAsync_ReturnsFailure_WhenRepositoryFails()
	{
		var dto = new AddOrUpdateDeckDto { Name = "Deck", OriginalLanguage = "en", TranslatedLanguage = "es" };
		_deckValidatorMock.Setup(v => v.EnsureIsValid(It.IsAny<Deck>())).ReturnsAsync(ResultFactory.Success());
		_deckRepositoryMock.Setup(r => r.AddDeckAsync(It.IsAny<Deck>())).ReturnsAsync(ResultFactory.Conflict("error"));

		var result = await _service.AddDeckAsync(dto, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.Conflict, result.Status);
	}

	[Fact]
	public async Task UpdateDeckAsync_ReturnsSuccess_WhenValid()
	{
		var dto = new AddOrUpdateDeckDto { Name = "Deck", OriginalLanguage = "en", TranslatedLanguage = "es" };
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckValidatorMock.Setup(v => v.EnsureIsValid(It.IsAny<Deck>())).ReturnsAsync(ResultFactory.Success());
		_deckRepositoryMock.Setup(r => r.UpdateDeckAsync(It.IsAny<Deck>())).ReturnsAsync(ResultFactory.Success());
		_deckRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

		var result = await _service.UpdateDeckAsync(_deckId, dto, _userId);

		Assert.True(result.IsSuccess);
	}

	[Fact]
	public async Task UpdateDeckAsync_ReturnsFailure_WhenNoPermission()
	{
		var dto = new AddOrUpdateDeckDto { Name = "Deck", OriginalLanguage = "en", TranslatedLanguage = "es" };
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck(Guid.NewGuid())));

		var result = await _service.UpdateDeckAsync(_deckId, dto, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task UpdateDeckAsync_ReturnsFailure_WhenValidationFails()
	{
		var dto = new AddOrUpdateDeckDto { Name = "Deck", OriginalLanguage = "en", TranslatedLanguage = "es" };
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckValidatorMock.Setup(v => v.EnsureIsValid(It.IsAny<Deck>())).ReturnsAsync(ResultFactory.InvalidArgument("error"));

		var result = await _service.UpdateDeckAsync(_deckId, dto, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.InvalidArguments, result.Status);
	}

	[Fact]
	public async Task DeleteDeckAsync_ReturnsSuccess_WhenValid()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.DeleteDeckAsync(_deckId)).ReturnsAsync(ResultFactory.Success());
		_deckRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

		var result = await _service.DeleteDeckAsync(_deckId, _userId);

		Assert.True(result.IsSuccess);
	}

	[Fact]
	public async Task DeleteDeckAsync_ReturnsFailure_WhenNoPermission()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck(Guid.NewGuid())));

		var result = await _service.DeleteDeckAsync(_deckId, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task DeleteDeckAsync_ReturnsFailure_WhenRepositoryFails()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.DeleteDeckAsync(_deckId)).ReturnsAsync(ResultFactory.Conflict("error"));

		var result = await _service.DeleteDeckAsync(_deckId, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.Conflict, result.Status);
	}

	[Fact]
	public async Task GetDeckByIdAsync_ReturnsSuccess_WhenValid()
	{
		var deck = CreateDeck();
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(deck));

		var result = await _service.GetDeckByIdAsync(_deckId, _userId);

		Assert.True(result.IsSuccess);
		Assert.Equal(deck.Id, result.Value.Id);
	}

	[Fact]
	public async Task GetDeckByIdAsync_ReturnsFailure_WhenNoPermission()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck(Guid.NewGuid())));

		var result = await _service.GetDeckByIdAsync(_deckId, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task GetDeckByIdAsync_ReturnsFailure_WhenNotFound()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.NotFound<Deck>("not found"));

		var result = await _service.GetDeckByIdAsync(_deckId, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task GetUserDecksAsync_ReturnsSuccess_WhenValid()
	{
		var decks = new List<Deck> { CreateDeck() };
		_deckRepositoryMock.Setup(r => r.GetDecksByUserIdAsync(_userId)).ReturnsAsync(ResultFactory.Success(decks.AsEnumerable()));

		var result = await _service.GetUserDecksAsync(_userId);

		Assert.True(result.IsSuccess);
		Assert.Single(result.Value);
	}

	[Fact]
	public async Task GetUserDecksAsync_ReturnsFailure_WhenRepositoryFails()
	{
		_deckRepositoryMock.Setup(r => r.GetDecksByUserIdAsync(_userId)).ReturnsAsync(ResultFactory.NotFound<IEnumerable<Deck>>("not found"));

		var result = await _service.GetUserDecksAsync(_userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task AddCardAsync_ReturnsSuccess_WhenValid()
	{
		var dto = new AddOrUpdateCardDto { OriginalWord = "hello", TranslatedWord = "hola" };
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.AddCardAsync(It.IsAny<Card>())).ReturnsAsync(ResultFactory.Success());
		_deckRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

		var result = await _service.AddCardAsync(dto, _deckId, _userId);

		Assert.True(result.IsSuccess);
		Assert.NotEqual(Guid.Empty, result.Value);
	}

	[Fact]
	public async Task AddCardAsync_ReturnsFailure_WhenNoPermission()
	{
		var dto = new AddOrUpdateCardDto { OriginalWord = "hello", TranslatedWord = "hola" };
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck(Guid.NewGuid())));

		var result = await _service.AddCardAsync(dto, _deckId, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task AddCardAsync_ReturnsFailure_WhenRepositoryFails()
	{
		var dto = new AddOrUpdateCardDto { OriginalWord = "hello", TranslatedWord = "hola" };
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.AddCardAsync(It.IsAny<Card>())).ReturnsAsync(ResultFactory.Conflict("error"));

		var result = await _service.AddCardAsync(dto, _deckId, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.Conflict, result.Status);
	}

	[Fact]
	public async Task UpdateCardAsync_ReturnsSuccess_WhenValid()
	{
		var dto = new AddOrUpdateCardDto { OriginalWord = "hello", TranslatedWord = "hola" };
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.UpdateCardAsync(It.IsAny<Card>())).ReturnsAsync(ResultFactory.Success());
		_deckRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

		var result = await _service.UpdateCardAsync(dto, _deckId, _cardId, _userId);

		Assert.True(result.IsSuccess);
	}

	[Fact]
	public async Task UpdateCardAsync_ReturnsFailure_WhenNoPermission()
	{
		var dto = new AddOrUpdateCardDto { OriginalWord = "hello", TranslatedWord = "hola" };
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck(Guid.NewGuid())));

		var result = await _service.UpdateCardAsync(dto, _deckId, _cardId, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task UpdateCardAsync_ReturnsFailure_WhenRepositoryFails()
	{
		var dto = new AddOrUpdateCardDto { OriginalWord = "hello", TranslatedWord = "hola" };
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.UpdateCardAsync(It.IsAny<Card>())).ReturnsAsync(ResultFactory.Conflict("error"));

		var result = await _service.UpdateCardAsync(dto, _deckId, _cardId, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.Conflict, result.Status);
	}

	[Fact]
	public async Task DeleteCardAsync_ReturnsSuccess_WhenValid()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.DeleteCardAsync(_deckId, _cardId)).ReturnsAsync(ResultFactory.Success());
		_deckRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

		var result = await _service.DeleteCardAsync(_deckId, _cardId, _userId);

		Assert.True(result.IsSuccess);
	}

	[Fact]
	public async Task DeleteCardAsync_ReturnsFailure_WhenNoPermission()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck(Guid.NewGuid())));

		var result = await _service.DeleteCardAsync(_deckId, _cardId, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task DeleteCardAsync_ReturnsFailure_WhenRepositoryFails()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.DeleteCardAsync(_deckId, _cardId)).ReturnsAsync(ResultFactory.Conflict("error"));

		var result = await _service.DeleteCardAsync(_deckId, _cardId, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.Conflict, result.Status);
	}

	[Fact]
	public async Task GetCardByIdAsync_ReturnsSuccess_WhenValid()
	{
		var card = CreateCard();
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.GetCardByIdAsync(_deckId, _cardId)).ReturnsAsync(ResultFactory.Success(card));

		var result = await _service.GetCardByIdAsync(_deckId, _cardId, _userId);

		Assert.True(result.IsSuccess);
		Assert.Equal(card.Id, result.Value.Id);
	}

	[Fact]
	public async Task GetCardByIdAsync_ReturnsFailure_WhenNoPermission()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck(Guid.NewGuid())));

		var result = await _service.GetCardByIdAsync(_deckId, _cardId, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task GetCardByIdAsync_ReturnsFailure_WhenNotFound()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.GetCardByIdAsync(_deckId, _cardId)).ReturnsAsync(ResultFactory.NotFound<Card>("not found"));

		var result = await _service.GetCardByIdAsync(_deckId, _cardId, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task GetCardsAsync_ReturnsSuccess_WhenValid()
	{
		var cards = new List<Card> { CreateCard() };
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.GetCardsAsync(_deckId, 1, 10)).ReturnsAsync(ResultFactory.Success(cards.AsEnumerable()));

		var result = await _service.GetCardsAsync(_deckId, _userId, 1, 10);

		Assert.True(result.IsSuccess);
		Assert.Single(result.Value);
	}

	[Fact]
	public async Task GetCardsAsync_ReturnsFailure_WhenNoPermission()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck(Guid.NewGuid())));

		var result = await _service.GetCardsAsync(_deckId, _userId, 1, 10);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task GetCardsAsync_ReturnsFailure_WhenRepositoryFails()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.GetCardsAsync(_deckId, 1, 10)).ReturnsAsync(ResultFactory.NotFound<IEnumerable<Card>>("not found"));

		var result = await _service.GetCardsAsync(_deckId, _userId, 1, 10);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task GetCardCountAsync_ReturnsSuccess_WhenValid()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.GetCardCountAsync(_deckId)).ReturnsAsync(ResultFactory.Success(5));

		var result = await _service.GetCardCountAsync(_deckId, _userId);

		Assert.True(result.IsSuccess);
		Assert.Equal(5, result.Value);
	}

	[Fact]
	public async Task GetCardCountAsync_ReturnsFailure_WhenNoPermission()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck(Guid.NewGuid())));

		var result = await _service.GetCardCountAsync(_deckId, _userId);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task GetCardsToReview_ReturnsSuccess_WhenValid()
	{
		var cards = new List<Card> { CreateCard() };
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.GetCardsToReview(_deckId, It.IsAny<DateTime>(), 10, null))
			.ReturnsAsync(ResultFactory.Success((cards.AsEnumerable(), (int?)1)));

		var result = await _service.GetCardsToReview(_deckId, _userId, DateTime.UtcNow, 10, null);

		Assert.True(result.IsSuccess);
		Assert.Single(result.Value.Cards);
		Assert.Equal(1, result.Value.Count);
	}

	[Fact]
	public async Task GetCardsToReview_ReturnsFailure_WhenPageSizeInvalid()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));

		var result = await _service.GetCardsToReview(_deckId, _userId, DateTime.UtcNow, 0, null);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.InvalidArguments, result.Status);
	}

	[Fact]
	public async Task GetCardsToReview_ReturnsFailure_WhenNoPermission()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck(Guid.NewGuid())));

		var result = await _service.GetCardsToReview(_deckId, _userId, DateTime.UtcNow, 10, null);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task GetCardsToReview_ReturnsFailure_WhenRepositoryFails()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.GetCardsToReview(_deckId, It.IsAny<DateTime>(), 10, null))
			.ReturnsAsync(ResultFactory.NotFound<(IEnumerable<Card>, int?)>("not found"));

		var result = await _service.GetCardsToReview(_deckId, _userId, DateTime.UtcNow, 10, null);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task ReviewCardAsync_ReturnsSuccess_WhenValid()
	{
		var card = CreateCard();
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.GetCardByIdAsync(_deckId, _cardId)).ReturnsAsync(ResultFactory.Success(card));
		_deckRepositoryMock.Setup(r => r.UpdateCardAsync(card)).ReturnsAsync(ResultFactory.Success());
		_deckRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

		var result = await _service.ReviewCardAsync(_deckId, _cardId, _userId, true);

		Assert.True(result.IsSuccess);
		_cardReviewServiceMock.Verify(s => s.UpdateReview(card, true), Times.Once);
	}

	[Fact]
	public async Task ReviewCardAsync_ReturnsFailure_WhenNoPermission()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck(Guid.NewGuid())));

		var result = await _service.ReviewCardAsync(_deckId, _cardId, _userId, true);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task ReviewCardAsync_ReturnsFailure_WhenCardNotFound()
	{
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.GetCardByIdAsync(_deckId, _cardId)).ReturnsAsync(ResultFactory.NotFound<Card>("not found"));

		var result = await _service.ReviewCardAsync(_deckId, _cardId, _userId, true);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task ReviewCardAsync_ReturnsFailure_WhenUpdateFails()
	{
		var card = CreateCard();
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(_deckId)).ReturnsAsync(ResultFactory.Success(CreateDeck()));
		_deckRepositoryMock.Setup(r => r.GetCardByIdAsync(_deckId, _cardId)).ReturnsAsync(ResultFactory.Success(card));
		_deckRepositoryMock.Setup(r => r.UpdateCardAsync(card)).ReturnsAsync(ResultFactory.Conflict("error"));

		var result = await _service.ReviewCardAsync(_deckId, _cardId, _userId, true);

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.Conflict, result.Status);
	}

	private Deck CreateDeck(Guid? userId = null) =>
	new Deck(_deckId, "Deck", "Desc", userId ?? _userId, "en", "es");

	private Card CreateCard(Guid? deckId = null) =>
		new Card(_cardId, deckId ?? _deckId, "hello", "hola", 0, DateTime.UtcNow.AddDays(1), DateTime.MinValue);
}
