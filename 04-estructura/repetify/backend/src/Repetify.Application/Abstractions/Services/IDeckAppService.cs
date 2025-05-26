using Repetify.Application.Dtos;
using Repetify.Crosscutting;

namespace Repetify.Application.Abstractions.Services;

/// <summary>
/// Interface for deck application service.
/// </summary>
public interface IDeckAppService
{
	/// <summary>
	/// Adds a new deck.
	/// </summary>
	/// <param name="deck">The deck to add.</param>
	/// <param name="userId">The ID of the user to whom this deck belongs.</param>
	/// <returns>A task representing the asynchronous operation, with the new Id as the result.</returns>
	Task<Result<Guid>> AddDeckAsync(AddOrUpdateDeckDto deck, Guid userId);

	/// <summary>
	/// Updates an existing deck.
	/// </summary>
	/// <param name="deckId">The Id of the deck to be updated</param>
	/// <param name="deck">The deck DTO containing updated information.</param>
	/// <param name="userId">The ID of the user to whom this deck belongs.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task<Result> UpdateDeckAsync(Guid deckId, AddOrUpdateDeckDto deck, Guid userId);

	/// <summary>
	/// Deletes the specified deck.
	/// </summary>
	/// <param name="deckId">The ID of the deck.</param>
	/// <returns>A task representing the asynchronous operation, with a boolean result indicating success or failure.</returns>
	Task<Result<bool>> DeleteDeckAsync(Guid deckId);

	/// <summary>
	/// Gets a deck by its ID.
	/// </summary>
	/// <param name="deckId">The ID of the deck.</param>
	/// <returns>A task representing the asynchronous operation, with a result of the deck DTO.</returns>
	Task<Result<DeckDto>> GetDeckByIdAsync(Guid deckId);

	/// <summary>
	/// Gets a list of decks for the specified user.
	/// </summary>
	/// <param name="userId">The ID of the user.</param>
	/// <returns>A task representing the asynchronous operation, with a result of an enumerable of deck DTOs.</returns>
	Task<Result<IEnumerable<DeckDto>>> GetUserDecksAsync(Guid userId);

	/// <summary>
	/// Adds a card to the specified deck.
	/// </summary>
	/// <param name="card">The card to add.</param>
	/// <param name="deckId">The ID of the deck to which the card will be added.</param>
	/// <returns>A task representing the asynchronous operation, with the new Id as the result.</returns>
	Task<Result<Guid>> AddCardAsync(AddOrUpdateCardDto card, Guid deckId);

	/// <summary>
	/// Updates a card in the specified deck.
	/// </summary>
	/// <param name="card">The card to be updated.</param>
	/// <param name="deckId">The ID of the deck to which the card belongs.</param>
	/// <param name="cardId">The ID of the card to be updated.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task<Result> UpdateCardAsync(AddOrUpdateCardDto card, Guid deckId, Guid cardId);

	/// <summary>
	/// Deletes a card from the specified deck.
	/// </summary>
	/// <param name="deckId">The ID of the deck.</param>
	/// <param name="cardId">The ID of the card.</param>
	/// <returns>A task representing the asynchronous operation</returns>
	Task<Result> DeleteCardAsync(Guid deckId, Guid cardId);

	/// <summary>
	/// Gets a card by its ID from the specified deck.
	/// </summary>
	/// <param name="deckId">The ID of the deck.</param>
	/// <param name="cardId">The ID of the card.</param>
	/// <returns>A task representing the asynchronous operation, with a result of the card DTO.</returns>
	Task<Result<CardDto>> GetCardByIdAsync(Guid deckId, Guid cardId);

	/// <summary>
	/// Gets the count of cards in the specified deck.
	/// </summary>
	/// <param name="deckId">The ID of the deck.</param>
	/// <returns>A task representing the asynchronous operation, with a result of the card count.</returns>
	Task<Result<int>> GetCardCountAsync(Guid deckId);

	/// <summary>
	/// Gets a paginated list of cards from the specified deck.
	/// </summary>
	/// <param name="deckId">The ID of the deck.</param>
	/// <param name="page">The page number.</param>
	/// <param name="pageSize">The size of the page.</param>
	/// <returns>A task representing the asynchronous operation, with a result of an enumerable of card DTOs.</returns>
	Task<Result<IEnumerable<CardDto>>> GetCardsAsync(Guid deckId, int page, int pageSize);

	/// <summary>
	/// Reviews a card in the specified deck.
	/// </summary>
	/// <param name="deckId">The ID of the deck.</param>
	/// <param name="cardId">The ID of the card.</param>
	/// <param name="isCorrect">Indicates whether the review was correct or not.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task<Result> ReviewCardAsync(Guid deckId, Guid cardId, bool isCorrect);

	/// <summary>  
	/// Gets a list of cards to review from the specified deck until a certain date.  
	/// </summary>  
	/// <param name="deckId">The ID of the deck.</param>  
	/// <param name="until">The date until which to get cards for review.</param>  
	/// <param name="pageSize">The number of cards to retrieve.</param>  
	/// <param name="cursor">The cursor for pagination, representing the last retrieved card's review date.</param>  
	/// <returns>A task representing the asynchronous operation, with a result of a <see cref="CardsToReviewDto"/> object.</returns>
	Task<Result<CardsToReviewDto>> GetCardsToReview(Guid deckId, DateTime until, int pageSize, DateTime? cursor);
}
