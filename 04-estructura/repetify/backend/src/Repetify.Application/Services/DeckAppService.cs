using Repetify.Application.Abstractions.Services;
using Repetify.Application.Dtos;
using Repetify.Application.Extensions.Mappers;
using Repetify.Crosscutting;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Abstractions.Services;
using Repetify.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

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
		var deckDomain = deck.ToEntity(userId);
		var result = await _deckValidator.EnsureIsValid(deckDomain).ConfigureAwait(false);
		if (!result.IsSuccess)
		{
			return ResultFactory.PropagateFailure<Guid>(result);
		}

		var addResult = await _deckRepository.AddDeckAsync(deckDomain).ConfigureAwait(false);
		if (!addResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<Guid>(addResult);
		}

		await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
		return ResultFactory.Success(deckDomain.Id);
	}

	///  <inheritdoc/>
	public async Task<Result> UpdateDeckAsync(Guid deckId, AddOrUpdateDeckDto deck, Guid userId)
	{
		var permissionResult = await CheckUserPermissionAsync(deckId, userId).ConfigureAwait(false);
		if (!permissionResult.IsSuccess)
		{
			return permissionResult;
		}

		var deckDomain = deck.ToEntity(userId, deckId);
		var validatorResult = await _deckValidator.EnsureIsValid(deckDomain).ConfigureAwait(false);
		if (!validatorResult.IsSuccess)
		{
			return validatorResult;
		}

		var updateResult = await _deckRepository.UpdateDeckAsync(deckDomain).ConfigureAwait(false);
		if (!updateResult.IsSuccess)
		{
			return updateResult;
		}

		await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
		return ResultFactory.Success();
	}

	///  <inheritdoc/>
	public async Task<Result> DeleteDeckAsync(Guid deckId, Guid userId)
	{
		var permissionResult = await CheckUserPermissionAsync(deckId, userId).ConfigureAwait(false);
		if (!permissionResult.IsSuccess)
		{
			return permissionResult;
		}
		
		var deletedResult = await _deckRepository.DeleteDeckAsync(deckId).ConfigureAwait(false);
		if (!deletedResult.IsSuccess)
		{
			return deletedResult;
		}
		
		await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
		return ResultFactory.Success();
	}

	///  <inheritdoc/>
	[SuppressMessage("Performance", "CA1849:Call async methods when in an async method", Justification = "The CheckPermission method does not require async calls")]
	public async Task<Result<DeckDto>> GetDeckByIdAsync(Guid deckId, Guid userId)
	{
		var deckResult = await _deckRepository.GetDeckByIdAsync(deckId).ConfigureAwait(false);
		if (!deckResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<DeckDto>(deckResult);
		}

		var permissionResult = CheckUserPermission(deckResult.Value, userId);
		if (!permissionResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<DeckDto>(permissionResult);
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
	public async Task<Result<Guid>> AddCardAsync(AddOrUpdateCardDto card, Guid deckId, Guid userId)
	{
		var permissionResult = await CheckUserPermissionAsync(deckId, userId).ConfigureAwait(false);
		if (!permissionResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<Guid>(permissionResult);
		}

		var cardDomain = card.ToEntity(deckId);
		var addResult = await _deckRepository.AddCardAsync(cardDomain).ConfigureAwait(false);
		if (!addResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<Guid>(addResult);
		}

		await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
		return ResultFactory.Success(cardDomain.Id);
	}

	///  <inheritdoc/>
	public async Task<Result> UpdateCardAsync(AddOrUpdateCardDto card, Guid deckId, Guid cardId, Guid userId)
	{
		var permissionResult = await CheckUserPermissionAsync(deckId, userId).ConfigureAwait(false);
		if (!permissionResult.IsSuccess)
		{
			return permissionResult;
		}

		var updateResult = await _deckRepository.UpdateCardAsync(card.ToEntity(deckId, cardId)).ConfigureAwait(false);
		if (!updateResult.IsSuccess)
		{
			return updateResult;
		}
		await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
		return ResultFactory.Success();
	}

	///  <inheritdoc/>
	public async Task<Result> DeleteCardAsync(Guid deckId, Guid cardId, Guid userId)
	{
		var permissionResult = await CheckUserPermissionAsync(deckId, userId).ConfigureAwait(false);
		if (!permissionResult.IsSuccess)
		{
			return permissionResult;
		}
		
		var deletedResult = await _deckRepository.DeleteCardAsync(deckId, cardId).ConfigureAwait(false);
		if (deletedResult.IsSuccess)
		{
			await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
			return ResultFactory.Success();
		}

		return deletedResult;
	}

	///  <inheritdoc/>
	public async Task<Result<CardDto>> GetCardByIdAsync(Guid deckId, Guid cardId, Guid userId)
	{
		var permissionResult = await CheckUserPermissionAsync(deckId, userId).ConfigureAwait(false);
		if (!permissionResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<CardDto>(permissionResult);
		}
		
		var cardResult = await _deckRepository.GetCardByIdAsync(deckId, cardId).ConfigureAwait(false);
		if (!cardResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<CardDto>(cardResult);
		}

		return ResultFactory.Success(cardResult.Value.ToDto());
	}

	///  <inheritdoc/>
	public async Task<Result<IEnumerable<CardDto>>> GetCardsAsync(Guid deckId, Guid userId, int page, int pageSize)
	{
		var permissionResult = await CheckUserPermissionAsync(deckId, userId).ConfigureAwait(false);
		if (!permissionResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<IEnumerable<CardDto>>(permissionResult);
		}

		var cardsResult = await _deckRepository.GetCardsAsync(deckId, page, pageSize).ConfigureAwait(false);
		if (!cardsResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<IEnumerable<CardDto>>(cardsResult);
		}

		return ResultFactory.Success(cardsResult.Value.ToDtoList());
	}

	///  <inheritdoc/>
	public async Task<Result<int>> GetCardCountAsync(Guid deckId, Guid userId)
	{
		var permissionResult = await CheckUserPermissionAsync(deckId, userId).ConfigureAwait(false);
		if (!permissionResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<int>(permissionResult);
		}

		return await _deckRepository.GetCardCountAsync(deckId).ConfigureAwait(false);
	}

	///  <inheritdoc/>
	public async Task<Result<CardsToReviewDto>> GetCardsToReview(Guid deckId, Guid userId, DateTime until, int pageSize, DateTime? cursor)
	{
		var permissionResult = await CheckUserPermissionAsync(deckId, userId).ConfigureAwait(false);
		if (pageSize < 1)
		{
			return ResultFactory.InvalidArgument<CardsToReviewDto>("The page number must be greater than 0.");
		}

		if (!permissionResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<CardsToReviewDto>(permissionResult);
		}

		var cardsResult = await _deckRepository.GetCardsToReview(deckId, until, pageSize, cursor).ConfigureAwait(false);
		if (!cardsResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure<CardsToReviewDto>(cardsResult);
		}

		return ResultFactory.Success(new CardsToReviewDto { Cards = cardsResult.Value.Cards.ToDtoList(), Count = cardsResult.Value.Count });
	}

	///  <inheritdoc/>
	public async Task<Result> ReviewCardAsync(Guid deckId, Guid cardId, Guid userId, bool isCorrect)
	{
		var permissionResult = await CheckUserPermissionAsync(deckId, userId).ConfigureAwait(false);
		if (!permissionResult.IsSuccess)
		{
			return permissionResult;
		}

		var cardResult = await _deckRepository.GetCardByIdAsync(deckId, cardId).ConfigureAwait(false);
		if (!cardResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure(cardResult);
		}

		_cardReviewService.UpdateReview(cardResult.Value, isCorrect);
		var updateResult = await _deckRepository.UpdateCardAsync(cardResult.Value).ConfigureAwait(false);
		if (!updateResult.IsSuccess)
		{
			return updateResult;
		}

		await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
		return ResultFactory.Success();
	}

	private async Task<Result> CheckUserPermissionAsync(Guid deckId, Guid userId)
	{
		var deckResult = await _deckRepository.GetDeckByIdAsync(deckId).ConfigureAwait(false);
		if (!deckResult.IsSuccess)
		{
			return ResultFactory.PropagateFailure(deckResult);
		}

		return CheckUserPermission(deckResult.Value, userId);
	}

	private static Result CheckUserPermission(Deck deck, Guid userId)
	{
		return deck.UserId == userId ?
					ResultFactory.Success() :
					ResultFactory.NotFound($"Unable to find a deck with Id {deck.Id}");
	}
}
