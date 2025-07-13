using Moq;

using Repetify.Crosscutting;
using Repetify.Crosscutting.Extensions;
using Repetify.Testing.Extensions;
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
			() => validator.EnsureIsValidAsync(null!)
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

		var deck = Deck.Create(
			name: "DuplicateName",
			description: "Deck Desc",
			userId: Guid.NewGuid()
		).AssertIsSuccess();

		// Act
		var result = await validator.EnsureIsValidAsync(deck);

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

		var deck = Deck.Create(
			name: "UniqueName",
			description: "Some description",
			userId: Guid.NewGuid()
		).AssertIsSuccess();

		// Act
		var result = await validator.EnsureIsValidAsync(deck);
		// assert
		Assert.True(result.IsSuccess, "Expected validation to succeed for a unique deck name.");
	}
}
