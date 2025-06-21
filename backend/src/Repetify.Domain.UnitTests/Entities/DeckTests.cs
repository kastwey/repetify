using Repetify.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repetify.Domain.UnitTests.Entities;

public class DeckTests
{

	[Fact]
	public void Deck_Initializes_WhenValidValuesAreProvided()
	{
		// Arrange

		var deckName = "My test";
		var deckDescription = "This is a test deck";
		var userId = Guid.NewGuid();
		var originalLanguage = "English";
		var translatedLanguage = "Spanish";
		// Act
		var deck = new Deck(deckName, deckDescription, userId, originalLanguage, translatedLanguage);

		// Assert
		Assert.NotNull(deck);
		Assert.NotEqual(Guid.Empty, deck.Id);
		Assert.Equal(deckName, deck.Name);
		Assert.Equal(deckDescription, deck.Description);
		Assert.Equal(userId, deck.UserId);
		Assert.Equal(originalLanguage, deck.OriginalLanguage);
		Assert.Equal(translatedLanguage, deck.TranslatedLanguage);
	}

	[Fact]
	public void Deck_ThrowsArgumentNullException_WhenNameIsNull()
	{
		// Arrange
		string? deckName = null;
		var deckDescription = "This is a test deck";
		var userId = Guid.NewGuid();
		var originalLanguage = "English";
		var translatedLanguage = "Spanish";

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new Deck(deckName!, deckDescription, userId, originalLanguage, translatedLanguage));
	}

	[Fact]
	public void Deck_ThrowsArgumentException_WhenNameIsEmpty()
	{
		// Arrange
		var deckName = string.Empty;
		var deckDescription = "This is a test deck";
		var userId = Guid.NewGuid();
		var originalLanguage = "English";
		var translatedLanguage = "Spanish";

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new Deck(deckName, deckDescription, userId, originalLanguage, translatedLanguage));
	}

	[Fact]
	public void Deck_ThrowsArgumentException_WhenNameIsWhitespace()
	{
		// Arrange
		var deckName = "   ";
		var deckDescription = "This is a test deck";
		var userId = Guid.NewGuid();
		var originalLanguage = "English";
		var translatedLanguage = "Spanish";
		// Act & Assert
		Assert.Throws<ArgumentException>(() => new Deck(deckName, deckDescription, userId, originalLanguage, translatedLanguage));
	}

	[Fact]
	public void Deck_Initialize_WhenDescriptionIsNull()
	{
		// Arrange
		var deckName = "My test";
		string? deckDescription = null;
		var userId = Guid.NewGuid();
		var originalLanguage = "English";
		var translatedLanguage = "Spanish";

		// Act
		var deck = new Deck(deckName, deckDescription, userId, originalLanguage, translatedLanguage);

		// Assert
		Assert.NotNull(deck);
		Assert.Null(deck.Description);
	}


	[Fact]
	public void Deck_ThrowsArgumentNullException_WhenOriginalLanguageIsNull()
	{
		// Arrange
		var deckName = "My test";
		var deckDescription = "This is a test deck";
		var userId = Guid.NewGuid();
		string? originalLanguage = null;
		var translatedLanguage = "Spanish";

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new Deck(deckName, deckDescription, userId, originalLanguage!, translatedLanguage));
	}

	[Fact]
	public void Deck_ThrowsArgumentException_WhenOriginalLanguageIsEmpty()
	{
		// Arrange
		var deckName = "My test";
		var deckDescription = "This is a test deck";
		var userId = Guid.NewGuid();
		var originalLanguage = string.Empty;
		var translatedLanguage = "Spanish";

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new Deck(deckName, deckDescription, userId, originalLanguage, translatedLanguage));
	}

	[Fact]
	public void Deck_ThrowsArgumentException_WhenOriginalLanguageIsWhitespace()
	{
		// Arrange
		var deckName = "My test";
		var deckDescription = "This is a test deck";
		var userId = Guid.NewGuid();
		var originalLanguage = "   ";
		var translatedLanguage = "Spanish";

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new Deck(deckName, deckDescription, userId, originalLanguage, translatedLanguage));
	}

	[Fact]
	public void Deck_ThrowsArgumentNullException_WhenTranslatedLanguageIsNull()
	{
		// Arrange
		var deckName = "My test";
		var deckDescription = "This is a test deck";
		var userId = Guid.NewGuid();
		var originalLanguage = "English";
		string? translatedLanguage = null;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new Deck(deckName, deckDescription, userId, originalLanguage, translatedLanguage!));
	}

	[Fact]
	public void Deck_ThrowsArgumentException_WhenTranslatedLanguageIsEmpty()
	{
		// Arrange
		var deckName = "My test";
		var deckDescription = "This is a test deck";
		var userId = Guid.NewGuid();
		var originalLanguage = "English";
		var translatedLanguage = string.Empty;

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new Deck(deckName, deckDescription, userId, originalLanguage, translatedLanguage));
	}

	[Fact]
	public void Deck_ThrowsArgumentException_WhenTranslatedLanguageIsWhitespace()
	{
		// Arrange
		var deckName = "My test";
		var deckDescription = "This is a test deck";
		var userId = Guid.NewGuid();
		var originalLanguage = "English";
		var translatedLanguage = "   ";
		// Act & Assert
		Assert.Throws<ArgumentException>(() => new Deck(deckName, deckDescription, userId, originalLanguage, translatedLanguage));
	}
}