using Repetify.Crosscutting.Abstractions;

namespace Repetify.Crosscutting.Time;

public static class Clock
{
	private static IClock? _current;

	public static IClock Current => _current ??= new SystemClock();

	public static void Set(IClock clock) => _current = clock;

	public static void Reset() => _current = new SystemClock();
}
