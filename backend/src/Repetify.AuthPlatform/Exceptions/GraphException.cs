namespace Repetify.AuthPlatform.Exceptions;

public class GraphException : OAuthException
{
	public GraphException()
	{
	}

	public GraphException(string message) : base(message)
	{
	}

	public GraphException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
