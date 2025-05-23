namespace Repetify.AuthPlatform.Exceptions;


[Serializable]
public class OauthException : Exception
{
	public OauthException() { }
	public OauthException(string message) : base(message) { }
	public OauthException(string message, Exception inner) : base(message, inner) { }
}