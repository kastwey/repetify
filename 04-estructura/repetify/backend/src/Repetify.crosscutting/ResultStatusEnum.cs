namespace Repetify.Crosscutting;
/// <summary>
/// Represents the status of a result.
/// </summary>
public enum ResultStatus
{
	/// <summary>
	/// Indicates a successful result.
	/// </summary>
	Success,

	/// <summary>
	/// Indicates that the requested resource was not found.
	/// </summary>
	NotFound,

	/// <summary>
	/// Indicates a conflict occurred.
	/// </summary>
	Conflict,

	/// <summary>
	/// Indicates that the provided arguments are invalid.
	/// </summary>
	InvalidArguments,
	/// <summary>
	///  Unknown status, typically used when the status cannot be determined or is not applicable.
	/// </summary>
	Unknown,
}
