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

	/// <summary>
	/// Initializes a new instance of the <see cref="Deck"/> class.
	/// </summary>
	/// <param name="name">The name of the deck.</param>
	/// <param name="description">The description of the deck.</param>
	/// <param name="userId">The unique identifier of the user who owns the deck.</param>
	/// <param name="originalLanguage">The original language of the deck.</param>
	/// <param name="translatedLanguage">The translated language of the deck.</param>
	public Deck(string name, string? description, Guid userId, string originalLanguage, string translatedLanguage)
		: this(Guid.NewGuid(), name, description, userId, originalLanguage, translatedLanguage)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Deck"/> class.
	/// </summary>	
	/// <param name="id">The unique identifier for the deck.</param>
	/// <param name="name">The name of the deck.</param>
	/// <param name="description">The description of the deck.</param>
	/// <param name="userId">The unique identifier of the user who owns the deck.</param>
	/// <param name="originalLanguage">The original language of the deck.</param>
	/// <param name="translatedLanguage">The translated language of the deck.</param>
	public Deck(Guid? id, string name, string? description, Guid userId, string originalLanguage, string translatedLanguage)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);
		ArgumentException.ThrowIfNullOrWhiteSpace(originalLanguage);
		ArgumentException.ThrowIfNullOrWhiteSpace(translatedLanguage);
		Id = id ?? Guid.NewGuid();
		Name = name;
		Description = description;
		UserId = userId;
		OriginalLanguage = originalLanguage;
		TranslatedLanguage = translatedLanguage;
	}
}
