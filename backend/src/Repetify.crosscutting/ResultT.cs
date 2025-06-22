using Repetify.Crosscutting;

using System.Diagnostics.CodeAnalysis;

namespace Repetify.Crosscutting;

/// <summary>
/// Represents a result of an operation, containing a value and a status.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public class Result<T> : Result, IResult
{
	/// <summary>
	/// Gets the value of the result.
	/// </summary>
	public T? Value { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Result{T}"/> class.
	/// </summary>
	/// <param name="value">The value of the result.</param>
	internal Result(T value) : base()
	{
		Value = value;
	}

	/// <summary>
	///  Initializes a new instance of the <see cref="Result{T}"/> class
	/// </summary>
	/// <param name="status">The operation status</param>
	/// <param name="errorMessage">The error message</param>
	internal Result(ResultStatus status, string? errorMessage = null, T? value = default(T)) : base(status, errorMessage)
	{
		Value = value;
	}

	internal Result(ResultStatus status, IEnumerable<string>? errors = null, T? value = default(T)) : base(status, errors)
	{
		Value = value;
	}

	[MemberNotNullWhen(true, nameof(Value))]
	public override bool IsSuccess => base.IsSuccess;
}