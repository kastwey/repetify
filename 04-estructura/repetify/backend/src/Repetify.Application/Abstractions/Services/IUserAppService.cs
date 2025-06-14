using Repetify.Application.Dtos;
using Repetify.Crosscutting;
using Repetify.Crosscutting.OAuth;

namespace Repetify.Application.Abstractions.Services;

/// <summary>
/// Defines operations for managing user data within the system.
/// </summary>
public interface IUserAppService
{
	/// <summary>
	/// Retrieves a user by their email address.
	/// </summary>
	/// <param name="email">The email address of the user to retrieve.</param>
	/// <returns>A <see cref="Result{T}"/> containing the user data if found, or an appropriate error status.</returns>
	Task<Result<UserDto>> GetUserByEmailAsync(string email);

	/// <summary>
	/// Gets the URI to initiate the OAuth sign-in process for the specified identity provider.
	/// </summary>
	/// <param name="provider">The identity provider to use for OAuth sign-in.</param>
	/// <returns>
	/// A <see cref="Result{T}"/> containing the URI to redirect the user to for OAuth authentication,
	/// or an appropriate error status if the URI could not be generated.
	/// </returns>
	Result<Uri> InitiateOAuthSignin(IdentityProvider provider, Uri? returnUrl = null);

	/// <summary>
	/// Completes the OAuth authentication flow for the specified identity provider.
	/// </summary>
	/// <param name="provider">The identity provider used for OAuth authentication.</param>
	/// <param name="code">The authorization code received from the OAuth provider.</param>
	/// <returns>
	/// A <see cref="Result{T}"/> containing a <see cref="FinishedOAuthResponseDto"/> with a JWT token (valid in our app)
	/// and a redirect URI for the controller to redirect the user if necessary.
	/// The method exchanges the code for a token, verifies if a user with the email in the token claim exists,
	/// creates a new user if not, and returns the appropriate response DTO.
	/// </returns>
	Task<Result<FinishedOAuthResponseDto>> FinishOAuthFlowAsync(IdentityProvider provider, string code, Uri? returnUrl = null);

}
