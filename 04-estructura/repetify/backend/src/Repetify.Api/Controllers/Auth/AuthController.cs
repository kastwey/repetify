using Microsoft.AspNetCore.Mvc;
using Repetify.Api.Constants;

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
		Response.Cookies.Delete(AuthConstants.AuthenticationCookieName);
		return Ok();
	}
}
