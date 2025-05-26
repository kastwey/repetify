using Microsoft.EntityFrameworkCore;

using Repetify.Crosscutting;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Context;
using Repetify.Infrastructure.Persistence.EfCore.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Extensions.Mappers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repetify.Infrastructure.Persistence.EfCore.Repositories;

/// <summary>
/// Provides an EF Core implementation of <see cref="IDeckRepository"/> for managing decks and cards in the persistence layer.
/// </summary>
public class DeckRepository(RepetifyDbContext context) : RepositoryBase(context), IDeckRepository
{
	private readonly RepetifyDbContext _context = context;

	/// <inheritdoc/>
	public async Task<Result> AddDeckAsync(Deck deck)
	{
		await _context.Decks.AddAsync(deck.ToDataEntity()).ConfigureAwait(false);
		return ResultFactory.Success();
	}

	/// <inheritdoc/>
	public async Task<Result> UpdateDeckAsync(Deck deck)
	{
		ArgumentNullException.ThrowIfNull(deck);

		var deckEntity = await _context.Decks.FindAsync(deck.Id).ConfigureAwait(false);

		if (deckEntity is null)
		{
			return ResultFactory.NotFound($"Unable to find a deck with Id {deck.Id}.");
		}

		deckEntity.UpdateFromDomain(deck);
		return ResultFactory.Success();
	}

	/// <inheritdoc/>
	public async Task<Result> DeleteDeckAsync(Guid deckId)
	{
		if (!await _context.Decks.AnyAsync(d => d.Id == deckId).ConfigureAwait(false))
		{
			return ResultFactory.NotFound($"Unable to find a deck with Id {deckId}");
		}

		if (IsInMemoryDb())
		{
			var deck = await _context.Decks.FindAsync(deckId).ConfigureAwait(false);
			_context.Decks.Remove(deck!);
		}
		else
		{
			await _context.Decks.Where(d => d.Id == deckId)
				.ExecuteDeleteAsync().ConfigureAwait(false);
		}

		return ResultFactory.Success();
	}

	/// <inheritdoc/>
	public async Task<Result<Deck>> GetDeckByIdAsync(Guid deckId)
	{
		var deckEntity = await _context.Decks
			.AsNoTracking()
			.FirstOrDefaultAsync(d => d.Id == deckId)
			.ConfigureAwait(false);

		return deckEntity is null ?
			ResultFactory.NotFound<Deck>($"Unable to find a deck with Id {deckId}.")
			: ResultFactory.Success(deckEntity.ToDomain());
	}

	/// <inheritdoc/>
	public async Task<Result<IEnumerable<Deck>>> GetDecksByUserIdAsync(Guid userId)
	{
		var decks = await _context.Decks.Where(d => d.UserId == userId)
					.AsNoTracking()
					.Select(d => d.ToDomain())
					.ToListAsync()
					.ConfigureAwait(false);

		return ResultFactory.Success(decks.AsEnumerable());
	}

	/// <inheritdoc/>
	public async Task<Result<bool>> DeckNameExistsForUserAsync(Guid deckId, string name, Guid userId)
	{
		ArgumentNullException.ThrowIfNullOrWhiteSpace(name);

		return ResultFactory.Success(
			await _context.Decks.AnyAsync(d => d.Name == name && d.UserId == userId && d.Id != deckId).ConfigureAwait(false));
	}

	/// <inheritdoc/>
	public async Task<Result> AddCardAsync(Card card)
	{
		ArgumentNullException.ThrowIfNull(card);
		var cardEntity = card.ToDataEntity();
		await _context.Cards.AddAsync(cardEntity).ConfigureAwait(false);

		return ResultFactory.Success();
	}

	/// <inheritdoc/>
	public async Task<Result> UpdateCardAsync(Card card)
	{
		ArgumentNullException.ThrowIfNull(card);
		var cardEntity = await _context.Cards.FindAsync(card.Id).ConfigureAwait(false);

		if (cardEntity is null)
		{
			return ResultFactory.NotFound($"Unable to find a card with Id {card.Id}.");
		}

		cardEntity.UpdateFromDomain(card);
		return ResultFactory.Success();
	}

	/// <inheritdoc/>
	public async Task<Result> DeleteCardAsync(Guid deckId, Guid cardId)
	{
		if (!await _context.Cards.AnyAsync(c => c.Id == cardId && c.DeckId == deckId).ConfigureAwait(false))
		{
			return ResultFactory.NotFound($"Unable to find a card with Id {cardId} in the deck with Id {deckId}.");
		}

		if (IsInMemoryDb())
		{
			var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == cardId && c.DeckId == deckId).ConfigureAwait(false);
			_context.Remove(card!);
		}
		else
		{
			await _context.Cards.Where(c => c.DeckId == deckId && c.Id == cardId)
				.ExecuteDeleteAsync().ConfigureAwait(false);
		}

		return ResultFactory.Success();
	}

	/// <inheritdoc/>
	public async Task<Result<Card>> GetCardByIdAsync(Guid deckId, Guid cardId)
	{
		var cardEntity = await _context.Cards.AsNoTracking()
			.FirstOrDefaultAsync(c => c.Id == cardId && c.DeckId == deckId)
			.ConfigureAwait(false);

		return cardEntity is null ?
			ResultFactory.NotFound<Card>($"Unable to find a card with Id {cardId} in the deck with Id {deckId}.")
			: ResultFactory.Success(cardEntity.ToDomain());
	}

	/// <inheritdoc/>
	public async Task<Result<int>> GetCardCountAsync(Guid deckId) =>
		ResultFactory.Success(
			await _context.Cards.CountAsync(c => c.DeckId == deckId)
			.ConfigureAwait(false));

	/// <inheritdoc/>
	public async Task<Result<IEnumerable<Card>>> GetCardsAsync(Guid deckId, int page, int pageSize)
	{
		if (page < 1)
		{
			return ResultFactory.InvalidArgument<IEnumerable<Card>>("The page number should be greater than 1.");
		}

		if (pageSize < 1)
		{
			return ResultFactory.InvalidArgument<IEnumerable<Card>>("Pge size should be greater than 1.");
		}

		var cards = await _context.Cards.Where(c => c.DeckId == deckId)
			.OrderBy(c => c.NextReviewDate)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.AsNoTracking()
			.Select(c => c.ToDomain())
			.ToListAsync()
			.ConfigureAwait(false);

		return ResultFactory.Success(cards.AsEnumerable());
	}

	/// <inheritdoc/>
	public async Task<Result<(IEnumerable<Card> Cards, int? Count)>> GetCardsToReview(Guid deckId, DateTime until, int pageSize, DateTime? cursor)
	{
		if (pageSize < 1)
		{
			return ResultFactory.InvalidArgument<(IEnumerable<Card>, int?)>("Pge size should be greater than 1.");
		}

		int? count = cursor is null ?
			await _context.Cards.CountAsync(c => c.DeckId == deckId && c.NextReviewDate <= until).ConfigureAwait(false)
			: null;

		var query = _context.Cards.Where(c => c.DeckId == deckId && c.NextReviewDate <= until);

		if (cursor is not null)
		{
			query = query.Where(c => c.NextReviewDate > cursor);
		}

		var cards = await query
			.OrderBy(c => c.NextReviewDate)
			.Take(pageSize)
			.AsNoTracking()
			.Select(c => c.ToDomain())
			.ToListAsync().ConfigureAwait(false);

		return ResultFactory.Success((cards.AsEnumerable(), count));
	}

	/// <inheritdoc/>
	public async Task SaveChangesAsync() => await _context.SaveChangesAsync().ConfigureAwait(false);
}
