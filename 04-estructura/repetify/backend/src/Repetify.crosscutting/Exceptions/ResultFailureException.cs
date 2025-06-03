using Repetify.Crosscutting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Repetify.Crosscutting.Exceptions;
/// <summary>
/// Exception thrown when an operation result indicates failure.
/// </summary>
public class ResultFailureException : Exception
{
	/// <summary>
	/// Gets the result associated with the failure, if available.
	/// </summary>
	public IResult Result { get; private set; } = new Result(ResultStatus.Unknown, "An error occurred while processing the request.");

	/// <summary>
	/// Initializes a new instance of the <see cref="ResultFailureException"/> class.
	/// </summary>
	public ResultFailureException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ResultFailureException"/> class with a specified error message.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public ResultFailureException(string? message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ResultFailureException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception.</param>
	public ResultFailureException(string? message, Exception? innerException) : base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ResultFailureException"/> class with the specified result.
	/// </summary>
	/// <param name="result">The result that caused the exception.</param>
	public ResultFailureException(IResult result)
	{
		Result = result;
	}
}
