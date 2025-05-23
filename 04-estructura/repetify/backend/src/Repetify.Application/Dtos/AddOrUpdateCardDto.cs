using System.ComponentModel.DataAnnotations;

namespace Repetify.Application.Dtos;

/// <summary>
/// Data Transfer Object for add or update a Card.
/// </summary>
public class AddOrUpdateCardDto
{
	/// <summary>
	/// Gets or sets the original word.
	/// </summary>
	[Required(ErrorMessage = "The original word is required.")]
	[StringLength(500, ErrorMessage = "The original word cannot exceed 500 characters.")]
	public string? OriginalWord { get; set; }

	/// <summary>
	/// Gets or sets the translated word.
	/// </summary>
	[Required(ErrorMessage = "The translated word is required.")]
	[StringLength(500, ErrorMessage = "The translated word cannot exceed 500 characters.")]
	public string? TranslatedWord { get; set; }
}
