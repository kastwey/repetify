namespace Repetify.AuthPlatform.Exceptions;

public class InvalidTokenException : OAuthException
{
	public InvalidTokenException() { }
	public InvalidTokenException(string message) : base(message) { }
	public InvalidTokenException(string message, Exception inner) : base(message, inner) { }
}