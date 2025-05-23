using FluentAssertions;
using FluentAssertions.Specialized;

using Moq;

using Repetify.Application.Dtos;
using Repetify.Application.Services;
using Repetify.Crosscutting;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Abstractions.Services;
using Repetify.Domain.Entities;
using Repetify.Domain.Exceptions;

namespace Repetify.Application.Tests.Services;

public class DeckAppServiceTests
{
	private readonly Mock<IDeckValidator> _deckValidatorMock;
	private readonly Mock<IDeckRepository> _deckRepositoryMock;
	private readonly Mock<ICardReviewService> _reviewCardServiceMock;
	private readonly DeckAppService _deckAppService;

	public DeckAppServiceTests()
	{
		_deckValidatorMock = new Mock<IDeckValidator>();
		_reviewCardServiceMock = new Mock<ICardReviewService>();
		_deckValidatorMock.Setup(m => m.EnsureIsValid(It.IsAny<Deck>())).Returns(Task.CompletedTask);
		_deckRepositoryMock = new Mock<IDeckRepository>();
		_deckAppService = new DeckAppService(_deckValidatorMock.Object, _reviewCardServiceMock.Object, _deckRepositoryMock.Object);
	}

	[Fact]
	public async Task AddDeckAsync_Should_Call_Repository_And_SaveChanges()
	{
		var deckDto = CreateFakeAddOrUpdateDeck();

		await _deckAppService.AddDeckAsync(deckDto, Guid.NewGuid());

		_deckRepositoryMock.Verify(r => r.AddDeckAsync(It.IsAny<Deck>()), Times.Once);
		_deckRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
	}

	[Fact]
	public async Task AddDeckAsync_Should_Return_Conflict_When_Deck_Already_Exists()
	{
		// Arrange
		var deckDto = CreateFakeAddOrUpdateDeck();
		var exceptionMessage = "Deck already exists!";
		_deckValidatorMock
			.Setup(v => v.EnsureIsValid(It.IsAny<Deck>()))
			.ThrowsAsync(new EntityExistsException(exceptionMessage));

		// Act
		var result = await _deckAppService.AddDeckAsync(deckDto, Guid.NewGuid());

		// Assert
		result.Status.Should().Be(ResultStatus.Conflict);
		result.ErrorMessage.Should().Be(exceptionMessage);
		_deckRepositoryMock.Verify(r => r.AddDeckAsync(It.IsAny<Deck>()), Times.Never);
	}


	[Fact]
	public async Task UpdateDeckAsync_Should_Call_Repository_And_SaveChanges()
	{
		var deckDto = CreateFakeAddOrUpdateDeck();
		_deckRepositoryMock.Setup(m => m.UpdateDeckAsync(It.IsAny<Deck>())).ReturnsAsync(ResultFactory.Success());

		await _deckAppService.UpdateDeckAsync(deckDto, Guid.NewGuid());

		_deckRepositoryMock.Verify(r => r.UpdateDeckAsync(It.IsAny<Deck>()), Times.Once);
		_deckRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
	}

	[Fact]
	public async Task UpdateDeckAsync_Should_Return_Conflict_When_Deck_Already_Exists()
	{
		// Arrange
		var deckDto = CreateFakeAddOrUpdateDeck();
		var exceptionMessage = "Deck name already in use!";
		_deckValidatorMock
			.Setup(v => v.EnsureIsValid(It.IsAny<Deck>()))
			.ThrowsAsync(new EntityExistsException(exceptionMessage));

		// Act
		var result = await _deckAppService.UpdateDeckAsync(deckDto, Guid.NewGuid());

		// Assert
		result.Status.Should().Be(ResultStatus.Conflict);
		result.ErrorMessage.Should().Be(exceptionMessage);
		_deckRepositoryMock.Verify(r => r.UpdateDeckAsync(It.IsAny<Deck>()), Times.Never);
	}

	[Fact]
	public async Task DeleteDeckAsync_Should_Call_Repository_And_SaveChanges_When_Deck_Exists()
	{
		var deckId = Guid.NewGuid();
		_deckRepositoryMock.Setup(r => r.DeleteDeckAsync(deckId)).ReturnsAsync(ResultFactory.Success());

		var result = await _deckAppService.DeleteDeckAsync(deckId);

		result.Value.Should()
			.BeTrue();
		_deckRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
	}

	[Fact]
	public async Task DeleteDeckAsync_Should_Return_False_When_Deck_Does_Not_Exist()
	{
		var deckId = Guid.NewGuid();
		_deckRepositoryMock.Setup(r => r.DeleteDeckAsync(deckId)).ReturnsAsync(ResultFactory.NotFound());

		var result = await _deckAppService.DeleteDeckAsync(deckId);

		result.Value.Should().BeFalse();
		_deckRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
	}

	[Fact]
	public async Task GetDeckByIdAsync_Should_Return_DeckDto_When_Deck_Exists()
	{
		var deckId = Guid.NewGuid();
		var deck = new Deck(deckId, "Deck", "description", Guid.NewGuid(), "english", "spanish");
		_deckRepositoryMock.Setup(r => r.GetDeckByIdAsync(deckId)).ReturnsAsync(ResultFactory.Success(deck));

		var result = await _deckAppService.GetDeckByIdAsync(deckId);

		result.Should().NotBeNull();
		result.Value!.Id.Should().Be(deckId);
	}

	[Fact]
	public async Task GetDeckByIdAsync_Should_Return_NotFound_When_Deck_Does_Not_Exist()
	{
		// Arrange
		_deckRepositoryMock
			.Setup(r => r.GetDeckByIdAsync(It.IsAny<Guid>()))
			.ReturnsAsync(ResultFactory.NotFound<Deck>("Deck not found."));

		// Act
		var result = await _deckAppService.GetDeckByIdAsync(Guid.NewGuid());

		// Assert
		result.Status.Should().Be(ResultStatus.NotFound);
		result.ErrorMessage.Should().Be("Deck not found.");
	}

	[Fact]
	public async Task AddCardAsync_Should_Call_Repository_And_SaveChanges()
	{
		var cardDto = new AddOrUpdateCardDto{ OriginalWord = "Hola", TranslatedWord = "Hello" };

		await _deckAppService.AddCardAsync(cardDto, Guid.NewGuid());

		_deckRepositoryMock.Verify(r => r.AddCardAsync(It.IsAny<Card>()), Times.Once);
		_deckRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
	}

	[Fact]
	public async Task ReviewCardAsync_Should_Update_Card_And_SaveChanges_When_Card_Exists()
	{
		var deckId = Guid.NewGuid();
		var card = new Card(deckId, "Hola", "Hello", 3, DateTime.UtcNow, DateTime.UtcNow);
		_deckRepositoryMock.Setup(r => r.GetCardByIdAsync(deckId, card.Id)).ReturnsAsync(ResultFactory.Success(card));

		await _deckAppService.ReviewCardAsync(deckId, card.Id, true);

		_reviewCardServiceMock.Verify(r => r.UpdateReview(card, true), Times.Once);
		_deckRepositoryMock.Verify(r => r.UpdateCardAsync(card), Times.Once);
		_deckRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
	}

	[Fact]
	public async Task ReviewCardAsync_Returns_NotFoundStatusResult_When_Card_Not_Found()
	{
		var deckId = Guid.NewGuid();
		var cardId = Guid.NewGuid();

		_deckRepositoryMock.Setup(r => r.GetCardByIdAsync(deckId, cardId)).ReturnsAsync(ResultFactory.NotFound<Card>());

		var result = await _deckAppService.ReviewCardAsync(deckId, cardId, true);

		result.Status.Should().Be(ResultStatus.NotFound);
	}

	[Fact]
	public async Task GetCardsToReview_Should_Return_Correct_Cards()
	{
		var deckId = Guid.NewGuid();
		var untilDate = DateTime.UtcNow;
		var pageSize = 10;
		var cards = new List<Card>
					{
						new Card(deckId, "Hola", "Hello", 1, DateTime.UtcNow, DateTime.UtcNow)
					};
		int? count = cards.Count;

		_deckRepositoryMock
			.Setup(r => r.GetCardsToReview(deckId, untilDate, pageSize, null))
			.ReturnsAsync(ResultFactory.Success((cards.AsEnumerable(), count)));

		var result = await _deckAppService.GetCardsToReview(deckId, untilDate, pageSize, null);
		Assert.True(result.IsSuccess);

		result.Value.Cards.Should().HaveCount(1);
		result.Value.Cards.First().OriginalWord.Should().Be("Hola");
		result.Value.Count.Should().Be(count);
	}

	[Fact]
	public async Task GetCardsToReview_Should_Return_InvalidArguments_When_PageSize_Is_Less_Than_One()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var untilDate = DateTime.UtcNow;
		var pageSize = 0; // <-- Inválido

		// Act
		var result = await _deckAppService.GetCardsToReview(deckId, untilDate, pageSize, null);

		// Assert
		result.Status.Should().Be(ResultStatus.InvalidArguments);
		result.ErrorMessage.Should().Contain("page number must be greater than 0");
	}


	[Fact]
	public async Task GetCardCountAsync_Should_Return_Correct_Count()
	{
		var deckId = Guid.NewGuid();
		_deckRepositoryMock.Setup(r => r.GetCardCountAsync(deckId)).ReturnsAsync(5);

		var result = await _deckAppService.GetCardCountAsync(deckId);

		result.Value.Should().Be(5);
	}

	private static DeckDto CreateFakeDeck()
	{
		return new DeckDto(Guid.NewGuid(), "Test Deck", "Description", Guid.NewGuid(), "english", "spanish");
	}

	private static AddOrUpdateDeckDto CreateFakeAddOrUpdateDeck()
	{
		return new AddOrUpdateDeckDto { Name = "Test Deck", Description = "Description", OriginalLanguage = "english", TranslatedLanguage = "spanish" };
	}
}