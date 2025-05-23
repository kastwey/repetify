namespace Repetify.AuthPlatform.Exceptions;


[Serializable]
public class InvalidTokenException : OauthException
{
	public InvalidTokenException() { }
	public InvalidTokenException(string message) : base(message) { }
	public InvalidTokenException(string message, Exception inner) : base(message, inner) { }
}