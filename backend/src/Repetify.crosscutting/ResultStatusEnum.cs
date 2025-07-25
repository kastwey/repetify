﻿namespace Repetify.Crosscutting;
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
	/// Indicates that a business rule was violated.
	/// </summary>
	BusinessRuleViolated,

	/// <summary>
	/// Indicates that multiple errors have been aggregated.
	/// </summary>
	AggregatedErrors,

	/// <summary>
	///  Unknown status, typically used when the status cannot be determined or is not applicable.
	/// </summary>
	Unknown,
}
