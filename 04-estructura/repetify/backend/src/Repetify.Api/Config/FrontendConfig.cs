namespace Repetify.Api.Config;

public record class FrontendConfig
{
	public static readonly string ConfigSection = "Frontend";

	public required Uri FrontendBaseUrl { get; set; }
}
