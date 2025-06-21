namespace Repetify.Crosscutting;

/// <summary>
/// Represents the result of an operation, including its status, error message, and success indicator.
/// </summary>
public interface IResult
{
	/// <summary>
	/// Gets the status of the result.
	/// </summary>
	public ResultStatus Status { get; }

	/// <summary>
	/// Gets the error message associated with the result, if any.
	/// </summary>
	public string? ErrorMessage { get; }

	/// <summary>
	/// Gets a value indicating whether the operation was successful.
	/// </summary>
	bool IsSuccess { get; }
}
