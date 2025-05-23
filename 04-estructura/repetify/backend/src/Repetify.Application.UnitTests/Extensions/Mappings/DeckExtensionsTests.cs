using Repetify.Application.Dtos;
using Repetify.Application.Extensions.Mappings;
using Repetify.Domain.Entities;

using Xunit;

namespace Repetify.Application.Tests.Extensions.Mappings;

public class DeckExtensionsTests
{
	#region ToDto(Deck)

	[Fact]
	public void ToDto_ShouldReturnDeckDto_WhenDeckIsValid()
	{
		// Arrange
		var deck = new Deck(
			id: Guid.NewGuid(),
			name: "My Deck",
			description: "Test description",
			userId: Guid.NewGuid(),
			originalLanguage: "English",
			translatedLanguage: "Spanish"
		);

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
			new(name: "Deck 1", description: "Desc1", userId: Guid.NewGuid(), originalLanguage: "En", translatedLanguage: "Es"),
			new(name: "Deck 2", description: "Desc2", userId: Guid.NewGuid(), originalLanguage: "Fr", translatedLanguage: "Es")
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

	#region ToEntity(DeckDto)

	[Fact]
	public void ToEntity_ShouldReturnDeck_WhenDeckDtoIsValid()
	{
		// Arrange
		var deckDto = new DeckDto(
			id: Guid.NewGuid(),
			name: "DTO Deck",
			description: "DTO Description",
			userId: Guid.NewGuid(),
			originalLanguage: "English",
			translatedLanguage: "Spanish"
		);

		// Act
		var result = deckDto.ToEntity();

		// Assert
		Assert.NotNull(result);
		Assert.Equal(deckDto.Name, result.Name);
		Assert.Equal(deckDto.Description, result.Description);
		Assert.Equal(deckDto.UserId, result.UserId);
		Assert.Equal(deckDto.OriginalLanguage, result.OriginalLanguage);
		Assert.Equal(deckDto.TranslatedLanguage, result.TranslatedLanguage);
	}

	[Fact]
	public void ToEntity_ShouldThrowArgumentNullException_WhenDeckDtoIsNull()
	{
		// Arrange
		DeckDto deckDto = null!;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => deckDto.ToEntity());
	}

	#endregion

	#region ToEntity(AddOrUpdateDeckDto, Guid)

	[Fact]
	public void ToEntity_ShouldReturnDeck_WhenAddOrUpdateDeckDtoIsValid()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var dto = new AddOrUpdateDeckDto { 
			Name = "New Deck",
			Description = "New Description",
			OriginalLanguage = "English",
			TranslatedLanguage = "Spanish"
		};

		// Act
		var result = dto.ToEntity(userId);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(dto.Name, result.Name);
		Assert.Equal(dto.Description, result.Description);
		Assert.Equal(userId, result.UserId);
		Assert.Equal(dto.OriginalLanguage, result.OriginalLanguage);
		Assert.Equal(dto.TranslatedLanguage, result.TranslatedLanguage);
	}

	[Fact]
	public void ToEntity_ShouldThrowArgumentNullException_WhenAddOrUpdateDeckDtoIsNull()
	{
		// Arrange
		AddOrUpdateDeckDto dto = null!;
		var userId = Guid.NewGuid();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => dto.ToEntity(userId));
	}

	#endregion
}
