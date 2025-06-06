namespace Repetify.Api.Config;

public class FrontendConfig
{
	public static readonly string ConfigSection = "Frontend";

	public required Uri FrontendBaseUrl { get; init; }
}
