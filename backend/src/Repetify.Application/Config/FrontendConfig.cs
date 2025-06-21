namespace Repetify.Application.Config;

/// <summary>
/// Represents the configuration settings for the frontend application.
/// </summary>
public class FrontendConfig
{
	/// <summary>
	/// The configuration section name for the frontend settings.
	/// </summary>
	public static readonly string ConfigSection = "Frontend";

	/// <summary>
	/// Gets the base URL of the frontend application.
	/// </summary>
	public required Uri FrontendBaseUrl { get; init; }
}
