using Repetify.Application.Abstractions.Services;
using Repetify.Application.Dtos;
using Repetify.Application.Extensions.Mappings;
using Repetify.Crosscutting;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Abstractions.Services;
using Repetify.Domain.Exceptions;

namespace Repetify.Application.Services;

/// <summary>
/// Service to handle decks and cards
/// </summary>
public class DeckAppService : IDeckAppService
{
	private readonly IDeckValidator _deckValidator;
	private readonly ICardReviewService _cardReviewService;
	private readonly IDeckRepository _deckRepository;

	/// <summary>
	/// Initializes a new instance of the <see cref="DeckAppService"/> class.
	/// </summary>
	/// <param name="deckValidator">The deck validator.</param>
	/// <param name="cardReviewService">The card review service.</param>
	/// <param name="deckRepository">The deck repository.</param>
	public DeckAppService(
		IDeckValidator deckValidator,
		ICardReviewService cardReviewService,
		IDeckRepository deckRepository)
	{
		_deckValidator = deckValidator;
		_cardReviewService = cardReviewService;
		_deckRepository = deckRepository;
	}

	///  <inheritdoc/>
	public async Task<Result<Guid>> AddDeckAsync(AddOrUpdateDeckDto deck, Guid userId)
	{
		try
		{
			var deckDomain = deck.ToEntity(userId);
			await _deckValidator.EnsureIsValid(deckDomain).ConfigureAwait(false);
			await _deckRepository.AddDeckAsync(deckDomain).ConfigureAwait(false);
			await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
			return ResultFactory.Success(deckDomain.Id);
		}
		catch (EntityExistsException ex)
		{
			return ResultFactory.Conflict<Guid>(ex.Message);
		}
	}

	///  <inheritdoc/>
	public async Task<Result> UpdateDeckAsync(AddOrUpdateDeckDto deck, Guid userId)
	{
		try
		{
			var deckDomain = deck.ToEntity(userId);
			await _deckValidator.EnsureIsValid(deckDomain).ConfigureAwait(false);
			var updateResult = await _deckRepository.UpdateDeckAsync(deckDomain).ConfigureAwait(false);
			if (!updateResult.IsSuccess)
			{
				return updateResult;
			}

			await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
			return ResultFactory.Success();
		}
		catch (EntityExistsException ex)
		{
			return ResultFactory.Conflict(ex.Message);
		}
	}

	///  <inheritdoc/>
	public async Task<Result<bool>> DeleteDeckAsync(Guid deckId)
	{
		var deleted = await _deckRepository.DeleteDeckAsync(deckId).ConfigureAwait(false);
		if (deleted.IsSuccess)
		{
			await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
			return ResultFactory.Success(true);
		}

		return ResultFactory.NotFound<bool>("Unable to find the deck to delete.");
	}

	///  <inheritdoc/>
	public async Task<Result<DeckDto>> GetDeckByIdAsync(Guid deckId)
	{
		var deckResult = await _deckRepository.GetDeckByIdAsync(deckId).ConfigureAwait(false);
		if (!deckResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<DeckDto>(deckResult);
		}
		
		return ResultFactory.Success(deckResult.Value.ToDto());
	}

	///  <inheritdoc/>
	public async Task<Result<IEnumerable<DeckDto>>> GetUserDecksAsync(Guid userId)
	{
		var decksResult = await _deckRepository.GetDecksByUserIdAsync(userId).ConfigureAwait(false);
		if (!decksResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<IEnumerable<DeckDto>>(decksResult);
		}

		return ResultFactory.Success(decksResult.Value.ToDtoList());
	}

	///  <inheritdoc/>
	public async Task<Result<Guid>> AddCardAsync(AddOrUpdateCardDto card, Guid deckId)
	{
		var cardDomain = card.ToEntity(deckId);
		await _deckRepository.AddCardAsync(cardDomain).ConfigureAwait(false);
		await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
		return ResultFactory.Success(cardDomain.Id);
	}

	///  <inheritdoc/>
	public async Task<Result> UpdateCardAsync(AddOrUpdateCardDto card, Guid deckId, Guid cardId)
	{
		await _deckRepository.UpdateCardAsync(card.ToEntity(deckId, cardId)).ConfigureAwait(false);
		await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
		return ResultFactory.Success();
	}

	///  <inheritdoc/>
	public async Task<Result> DeleteCardAsync(Guid deckId, Guid cardId)
	{
		var deletedResult = await _deckRepository.DeleteCardAsync(deckId, cardId).ConfigureAwait(false);
		if (deletedResult.IsSuccess)
		{
			await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
			return ResultFactory.Success();
		}

		return deletedResult;
	}

	///  <inheritdoc/>
	public async Task<Result<CardDto>> GetCardByIdAsync(Guid deckId, Guid cardId)
	{
		var cardResult = await _deckRepository.GetCardByIdAsync(deckId, cardId).ConfigureAwait(false);
		if (!cardResult .IsSuccess)
		{
			return ResultFactory.PropagateFailure<CardDto>(cardResult);
		}
		
		return ResultFactory.Success(cardResult.Value.ToDto());
	}

	///  <inheritdoc/>
	public async Task<Result<IEnumerable<CardDto>>> GetCardsAsync(Guid deckId, int page, int pageSize)
	{
		var cardsResult = await _deckRepository.GetCardsAsync(deckId, page, pageSize).ConfigureAwait(false);
		if (!cardsResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<IEnumerable<CardDto>>(cardsResult);
		}

		return ResultFactory.Success(cardsResult.Value.ToDtoList());
	}

	///  <inheritdoc/>
	public async Task<Result<int>> GetCardCountAsync(Guid deckId)
	{
		int count = await _deckRepository.GetCardCountAsync(deckId).ConfigureAwait(false);
		return ResultFactory.Success(count);
	}

	///  <inheritdoc/>
	public async Task<Result<CardsToReviewDto>> GetCardsToReview(Guid deckId, DateTime until, int pageSize, DateTime? cursor)
	{
		if (pageSize < 1)
		{
			return ResultFactory.InvalidArgument<CardsToReviewDto>("The page number must be greater than 0.");
		}

		var cardsResult = await _deckRepository.GetCardsToReview(deckId, until, pageSize, cursor).ConfigureAwait(false);
		if (!cardsResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<CardsToReviewDto>(cardsResult);
		}
		
		return ResultFactory.Success(new CardsToReviewDto { Cards = cardsResult.Value.Cards.ToDtoList(), Count = cardsResult.Value.Count });
	}

	///  <inheritdoc/>
	public async Task<Result> ReviewCardAsync(Guid deckId, Guid cardId, bool isCorrect)
	{
		var cardResult = await _deckRepository.GetCardByIdAsync(deckId, cardId).ConfigureAwait(false);
		if (!cardResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure(cardResult);
		}
		
		_cardReviewService.UpdateReview(cardResult.Value, isCorrect);
		await _deckRepository.UpdateCardAsync(cardResult.Value).ConfigureAwait(false);
		await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
		return ResultFactory.Success();
	}
}
