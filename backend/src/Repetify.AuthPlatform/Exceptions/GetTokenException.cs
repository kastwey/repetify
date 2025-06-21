namespace Repetify.AuthPlatform.Exceptions;


[Serializable]
public class GetTokenException : OAuthException
{
	public GetTokenException() { }
	public GetTokenException(string message) : base(message) { }
	public GetTokenException(string message, Exception inner) : base(message, inner) { }
}