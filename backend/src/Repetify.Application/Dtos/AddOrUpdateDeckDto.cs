using System.ComponentModel.DataAnnotations;

namespace Repetify.Application.Dtos;

/// <summary>
/// Data Transfer Object for adding or updating a Deck.
/// </summary>
public class AddOrUpdateDeckDto
{
	/// <summary>
	/// Gets or sets the name of the deck.
	/// </summary>
	[Required(ErrorMessage = "The name is required.")]
	[StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
	public string? Name { get; set; }

	/// <summary>
	/// Gets or sets the description of the deck.
	/// </summary>
	[StringLength(500, ErrorMessage = "The description cannot exceed 500 characters.")]
	public string? Description { get; set; }

	/// <summary>
	/// Gets or sets the original language of the deck.
	/// </summary>
	[Required(ErrorMessage = "The original language is required.")]
	[StringLength(50, ErrorMessage = "The original language cannot exceed 50 characters.")]
	public string? OriginalLanguage { get; set; }

	/// <summary>
	/// Gets or sets the translated language of the deck.
	/// </summary>
	[Required(ErrorMessage = "The translated language is required.")]
	[StringLength(50, ErrorMessage = "The translated language cannot exceed 50 characters.")]
	public string? TranslatedLanguage { get; set; }
}
