using Repetify.Crosscutting.Exceptions;
using Repetify.Crosscutting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Repetify.Crosscutting.Extensions;

public static class ResultExtensions
{
	public static void EnsureSuccess(this Result result)
	{
		ArgumentNullException.ThrowIfNull(result);

		if (!result.IsSuccess)
		{
			throw new ResultFailureException(result);
		}
	}

	public static T EnsureSuccess<T>(this Result<T> result)
	{
		ArgumentNullException.ThrowIfNull(result);

		if (!result.IsSuccess)
		{
			throw new ResultFailureException(result);
		}

		return result.Value;
	}

	public static async Task EnsureSuccessAsync(this Task<Result> resultTask)
	{
		ArgumentNullException.ThrowIfNull(resultTask);

		var result = await resultTask.ConfigureAwait(false);
		if (!result.IsSuccess)
		{
			throw new ResultFailureException(result);
		}
	}

	public static async Task<T> EnsureSuccessAsync<T>(this Task<Result<T>> resultTask)
	{
		ArgumentNullException.ThrowIfNull(resultTask);

		var result = await resultTask.ConfigureAwait(false);
		if (!result.IsSuccess)
		{
			throw new ResultFailureException(result);
		}

		return result.Value;
	}
}