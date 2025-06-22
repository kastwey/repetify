using System.Diagnostics.CodeAnalysis;

namespace Repetify.Crosscutting;

/// <summary>
/// Represents a result of a void operation, containing a status.
/// </summary>
public class Result : IResult
{
	private readonly List<string> _errors = new();

	/// <summary>
	/// Gets the status of the result.
	/// </summary>
	public ResultStatus Status { get; }

	/// <summary>
	/// Gets the list of error messages associated with the result.
	/// </summary>
	public IReadOnlyList<string> Errors => _errors;

	/// <summary>
	///  Gets the error message of the result.
	/// </summary>
	public string? ErrorMessage => _errors.Count > 0 ? string.Join(Environment.NewLine, _errors) : null;

	/// <summary>
	/// Initializes a new instance of the <see cref="Result{T}"/> class.
	/// </summary>
	/// <param name="status">The status of the result.</param>
	/// <param name="errorMessage">The error message of the result</param>
	internal Result(ResultStatus status, string? errorMessage)
	{
		Status = status;
		if (errorMessage is not null)
		{
			_errors.Add(errorMessage);
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Result"/> class with the specified status and a collection of error messages.
	/// </summary>
	/// <param name="status">The status of the result.</param>
	/// <param name="errors">A collection of error messages associated with the result.</param>
	internal Result(ResultStatus status, IEnumerable<string>? errors)
	{
		Status = status;

		if (errors is not null && errors.Any())
		{
			_errors.AddRange(errors);
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Result"/> class with a default status of Success.
	/// </summary>
	internal Result()
	{
		Status = ResultStatus.Success;
	}

	/// <summary>  
	/// Gets a value indicating whether the result is successful.  
	/// </summary>  
	public virtual bool IsSuccess => this.Status == ResultStatus.Success;
}
