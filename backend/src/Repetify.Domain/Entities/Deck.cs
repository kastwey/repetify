using Repetify.Crosscutting;

namespace Repetify.Domain.Entities;

/// <summary>
/// Represents a deck of cards for language learning.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Deck"/> class.
/// </remarks>
/// <exception cref="ArgumentNullException">Thrown when name, originalLanguage, or translatedLanguage is null.</exception>
public class Deck
{
	/// <summary>
	/// Gets or sets the unique identifier for the deck.
	/// </summary>
	public Guid Id { get; private set; }

	/// <summary>
	/// Gets the name of the deck.
	/// </summary>
	public string Name { get; private set; }

	/// <summary>
	/// Gets the description of the deck.
	/// </summary>
	public string? Description { get; private set; }

	/// <summary>
	/// Gets the unique identifier of the user who owns the deck.
	/// </summary>
	public Guid UserId { get; private set; }

	/// <summary>
	/// Gets the original language of the deck.
	/// </summary>
	public string OriginalLanguage { get; private set; }

	/// <summary>
	/// Gets the translated language of the deck.
	/// </summary>
	public string TranslatedLanguage { get; private set; }

	private Deck(Guid id, string name, string? description, Guid userId, string originalLanguage, string translatedLanguage)
	{
		Id = id;
		Name = name;
		Description = description;
		UserId = userId;
		OriginalLanguage = originalLanguage;
		TranslatedLanguage = translatedLanguage;
	}

	/// <summary>
	/// Attempts to create a new Deck instance, returning a Result&lt;Deck&gt; indicating success or failure.
	/// </summary>
	/// <param name="name">The name of the deck.</param>
	/// <param name="description">The description of the deck.</param>
	/// <param name="userId">The unique identifier of the user who owns the deck.</param>
	/// <param name="originalLanguage">The original language of the deck.</param>
	/// <param name="translatedLanguage">The translated language of the deck.</param>
	public static Result<Deck> TryCreate(
	string name,
	string? description,
	Guid userId,
	string originalLanguage,
	string translatedLanguage)
	{
		return TryCreate(null, name, description, userId, originalLanguage, translatedLanguage);
	}

	/// <summary>
	/// Attempts to create a new Deck instance, returning a Result&lt;Deck&gt; indicating success or failure.
	/// </summary>
	public static Result<Deck> TryCreate(
		Guid? id,
		string name,
		string? description,
		Guid userId,
		string originalLanguage,
		string translatedLanguage)
	{
		var errors = new List<string>();

		if (string.IsNullOrWhiteSpace(name))
		{
			errors.Add("Name cannot be null or whitespace.");
		}

		if (string.IsNullOrWhiteSpace(originalLanguage))
		{
			errors.Add("Original language cannot be null or whitespace.");
		}

		if (string.IsNullOrWhiteSpace(translatedLanguage))
		{
			errors.Add("Translated language cannot be null or whitespace.");
		}

		if (userId == Guid.Empty)
		{
			errors.Add("UserId cannot be empty.");
		}

		if (errors.Count > 0)
		{
			return ResultFactory.BusinessRuleViolated<Deck>(errors.ToArray());
		}

		var deck = new Deck(
			id ?? Guid.NewGuid(),
			name,
			description,
			userId,
			originalLanguage,
			translatedLanguage
		);

		return ResultFactory.Success(deck);
	}
}
