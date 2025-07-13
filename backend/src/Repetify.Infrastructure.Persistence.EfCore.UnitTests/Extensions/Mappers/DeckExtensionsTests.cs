using System;
using Xunit;
using Repetify.Domain.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Extensions.Mappers;
using Repetify.Testing.Extensions;

namespace Repetify.Infrastructure.Persistence.EfCore.UnitTests.Extensions.Mappers;

public class DeckExtensionsTests
{
	[Fact]
	public void ToEntity_ShouldMapCorrectly()
	{
		// Arrange
		var deckId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var deck = Deck.Create(
			id: deckId,
			name: "Spanish Vocabulary",
			description: "Basic Spanish words",
			userId: userId
		).AssertIsSuccess();

		// Act
		var entity = deck.ToDataEntity();

		// Assert
		Assert.NotNull(entity);
		Assert.Equal(deck.Id, entity.Id);
		Assert.Equal(deck.Name, entity.Name);
		Assert.Equal(deck.Description, entity.Description);
		Assert.Equal(deck.UserId, entity.UserId);
	}

	[Fact]
	public void ToDomain_ShouldMapCorrectly()
	{
		// Arrange
		var deckEntity = new DeckEntity
		{
			Id = Guid.NewGuid(),
			Name = "French Vocabulary",
			Description = "Intermediate French words",
			UserId = Guid.NewGuid(),
		};

		// Act
		var domain = deckEntity.ToDomain().AssertIsSuccess();

		// Assert
		Assert.NotNull(domain);
		Assert.Equal(deckEntity.Id, domain.Id);
		Assert.Equal(deckEntity.Name, domain.Name);
		Assert.Equal(deckEntity.Description, domain.Description);
		Assert.Equal(deckEntity.UserId, domain.UserId);
	}

	[Fact]
	public void ToEntity_ShouldThrowArgumentNullException_WhenDeckIsNull()
	{
		// Arrange
		Deck? nullDeck = null;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => nullDeck!.ToDataEntity());
	}

	[Fact]
	public void ToDomain_ShouldThrowArgumentNullException_WhenDeckEntityIsNull()
	{
		// Arrange
		DeckEntity? nullEntity = null;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => nullEntity!.ToDomain());
	}
	[Fact]
	public void UpdateFromDomain_ShouldUpdateCorrectly()
	{
		// Arrange
		var deckEntity = new DeckEntity
		{
			Id = Guid.NewGuid(),
			Name = "Old Name",
			Description = "Old Description",
			UserId = Guid.NewGuid(),
		};

		var deckDomain = Deck.Create(
			id: deckEntity.Id,
			name: "Updated Name",
			description: "Updated Description",
			userId: deckEntity.UserId
		).AssertIsSuccess();

		// Act
		deckEntity.UpdateFromDomain(deckDomain);

		// Assert
		Assert.Equal(deckDomain.Id, deckEntity.Id);
		Assert.Equal(deckDomain.Name, deckEntity.Name);
		Assert.Equal(deckDomain.Description, deckEntity.Description);
		Assert.Equal(deckDomain.UserId, deckEntity.UserId);
	}

	[Theory]
	[InlineData(true, false)]
	[InlineData(false, true)]
	public void UpdateFromDomain_ShouldThrowArgumentNullException_WhenInputIsNull(bool dataEntityIsNull, bool domainIsNull)
	{
		// Arrange
		DeckEntity? deckEntity = !dataEntityIsNull? new DeckEntity
		{
			Name = "Deck test",
		} : null;
		Deck? deck = !domainIsNull ? Deck.Create(
			id: Guid.NewGuid(),
			name: "Test Name",
			description: "Test Description",
			userId: Guid.NewGuid()
		).AssertIsSuccess() : null;

		// Act & Assert
		var exception = Assert.Throws<ArgumentNullException>(() => deckEntity!.UpdateFromDomain(deck!));
		Assert.Equal(exception.ParamName, domainIsNull ? nameof(deck) : nameof(deckEntity));
	}
}
