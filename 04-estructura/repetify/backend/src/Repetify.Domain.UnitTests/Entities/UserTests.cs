using Repetify.Domain.Entities;

namespace Repetify.Domain.UnitTests.Entities;

public class UserTests
{
	[Fact]
	public void User_Initializes_WhenValidValuesAreProvided()
	{
		// Arrange
		var username = "JohnDoe";
		var email = "johndoe@example.com";

		// Act
		var user = new User(Guid.NewGuid(), username, email);

		// Assert
		Assert.Equal(username, user.Username);
		Assert.Equal(email, user.Email);
		Assert.NotEqual(Guid.Empty, user.Id); // Ensure ID is generated
	}

	[Fact]
	public void User_ThrowsArgumentNullException_WhenUsernameIsNull()
	{
		// Arrange
		string? username = null;
		var email = "johndoe@example.com";

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new User(Guid.NewGuid(), username!, email));
	}

	[Fact]
	public void User_ThrowsArgumentException_WhenUsernameIsEmpty()
	{
		// Arrange
		var username = string.Empty;
		var email = "johndoe@example.com";

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new User(Guid.NewGuid(), username, email));
	}

	[Fact]
	public void User_ThrowsArgumentException_WhenUsernameIsWhitespace()
	{
		// Arrange
		var username = "   ";
		var email = "johndoe@example.com";

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new User(Guid.NewGuid(), username, email));
	}

	[Fact]
	public void User_ThrowsArgumentNullException_WhenEmailIsNull()
	{
		// Arrange
		var username = "JohnDoe";
		string? email = null;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new User(Guid.NewGuid(), username, email!));
	}

	[Fact]
	public void User_ThrowsArgumentException_WhenEmailIsEmpty()
	{
		// Arrange
		var username = "JohnDoe";
		var email = string.Empty;

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new User(Guid.NewGuid(), username, email));
	}

	[Fact]
	public void User_ThrowsArgumentException_WhenEmailIsWhitespace()
	{
		// Arrange
		var username = "JohnDoe";
		var email = "   ";

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new User(Guid.NewGuid(), username, email));
	}

	[Fact]
	public void User_ThrowsArgumentException_WhenEmailIsInvalid()
	{
		// Arrange
		var username = "JohnDoe";
		var email = "invalid-email";

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new User(Guid.NewGuid(), username, email));
	}
}
