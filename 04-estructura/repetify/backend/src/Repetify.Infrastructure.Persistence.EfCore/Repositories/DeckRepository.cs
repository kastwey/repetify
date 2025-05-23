using Microsoft.EntityFrameworkCore;

using Repetify.Crosscutting;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Context;
using Repetify.Infrastructure.Persistence.EfCore.Extensions.Mappers;

namespace Repetify.Infrastructure.Persistence.EfCore.Repositories;

public class DeckRepository(RepetifyDbContext dbContext) : RepositoryBase(dbContext), IDeckRepository
{
	private readonly RepetifyDbContext _context = dbContext;

	public async Task<Result> AddDeckAsync(Deck deck)
	{
		await _context.Decks.AddAsync(deck.ToDataEntity()).ConfigureAwait(false);
		return ResultFactory.Success();
	}

	public async Task<Result> UpdateDeckAsync(Deck deck)
	{
		var deckEntity = await _context.Decks.FindAsync(deck.Id).ConfigureAwait(false);

		if (deckEntity is null)
		{
			return ResultFactory.NotFound($"Unable to find the deck with Id ${deck.Id}.");
		}

		deckEntity.UpdateFromDomain(deck);
		return ResultFactory.Success();
	}

	public async Task<Result> DeleteDeckAsync(Guid deckId)
	{
		if (!await _context.Decks.AnyAsync(d => d.Id == deckId).ConfigureAwait(false))
		{
			return ResultFactory.NotFound($"Deck with ID {deckId} was not found.");
		}

		if (IsInMemoryDb())
		{
			var deck = await _context.Decks.FirstOrDefaultAsync(d => d.Id == deckId).ConfigureAwait(false);
			_context.Decks.Remove(deck!);
		}
		else
		{
			await _context.Decks.Where(d => d.Id == deckId).ExecuteDeleteAsync().ConfigureAwait(false);
		}

		return ResultFactory.Success();
	}

	public async Task<Result<Deck>> GetDeckByIdAsync(Guid deckId)
	{
		var deckEntity = await _context.Decks
			.AsNoTracking()
			.FirstOrDefaultAsync(d => d.Id == deckId).ConfigureAwait(false);

		return deckEntity is null
			? ResultFactory.NotFound<Deck>($"Deck with ID {deckId} not found.")
			: ResultFactory.Success(deckEntity.ToDomain());
	}

	public async Task<Result<IEnumerable<Deck>>> GetDecksByUserIdAsync(Guid userId)
	{
		var decks = await _context.Decks
			.Where(d => d.UserId == userId)
			.AsNoTracking()
			.Select(d => d.ToDomain())
			.ToListAsync().ConfigureAwait(false);

		return ResultFactory.Success(decks.AsEnumerable());
	}

	public Task<bool> DeckNameExistsForUserAsync(Guid deckId, string name, Guid userId)
	{
		ArgumentNullException.ThrowIfNull(name);
		return _context.Decks.AnyAsync(d => d.Name == name && d.UserId == userId && d.Id != deckId);
	}

	public async Task<Result> AddCardAsync(Card card)
	{
		ArgumentNullException.ThrowIfNull(card);
		var cardEntity = CardExtensions.ToDataEntity(card);
		await _context.Cards.AddAsync(cardEntity).ConfigureAwait(false);
		return ResultFactory.Success();
	}

	public async Task<Result> UpdateCardAsync(Card card)
	{
		var cardEntity = await _context.Cards.FindAsync(card.Id).ConfigureAwait(false);

		if (cardEntity is null)
		{
			return ResultFactory.NotFound($"Unable to find a card with id ${card.Id}.");
		}

		cardEntity.UpdateFromDomain(card);
		return ResultFactory.Success();
	}

	public async Task<Result> DeleteCardAsync(Guid deckId, Guid cardId)
	{
		if (!await _context.Cards.AnyAsync(c => c.DeckId == deckId && c.Id == cardId).ConfigureAwait(false))
		{
			return ResultFactory.NotFound($"Card with ID {cardId} in deck {deckId} was not found.");
		}

		if (IsInMemoryDb())
		{
			var card = await _context.Cards
				.FirstOrDefaultAsync(c => c.DeckId == deckId && c.Id == cardId).ConfigureAwait(false);
			_context.Cards.Remove(card!);
		}
		else
		{
			await _context.Cards
				.Where(c => c.DeckId == deckId && c.Id == cardId)
				.ExecuteDeleteAsync().ConfigureAwait(false);
		}

		return ResultFactory.Success();
	}

	public async Task<Result<IEnumerable<Card>>> GetCardsAsync(Guid deckId, int page, int pageSize)
	{
		var cards = await _context.Cards
			.Where(c => c.DeckId == deckId)
			.OrderBy(c => c.NextReviewDate)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.AsNoTracking()
			.Select(c => c.ToDomain()!)
			.ToListAsync().ConfigureAwait(false);

		return ResultFactory.Success(cards.AsEnumerable());
	}

	public Task<int> GetCardCountAsync(Guid deckId) => _context.Cards
		.CountAsync(c => c.DeckId == deckId);

	public async Task<Result<Card>> GetCardByIdAsync(Guid deckId, Guid cardId)
	{
		var cardEntity = await _context.Cards
			.AsNoTracking()
			.FirstOrDefaultAsync(c => c.DeckId == deckId && c.Id == cardId).ConfigureAwait(false);

		return cardEntity is null
			? ResultFactory.NotFound<Card>($"Card with ID {cardId} not found in deck {deckId}.")
			: ResultFactory.Success(cardEntity.ToDomain()!);
	}

	public async Task<Result<(IEnumerable<Card> Cards, int? Count)>> GetCardsToReview(Guid deckId, DateTime until, int pageSize, DateTime? cursor)
	{
		int? count =
			cursor is null ? await _context.Cards.CountAsync(c => c.DeckId == deckId && c.NextReviewDate <= until).ConfigureAwait(false)
			: null;

		var query = _context.Cards
			.Where(c => c.DeckId == deckId && c.NextReviewDate <= until);

		if (cursor.HasValue)
		{
			query = query.Where(c => c.NextReviewDate > cursor);
		}

		var cards = await query
			.OrderBy(c => c.NextReviewDate)
			.Take(pageSize)
			.AsNoTracking()
			.Select(d => d.ToDomain())
			.ToListAsync().ConfigureAwait(false);

		return ResultFactory.Success((cards.AsEnumerable(), count));
	}

	public Task SaveChangesAsync() => _context.SaveChangesAsync();
}