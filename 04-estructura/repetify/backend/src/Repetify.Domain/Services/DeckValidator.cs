using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Abstractions.Services;
using Repetify.Domain.Entities;
using Repetify.Domain.Exceptions;

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
	public async Task EnsureIsValid(Deck deck)
	{
		await EnsureNameIsUnique(deck).ConfigureAwait(false);
	}

	/// <summary>
	/// Ensures that the deck name is unique for the user.
	/// </summary>
	/// <param name="deck">The deck to validate.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the deck is null.</exception>
	/// <exception cref="EntityExistsException">Thrown when a deck with the same name already exists for the user.</exception>
	private async Task EnsureNameIsUnique(Deck deck)
	{
		ArgumentNullException.ThrowIfNull(deck);

		if (await _deckRepository.DeckNameExistsForUserAsync(deck.Id, deck.Name, deck.UserId).ConfigureAwait(false))
		{
			throw new EntityExistsException("Deck", "Name", deck.Name);
		}
	}
}
