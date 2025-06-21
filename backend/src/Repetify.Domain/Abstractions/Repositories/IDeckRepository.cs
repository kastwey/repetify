using Repetify.Crosscutting;
using Repetify.Domain.Entities;

namespace Repetify.Domain.Abstractions.Repositories;

/// <summary>  
/// Interface for deck repository operations.  
/// </summary>  
public interface IDeckRepository
{
	/// <summary>  
	/// Adds a new deck asynchronously.  
	/// </summary>  
	/// <param name="deck">The deck to add.</param>  
	Task<Result> AddDeckAsync(Deck deck);

	/// <summary>  
	/// Deletes a deck asynchronously by its identifier.  
	/// </summary>  
	/// <param name="deckId">The identifier of the deck to delete.</param>  
	/// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>  
	Task<Result> DeleteDeckAsync(Guid deckId);

	/// <summary>  
	/// Updates an existing deck.  
	/// </summary>  
	/// <param name="deck">The deck to update.</param>  
	/// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>  
	Task<Result> UpdateDeckAsync(Deck deck);

	/// <summary>  
	/// Gets a deck by its identifier asynchronously.  
	/// </summary>  
	/// <param name="deckId">The identifier of the deck to retrieve.</param>  
	/// <returns>A <see cref="Result{T}"/> containing the deck or an error status.</returns>  
	Task<Result<Deck>> GetDeckByIdAsync(Guid deckId);

	/// <summary>  
	/// Gets all decks associated with a specific user asynchronously.  
	/// </summary>  
	/// <param name="userId">The identifier of the user whose decks to retrieve.</param>  
	/// <returns>A <see cref="Result{T}"/> containing the list of decks or an error status.</returns>  
	Task<Result<IEnumerable<Deck>>> GetDecksByUserIdAsync(Guid userId);

	/// <summary>  
	/// Checks if a deck name exists for a specific user.  
	/// </summary>  
	/// <param name="deckId">The identifier of the deck to exclude from the check.</param>  
	/// <param name="name">The name of the deck to check.</param>  
	/// <param name="userId">The identifier of the user.</param>  
	/// <returns>A boolean indicating whether the deck name exists for the user.</returns>  
	Task<Result<bool>> DeckNameExistsForUserAsync(Guid deckId, string name, Guid userId);

	/// <summary>  
	/// Gets the count of cards in a specific deck.  
	/// </summary>  
	/// <param name="deckId">The identifier of the deck.</param>  
	/// <returns>The number of cards in the deck.</returns>  
	Task<Result<int>> GetCardCountAsync(Guid deckId);

	/// <summary>  
	/// Gets a card by its identifier within a specific deck asynchronously.  
	/// </summary>  
	/// <param name="deckId">The identifier of the deck.</param>  
	/// <param name="cardId">The identifier of the card to retrieve.</param>  
	/// <returns>A <see cref="Result{T}"/> containing the card or an error status.</returns>  
	Task<Result<Card>> GetCardByIdAsync(Guid deckId, Guid cardId);

	/// <summary>  
	/// Gets a list of cards to review.  
	/// </summary>  
	/// <param name="deckId">The identifier of the deck.</param>  
	/// <param name="until">The date until which cards should be reviewed.</param>  
	/// <param name="pageSize">The maximum number of cards to retrieve.</param>  
	/// <param name="cursor">The optional cursor for pagination.</param>  
	/// <returns>A <see cref="Result{T}"/> containing the list of cards to review or an error status.</returns>  
	/// <summary>  
	/// Gets a list of cards to review.  
	/// </summary>  
	/// <param name="deckId">The identifier of the deck.</param>  
	/// <param name="until">The date until which cards should be reviewed.</param>  
	/// <param name="pageSize">The maximum number of cards to retrieve.</param>  
	/// <param name="cursor">The optional cursor for pagination.</param>  
	/// <returns>A <see cref="Result{T}"/> containing a tuple with the list of cards to review and an optional count, or an error status.</returns>
	Task<Result<(IEnumerable<Card> Cards, int? Count)>> GetCardsToReview(Guid deckId, DateTime until, int pageSize, DateTime? cursor);

	/// <summary>  
	/// Adds a new card asynchronously.  
	/// </summary>  
	/// <param name="card">The card to add.</param>  
	Task<Result> AddCardAsync(Card card);

	/// <summary>  
	/// Updates an existing card.  
	/// </summary>  
	/// <param name="card">The card to update.</param>  
	/// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>  
	Task<Result> UpdateCardAsync(Card card);

	/// <summary>  
	/// Deletes a card from a specific deck asynchronously.  
	/// </summary>  
	/// <param name="deckId">The identifier of the deck.</param>  
	/// <param name="cardId">The identifier of the card to delete.</param>  
	/// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>  
	Task<Result> DeleteCardAsync(Guid deckId, Guid cardId);

	/// <summary>  
	/// Gets a paginated list of cards from a specific deck asynchronously.  
	/// </summary>  
	/// <param name="deckId">The identifier of the deck.</param>  
	/// <param name="page">The page number to retrieve.</param>  
	/// <param name="pageSize">The number of cards per page.</param>  
	/// <returns>A <see cref="Result{T}"/> containing the list of cards or an error status.</returns>  
	Task<Result<IEnumerable<Card>>> GetCardsAsync(Guid deckId, int page, int pageSize);

	/// <summary>  
	/// Saves changes to the repository asynchronously.  
	/// </summary>  
	Task SaveChangesAsync();
}
