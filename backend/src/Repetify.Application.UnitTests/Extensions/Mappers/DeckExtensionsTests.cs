using Repetify.Application.Dtos;
using Repetify.Application.Extensions.Mappers;
using Repetify.Domain.Entities;
using Repetify.Testing.Extensions;

using Xunit;

namespace Repetify.Application.UnitTests.Extensions.Mappers;

public class DeckExtensionsTests
{
	#region ToDto(Deck)

	[Fact]
	public void ToDto_ShouldReturnDeckDto_WhenDeckIsValid()
	{
		// Arrange
		var deck = Deck.TryCreate(
			id: Guid.NewGuid(),
			name: "My Deck",
			description: "Test description",
			userId: Guid.NewGuid(),
			originalLanguage: "English",
			translatedLanguage: "Spanish"
		).AssertIsSuccess();

		// Act
		var result = deck.ToDto();

		// Assert
		Assert.NotNull(result);
		Assert.Equal(deck.Id, result.Id);
		Assert.Equal(deck.Name, result.Name);
		Assert.Equal(deck.Description, result.Description);
		Assert.Equal(deck.UserId, result.UserId);
		Assert.Equal(deck.OriginalLanguage, result.OriginalLanguage);
		Assert.Equal(deck.TranslatedLanguage, result.TranslatedLanguage);
	}

	[Fact]
	public void ToDto_ShouldThrowArgumentNullException_WhenDeckIsNull()
	{
		// Arrange
		Deck deck = null!;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => deck.ToDto());
	}

	#endregion

	#region ToDtoList(IEnumerable<Deck>)

	[Fact]
	public void ToDtoList_ShouldReturnListOfDeckDto_WhenListIsNotEmpty()
	{
		// Arrange
		var decks = new List<Deck>
		{
			Deck.TryCreate(name: "Deck 1", description: "Desc1", userId: Guid.NewGuid(), originalLanguage: "En", translatedLanguage: "Es").AssertIsSuccess(),
			Deck.TryCreate(name: "Deck 2", description: "Desc2", userId: Guid.NewGuid(), originalLanguage: "Fr", translatedLanguage: "Es").AssertIsSuccess()
		};

		// Act
		var result = decks.ToDtoList().ToList();

		// Assert
		Assert.NotNull(result);
		Assert.Equal(decks.Count, result.Count);

		for (int i = 0; i < decks.Count; i++)
		{
			Assert.Equal(decks[i].Name, result[i].Name);
			Assert.Equal(decks[i].Description, result[i].Description);
			Assert.Equal(decks[i].UserId, result[i].UserId);
			Assert.Equal(decks[i].OriginalLanguage, result[i].OriginalLanguage);
			Assert.Equal(decks[i].TranslatedLanguage, result[i].TranslatedLanguage);
		}
	}

	[Fact]
	public void ToDtoList_ShouldReturnEmptyList_WhenListIsEmpty()
	{
		// Arrange
		var emptyDecks = new List<Deck>();

		// Act
		var result = emptyDecks.ToDtoList().ToList();

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result);
	}

	[Fact]
	public void ToDtoList_ShouldThrowArgumentNull_WhenListIsNull()
	{
		// Arrange
		IEnumerable<Deck> decks = null!;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => decks.ToDtoList());
	}

	#endregion


	#region ToEntity(AddOrUpdateDeckDto, Guid)

	[Fact]
	public void ToEntity_ShouldReturnDeck_WhenAddOrUpdateDeckDtoIsValid()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var deckId = Guid.NewGuid();

		var dto = new AddOrUpdateDeckDto { 
			Name = "New Deck",
			Description = "New Description",
			OriginalLanguage = "English",
			TranslatedLanguage = "Spanish"
		};

		// Act
		var entity = dto.ToEntity(userId, deckId).AssertIsSuccess();

		// Assert
		Assert.NotNull(entity);
		Assert.Equal(deckId, entity.Id);
		Assert.Equal(dto.Name, entity.Name);
		Assert.Equal(dto.Description, entity.Description);
		Assert.Equal(userId, entity.UserId);
		Assert.Equal(dto.OriginalLanguage, entity.OriginalLanguage);
		Assert.Equal(dto.TranslatedLanguage, entity.TranslatedLanguage);
	}

	[Fact]
	public void ToEntity_ShouldThrowArgumentNullException_WhenAddOrUpdateDeckDtoIsNull()
	{
		// Arrange
		AddOrUpdateDeckDto dto = null!;
		var userId = Guid.NewGuid();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => dto.ToEntity(userId, null));
	}

	#endregion
}
