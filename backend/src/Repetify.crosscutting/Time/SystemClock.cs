using Repetify.Crosscutting.Abstractions;

namespace Repetify.Crosscutting.Time;

public class SystemClock : IClock
{
	/// <Inheritdoc/>
	public DateTime UtcNow => DateTime.UtcNow;

	/// <Inheritdoc/>
	public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;
}
