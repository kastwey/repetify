using Moq;

using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Entities;
using Repetify.Domain.Exceptions;
using Repetify.Domain.Services;

namespace Repetify.Domain.Tests;

public class DeckValidatorTests
{
	[Fact]
	public async Task EnsureIsValid_ShouldThrowArgumentNullException_WhenDeckIsNull()
	{
		// Arrange
		var mockDeckRepository = new Mock<IDeckRepository>();
		var validator = new DeckValidator(mockDeckRepository.Object);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(
			() => validator.EnsureIsValid(null!)
		);
	}

	[Fact]
	public async Task EnsureIsValid_ShouldThrowEntityExistsException_WhenDeckNameExistsForUser()
	{
		// Arrange
		var mockDeckRepository = new Mock<IDeckRepository>();
		mockDeckRepository
			.Setup(repo => repo.DeckNameExistsForUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
			.ReturnsAsync(true);

		var validator = new DeckValidator(mockDeckRepository.Object);

		var deck = new Deck(
			name: "DuplicateName",
			description: "Deck Desc",
			userId: Guid.NewGuid(),
			originalLanguage: "English",
			translatedLanguage: "Spanish"
		);

		// Act & Assert
		await Assert.ThrowsAsync<EntityExistsException>(
			() => validator.EnsureIsValid(deck)
		);
	}

	[Fact]
	public async Task EnsureIsValid_ShouldNotThrow_WhenDeckNameIsUnique()
	{
		// Arrange
		var mockDeckRepository = new Mock<IDeckRepository>();
		mockDeckRepository
			.Setup(repo => repo.DeckNameExistsForUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
			.ReturnsAsync(false);

		var validator = new DeckValidator(mockDeckRepository.Object);

		var deck = new Deck(
			name: "UniqueName",
			description: "Some description",
			userId: Guid.NewGuid(),
			originalLanguage: "English",
			translatedLanguage: "Spanish"
		);

		// Act & Assert
		var exception = await Record.ExceptionAsync(
			() => validator.EnsureIsValid(deck)
		);

		Assert.Null(exception); // A null exception means no error was thrown
	}
}
