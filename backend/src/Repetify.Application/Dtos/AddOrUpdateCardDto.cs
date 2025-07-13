using System.ComponentModel.DataAnnotations;

namespace Repetify.Application.Dtos;

/// <summary>
/// Data Transfer Object for add or update a Card.
/// </summary>
public class AddOrUpdateCardDto
{
	/// <summary>
	/// Gets or sets the front side of the card.
	/// </summary>
	[Required(ErrorMessage = "The front is required.")]
	[StringLength(500, ErrorMessage = "The front cannot exceed 500 characters.")]
	public string? Front { get; set; }

	/// <summary>
	/// Gets or sets the back.
	/// </summary>
	[Required(ErrorMessage = "The back is required.")]
	[StringLength(500, ErrorMessage = "The back cannot exceed 500 characters.")]
	public string? Back { get; set; }
}
