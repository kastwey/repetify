namespace Repetify.Application.Dtos;

/// <summary>  
/// Represents the data transfer object for cards to review.  
/// </summary>  
public class CardsToReviewDto
{
	/// <summary>  
	/// Gets or sets the count of cards to review.  
	/// </summary>  
	public int? Count { get; set; }

	/// <summary>  
	/// Gets or sets the collection of cards to review.  
	/// </summary>  
	public required IEnumerable<CardDto> Cards { get; set; }
}
