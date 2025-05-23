namespace Repetify.Domain.Exceptions;

[Serializable]
/// <summary>
/// Represents an exception that is thrown when an entity already exists with the specified field and value.
/// </summary>
public class EntityExistsException : Exception
{
	/// <summary>
	/// Gets the name of the entity that caused the exception.
	/// </summary>
	public string? Entity { get; }

	/// <summary>
	/// Gets the name of the field that caused the exception.
	/// </summary>
	public string? Field { get; }

	/// <summary>
	/// Gets the value of the field that caused the exception.
	/// </summary>
	public string? Value { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="EntityExistsException"/> class with the specified entity, field, and value.
	/// </summary>
	/// <param name="entity">The name of the entity that caused the exception.</param>
	/// <param name="field">The name of the field that caused the exception.</param>
	/// <param name="value">The value of the field that caused the exception.</param>
	public EntityExistsException(string entity, string field, string value)
		: base($"There is already a {entity} entity with the {field} {value}.")
	{
		Entity = entity;
		Field = field;
		Value = value;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="EntityExistsException"/> class.
	/// </summary>
	public EntityExistsException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="EntityExistsException"/> class with a specified error message.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public EntityExistsException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="EntityExistsException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="innerException">The exception that is the cause of the current exception.</param>
	public EntityExistsException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
