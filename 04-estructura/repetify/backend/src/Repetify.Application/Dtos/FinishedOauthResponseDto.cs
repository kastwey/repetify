using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repetify.Application.Dtos;

public sealed class FinishedOAuthResponseDto
{
	public required string JwtToken { get; init; }

	public required Uri ReturnUrl { get; init; }
}
