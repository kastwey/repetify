using Moq;

using Repetify.Crosscutting;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Entities;
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
	public async Task EnsureIsValid_ShouldReturnConflictResult_WhenDeckNameExistsForUser()
	{
		// Arrange
		var mockDeckRepository = new Mock<IDeckRepository>();
		mockDeckRepository
			.Setup(repo => repo.DeckNameExistsForUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
			.ReturnsAsync(ResultFactory.Success(true));

		var validator = new DeckValidator(mockDeckRepository.Object);

		var deck = new Deck(
			name: "DuplicateName",
			description: "Deck Desc",
			userId: Guid.NewGuid(),
			originalLanguage: "English",
			translatedLanguage: "Spanish"
		);

		// Act
		var result = await validator.EnsureIsValid(deck);

		// Assert
		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.Conflict, result.Status);
	}

	[Fact]
	public async Task EnsureIsValid_ShouldReturnsSuccessResult_WhenDeckNameIsUnique()
	{
		// Arrange
		var mockDeckRepository = new Mock<IDeckRepository>();
		mockDeckRepository
			.Setup(repo => repo.DeckNameExistsForUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
			.ReturnsAsync(ResultFactory.Success(false));

		var validator = new DeckValidator(mockDeckRepository.Object);

		var deck = new Deck(
			name: "UniqueName",
			description: "Some description",
			userId: Guid.NewGuid(),
			originalLanguage: "English",
			translatedLanguage: "Spanish"
		);

		// Act
		var result = await validator.EnsureIsValid(deck);
		// assert
		Assert.True(result.IsSuccess, "Expected validation to succeed for a unique deck name.");
	}
}
