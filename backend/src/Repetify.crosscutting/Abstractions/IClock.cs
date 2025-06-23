namespace Repetify.Crosscutting.Abstractions;

/// <summary>
/// Interface for providing the current UTC date and time.
/// </summary>
public interface IClock
{
	/// <summary>
	/// Gets the current UTC date.
	/// </summary>
	DateTime UtcNow { get; }

	/// <summary>
	///  Gets the current UTC date in a DAteTimeOffset date type.
	/// </summary>
	DateTimeOffset OffsetUtcNow { get; }
}
