using Repetify.Crosscutting;
using Repetify.Domain.Entities;
using Repetify.Testing.Extensions;

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
		// Act
		var deck = Deck.Create(deckName, deckDescription, userId).AssertIsSuccess();

		// Assert
		Assert.NotNull(deck);
		Assert.NotEqual(Guid.Empty, deck.Id);
		Assert.Equal(deckName, deck.Name);
		Assert.Equal(deckDescription, deck.Description);
		Assert.Equal(userId, deck.UserId);
	}

	[Fact]
	public void Deck_ReturnsInvalidArgumentResult_WhenNameIsNull()
	{
		// Arrange
		string? deckName = null;
		var deckDescription = "This is a test deck";
		var userId = Guid.NewGuid();

		// Act & Assert
		var deckResult = Deck.Create(deckName!, deckDescription, userId);
		Assert.False(deckResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, deckResult.Status);
	}

	[Fact]
	public void Deck_ReturnsInvalidArgumentResult_WhenNameIsEmpty()
	{
		// Arrange
		var deckName = string.Empty;
		var deckDescription = "This is a test deck";
		var userId = Guid.NewGuid();

		// Act & Assert
		var deckResult = Deck.Create(deckName, deckDescription, userId);
		Assert.False(deckResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, deckResult.Status);
	}

	[Fact]
	public void Deck_ReturnsInvalidArgumentResult_WhenNameIsWhitespace()
	{
		// Arrange
		var deckName = "   ";
		var deckDescription = "This is a test deck";
		var userId = Guid.NewGuid();
		// Act & Assert
		var deckResult = Deck.Create(deckName, deckDescription, userId);
		Assert.False(deckResult.IsSuccess);
		Assert.Equal(ResultStatus.BusinessRuleViolated, deckResult.Status);
	}

	[Fact]
	public void Deck_Initialize_WhenDescriptionIsNull()
	{
		// Arrange
		var deckName = "My test";
		string? deckDescription = null;
		var userId = Guid.NewGuid();

		// Act
		var deck = Deck.Create(deckName, deckDescription, userId).AssertIsSuccess();

		// Assert
		Assert.NotNull(deck);
		Assert.Null(deck.Description);
	}


}