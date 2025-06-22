namespace Repetify.Crosscutting;

/// <summary>
/// Factory class for creating <see cref="Result"/> and <see cref="Result{T}"/> instances.
/// </summary>
public static class ResultFactory
{
	/// <summary>
	/// Creates a successful <see cref="Result{T}"/> with the specified value.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value of the result.</param>
	/// <returns>A successful <see cref="Result{T}"/>.</returns>
	public static Result<T> Success<T>(T value) => new(value);

	/// <summary>
	/// Creates a successful <see cref="Result"/>.
	/// </summary>
	/// <returns>A successful <see cref="Result"/>.</returns>
	public static Result Success() => new();

	/// <summary>
	/// Creates an <see cref="Result"/> with a not found status and an optional error message.
	/// </summary>
	/// <param name="errors">The optional error message.</param>
	/// <returns>An <see cref="Result"/> with a not found status.</returns>
	public static Result NotFound(params string[] errors) => new(ResultStatus.NotFound, errors);
	
	/// <summary>
	/// Creates an <see cref="Result{T}"/> with a not found status and an optional error message.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="errorMessage">The optional error message.</param>
	/// <returns>An <see cref="Result{T}"/> with a not found status.</returns>
	public static Result<T> NotFound<T>(params string[] errors) => new Result<T>(ResultStatus.NotFound, errors, default(T));

	/// <summary>
	/// Creates an <see cref="Result"/> with an invalid argument status and an optional error message.
	/// </summary>
	/// <param name="errorMessage">The optional error message.</param>
	/// <returns>An <see cref="Result"/> with an invalid argument status.</returns>
	public static Result InvalidArgument(params string[] errors) => new(ResultStatus.InvalidArguments, errors);

	/// <summary>
	/// Creates an <see cref="Result{T}"/> with an invalid argument status and an error message.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="errorMessage">The error message of the result.</param>
	/// <returns>An <see cref="Result{T}"/> with an invalid argument status.</returns>
	public static Result<T> InvalidArgument<T>(params string[] errors) => new(ResultStatus.InvalidArguments, errors);

	/// <summary>
	/// Creates an <see cref="Result{T}"/> with an Conflict status and an error message.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="errorMessage">The error message of the result.</param>
	/// <returns>An <see cref="Result{T}"/> with an Conflict status.</returns>
	public static Result Conflict(params string[] errors) => new(ResultStatus.Conflict, errors);

	/// <summary>
	/// Creates an <see cref="Result{T}"/> with an Conflict status and an error message.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="errorMessage">The error message of the result.</param>
	/// <returns>An <see cref="Result{T}"/> with an Conflict status.</returns>
	public static Result<T> Conflict<T>(params string[] errors) => new(ResultStatus.Conflict, errors);

	public static Result BusinessRuleViolated(params string[] errors) => new(ResultStatus.BusinessRuleViolated, errors);
	
	public static Result<T> BusinessRuleViolated<T>(params string[] errors) => new(ResultStatus.BusinessRuleViolated, errors);

	/// <summary>
	/// Creates a <see cref="Result{T}"/> from an existing <see cref="IResult"/> instance.
	/// </summary>
	/// <typeparam name="T">The type of the value for the resulting <see cref="Result{T}"/>.</typeparam>
	/// <param name="result">The existing <see cref="IResult"/> instance to convert.</param>
	/// <returns>A new <see cref="Result{T}"/> with the same status and error message as the provided <see cref="IResult"/>.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="result"/> is null.</exception>
	/// <exception cref="InvalidOperationException">Thrown if the result status is success.</exception>
	public static Result<T> PropagateFailure<T>(IResult result)
	{
		EnsureIsValidToPropagate(result);
		
		return new Result<T>(result.Status, result.ErrorMessage);
	}

	/// <summary>
	/// Creates a <see cref="Result{T}"/> from an existing <see cref="IResult"/> instance.
	/// </summary>
	/// <typeparam name="T">The type of the value for the resulting <see cref="Result{T}"/>.</typeparam>
	/// <param name="result">The existing <see cref="IResult"/> instance to convert.</param>
	/// <returns>A new <see cref="Result"/> with the same status and error message as the provided <see cref="IResult"/>.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="result"/> is null.</exception>
	/// <exception cref="InvalidOperationException">Thrown if the result status is success.</exception>
	public static Result PropagateFailure(IResult result)
	{
		EnsureIsValidToPropagate(result);

		return new Result(result.Status, result.ErrorMessage);
	}

	/// <summary>
	/// Validates that the given <see cref="IResult"/> is not null and represents a failure.
	/// </summary>
	/// <param name="result">The result to validate.</param>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="result"/> is null.</exception>
	/// <exception cref="InvalidOperationException">Thrown if the <paramref name="result"/> represents a successful operation.</exception>
	private static void EnsureIsValidToPropagate(IResult result)
	{
		ArgumentNullException.ThrowIfNull(result);

		if (result.IsSuccess)
		{
			throw new InvalidOperationException("This method should be used only with failure results.");
		}
	}
}
