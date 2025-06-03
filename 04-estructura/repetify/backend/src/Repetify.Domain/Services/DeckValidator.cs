using Repetify.Crosscutting;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Abstractions.Services;
using Repetify.Domain.Entities;

namespace Repetify.Domain.Services;

/// <summary>
/// Validates deck entities to ensure they meet specific rules.
/// </summary>
public class DeckValidator : IDeckValidator
{
	private IDeckRepository _deckRepository;

	/// <summary>
	/// Initializes a new instance of the <see cref="DeckValidator"/> class.
	/// </summary>
	/// <param name="deckRepository">The deck repository.</param>
	public DeckValidator(IDeckRepository deckRepository)
	{
		_deckRepository = deckRepository;
	}

	/// <inheritdoc/>
	public async Task<Result> EnsureIsValidAsync(Deck deck)
	{
		return await EnsureNameIsUnique(deck).ConfigureAwait(false);
	}

	/// <summary>
	/// Ensures that the deck name is unique for the user.
	/// </summary>
	/// <param name="deck">The deck to validate.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the deck is null.</exception>
	private async Task<Result> EnsureNameIsUnique(Deck deck)
	{
		ArgumentNullException.ThrowIfNull(deck);

		var result = await _deckRepository.DeckNameExistsForUserAsync(deck.Id, deck.Name, deck.UserId).ConfigureAwait(false);
		if (!result.IsSuccess)
		{
			return ResultFactory.PropagateFailure(result);
		}
		
		return result.Value ? ResultFactory.Conflict($"A deck with the name '{deck.Name}' already exists for this user.")
			: ResultFactory.Success();
	}
}
