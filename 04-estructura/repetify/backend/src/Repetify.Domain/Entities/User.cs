using System.Net.Mail;

namespace Repetify.Domain.Entities;

/// <summary>  
/// Represents a user entity with an ID, username, and email.  
/// </summary>  
public class User
{
	/// <summary>  
	/// Gets the unique identifier for the user.  
	/// </summary>  
	public Guid Id { get; private set; }

	/// <summary>  
	/// Gets the username of the user.  
	/// </summary>  
	public string Username { get; private set; }

	/// <summary>  
	/// Gets the email address of the user.  
	/// </summary>  
	public string Email { get; private set; }

	/// <summary>  
	/// Initializes a new instance of the <see cref="User"/> class.  
	/// </summary>  
	/// <param name="id">The unique identifier for the user. If null, a new GUID will be generated.</param>  
	/// <param name="username">The username of the user. Cannot be null.</param>  
	/// <param name="email">The email address of the user. Cannot be null.</param>  
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="username"/> or <paramref name="email"/> is null.</exception>  
	public User(Guid? id, string username, string email)
	{
		ArgumentNullException.ThrowIfNullOrWhiteSpace(username);
		ArgumentNullException.ThrowIfNullOrWhiteSpace(email);
		try
		{
			_ = new MailAddress(email);
		}
		catch (FormatException)
		{
			throw new ArgumentException("Invalid email format", nameof(email));
		}

		Id = id ?? Guid.NewGuid();
		Username = username;
		Email = email;
	}
}
