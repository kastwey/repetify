using Microsoft.AspNetCore.Mvc;

namespace Repetify.Api.Controllers.Auth;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
	/// <summary>
	/// Logs out the user by deleting the authentication cookie.
	/// </summary>
	/// <returns>Returns an OK result.</returns>
	[HttpPost("logout")]
	public IActionResult Logout()
	{
		Response.Cookies.Delete("AuthToken");
		return Ok();
	}
}
