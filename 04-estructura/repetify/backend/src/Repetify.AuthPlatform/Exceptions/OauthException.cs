namespace Repetify.AuthPlatform.Exceptions;


public class OAuthException : Exception
{
	public OAuthException() { }
	public OAuthException(string message) : base(message) { }
	public OAuthException(string message, Exception inner) : base(message, inner) { }
}