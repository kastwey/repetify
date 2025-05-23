using System.Diagnostics.CodeAnalysis;

namespace Repetify.Infrastructure.Persistence.EfCore.Entities;

/// <summary>
/// Represents a user entity in the system.
/// </summary>
[SuppressMessage("Design", "CA2227:Prefer readonly fields for collections", Justification = "Required for EF Core.")]
public class UserEntity
{
	/// <summary>
	/// Gets or sets the unique identifier for the user.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the username of the user.
	/// </summary>
	public required string Username { get; set; }

	/// <summary>
	/// Gets or sets the email of the user.
	/// </summary>
	public required string Email { get; set; }

	/// <summary>
	/// Gets or sets the collection of decks associated with the user.
	/// </summary>
	public ICollection<DeckEntity> Decks { get; set; } = new List<DeckEntity>();
}
