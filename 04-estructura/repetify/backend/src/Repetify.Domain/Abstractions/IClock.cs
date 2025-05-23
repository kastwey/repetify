namespace Repetify.Domain.Abstractions;

/// <summary>
/// Interface for providing the current UTC date and time.
/// </summary>
public interface IClock
{
	/// <summary>
	/// Gets the current UTC date.
	/// </summary>
	DateTime UtcNow { get; }
}
