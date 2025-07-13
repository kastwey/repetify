using System.ComponentModel.DataAnnotations;

namespace Repetify.Application.Dtos;

/// <summary>
/// Data Transfer Object for Deck.
/// </summary>
public class DeckDto
{
	/// <summary>
	/// Gets or sets the deck ID.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the name of the deck.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the description of the deck.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Gets or sets the user ID associated with the deck.
	/// </summary>
	public Guid UserId { get; set; }


	/// <summary>
	/// Initializes a new instance of the <see cref="DeckDto"/> class.
	/// </summary>
	/// <param name="id">The deck ID.</param>
	/// <param name="name">The name of the deck.</param>
	/// <param name="description">The description of the deck.</param>
	/// <param name="userId">The user ID associated with the deck.</param>
	public DeckDto(Guid id, string name, string? description, Guid userId)
	{
		ArgumentException.ThrowIfNullOrEmpty(name);

		Id = id;
		Name = name;
		Description = description;
		UserId = userId;
	}
}
