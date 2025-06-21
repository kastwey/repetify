using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Repetify.Api.Controllers;

[Route("api/[controller]")]
public class UserController : ControllerBase
{

	[HttpGet("me")]
 	[Authorize]
	public IActionResult GetMe()
	{
		var name = User?.Identity?.Name;
		if (name is null)
		{
			return Unauthorized();
		}

		return Ok(new { name });
	}
}
