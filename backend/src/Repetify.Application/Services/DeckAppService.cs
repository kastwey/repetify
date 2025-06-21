using Repetify.Application.Abstractions.Services;
using Repetify.Application.Dtos;
using Repetify.Application.Extensions.Mappers;
using Repetify.Crosscutting;
using Repetify.Crosscutting.Exceptions;
using Repetify.Crosscutting.Extensions;
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

	public DeckAppService(
		IDeckValidator deckValidator,
		ICardReviewService cardReviewService,
		IDeckRepository deckRepository)
	{
		_deckValidator = deckValidator ?? throw new ArgumentNullException(nameof(deckValidator));
		_cardReviewService = cardReviewService ?? throw new ArgumentNullException(nameof(cardReviewService));
		_deckRepository = deckRepository ?? throw new ArgumentNullException(nameof(deckRepository));
	}

	public async Task<Result<Guid>> AddDeckAsync(AddOrUpdateDeckDto deck, Guid userId)
	{
		try
		{
			var deckDomain = deck.ToEntity(userId);
			await _deckValidator.EnsureIsValidAsync(deckDomain).EnsureSuccessAsync().ConfigureAwait(false);
			await _deckRepository.AddDeckAsync(deckDomain).EnsureSuccessAsync().ConfigureAwait(false);
			await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
			return ResultFactory.Success(deckDomain.Id);
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure<Guid>(ex.Result);
		}
	}

	public async Task<Result> UpdateDeckAsync(Guid deckId, AddOrUpdateDeckDto deck, Guid userId)
	{
		ArgumentNullException.ThrowIfNull(deck);

		try
		{
			await CheckUserPermissionAsync(deckId, userId).EnsureSuccessAsync().ConfigureAwait(false);
			var deckDomain = deck.ToEntity(userId, deckId);
			await _deckValidator.EnsureIsValidAsync(deckDomain).EnsureSuccessAsync().ConfigureAwait(false);
			await _deckRepository.UpdateDeckAsync(deckDomain).EnsureSuccessAsync().ConfigureAwait(false);
			await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
			return ResultFactory.Success();
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure(ex.Result);
		}
	}

	public async Task<Result> DeleteDeckAsync(Guid deckId, Guid userId)
	{
		try
		{
			await CheckUserPermissionAsync(deckId, userId).EnsureSuccessAsync().ConfigureAwait(false);
			await _deckRepository.DeleteDeckAsync(deckId).EnsureSuccessAsync().ConfigureAwait(false);
			await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
			return ResultFactory.Success();
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure(ex.Result);
		}
	}

	[SuppressMessage("Performance", "CA1849:Call async methods when in an async method", Justification = "The CheckPermission method does not require async calls")]
	public async Task<Result<DeckDto>> GetDeckByIdAsync(Guid deckId, Guid userId)
	{
		try
		{
			var deck = await _deckRepository.GetDeckByIdAsync(deckId).EnsureSuccessAsync().ConfigureAwait(false);
			CheckUserPermission(deck, userId).EnsureSuccess();
			return ResultFactory.Success(deck.ToDto());
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure<DeckDto>(ex.Result);
		}
	}

	public async Task<Result<IEnumerable<DeckDto>>> GetUserDecksAsync(Guid userId)
	{
		try
		{
			var decks = await _deckRepository.GetDecksByUserIdAsync(userId).EnsureSuccessAsync().ConfigureAwait(false);
			return ResultFactory.Success(decks.ToDtoList());
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure<IEnumerable<DeckDto>>(ex.Result);
		}
	}

	public async Task<Result<Guid>> AddCardAsync(AddOrUpdateCardDto card, Guid deckId, Guid userId)
	{
		try
		{
			await CheckUserPermissionAsync(deckId, userId).EnsureSuccessAsync().ConfigureAwait(false);
			var cardDomain = card.ToEntity(deckId);
			await _deckRepository.AddCardAsync(cardDomain).EnsureSuccessAsync().ConfigureAwait(false);
			await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
			return ResultFactory.Success(cardDomain.Id);
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure<Guid>(ex.Result);
		}
	}

	public async Task<Result> UpdateCardAsync(AddOrUpdateCardDto card, Guid deckId, Guid cardId, Guid userId)
	{
		try
		{
			await CheckUserPermissionAsync(deckId, userId).EnsureSuccessAsync().ConfigureAwait(false);
			await _deckRepository.UpdateCardAsync(card.ToEntity(deckId, cardId)).EnsureSuccessAsync().ConfigureAwait(false);
			await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
			return ResultFactory.Success();
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure(ex.Result);
		}
	}

	public async Task<Result> DeleteCardAsync(Guid deckId, Guid cardId, Guid userId)
	{
		try
		{
			await CheckUserPermissionAsync(deckId, userId).EnsureSuccessAsync().ConfigureAwait(false);
			await _deckRepository.DeleteCardAsync(deckId, cardId).EnsureSuccessAsync().ConfigureAwait(false);
			await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
			return ResultFactory.Success();
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure(ex.Result);
		}
	}

	public async Task<Result<CardDto>> GetCardByIdAsync(Guid deckId, Guid cardId, Guid userId)
	{
		try
		{
			await CheckUserPermissionAsync(deckId, userId).EnsureSuccessAsync().ConfigureAwait(false);
			var card = await _deckRepository.GetCardByIdAsync(deckId, cardId).EnsureSuccessAsync().ConfigureAwait(false);
			return ResultFactory.Success(card.ToDto());
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure<CardDto>(ex.Result);
		}
	}

	public async Task<Result<IEnumerable<CardDto>>> GetCardsAsync(Guid deckId, Guid userId, int page, int pageSize)
	{
		try
		{
			await CheckUserPermissionAsync(deckId, userId).EnsureSuccessAsync().ConfigureAwait(false);
			var cards = await _deckRepository.GetCardsAsync(deckId, page, pageSize).EnsureSuccessAsync().ConfigureAwait(false);
			return ResultFactory.Success(cards.ToDtoList());
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure<IEnumerable<CardDto>>(ex.Result);
		}
	}

	public async Task<Result<int>> GetCardCountAsync(Guid deckId, Guid userId)
	{
		try
		{
			await CheckUserPermissionAsync(deckId, userId).EnsureSuccessAsync().ConfigureAwait(false);
			var count = await _deckRepository.GetCardCountAsync(deckId).EnsureSuccessAsync().ConfigureAwait(false);
			return ResultFactory.Success(count);
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure<int>(ex.Result);
		}
	}

	public async Task<Result<CardsToReviewDto>> GetCardsToReview(Guid deckId, Guid userId, DateTime until, int pageSize, DateTime? cursor)
	{
		if (pageSize < 1)
		{
			return ResultFactory.InvalidArgument<CardsToReviewDto>("The page number must be greater than 0.");
		}

		try
		{
			await CheckUserPermissionAsync(deckId, userId).EnsureSuccessAsync().ConfigureAwait(false);
			var cardsResult = await _deckRepository.GetCardsToReview(deckId, until, pageSize, cursor).EnsureSuccessAsync().ConfigureAwait(false);
			return ResultFactory.Success(new CardsToReviewDto { Cards = cardsResult.Cards.ToDtoList(), Count = cardsResult.Count });
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure<CardsToReviewDto>(ex.Result);
		}
	}

	public async Task<Result> ReviewCardAsync(Guid deckId, Guid cardId, Guid userId, bool isCorrect)
	{
		try
		{
			await CheckUserPermissionAsync(deckId, userId).EnsureSuccessAsync().ConfigureAwait(false);
			var card = await _deckRepository.GetCardByIdAsync(deckId, cardId).EnsureSuccessAsync().ConfigureAwait(false);
			_cardReviewService.UpdateReview(card, isCorrect);
			await _deckRepository.UpdateCardAsync(card).EnsureSuccessAsync().ConfigureAwait(false);
			await _deckRepository.SaveChangesAsync().ConfigureAwait(false);
			return ResultFactory.Success();
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure(ex.Result);
		}
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
