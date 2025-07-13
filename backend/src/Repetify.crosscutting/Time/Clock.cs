using System.Threading;

using Repetify.Crosscutting.Abstractions;

namespace Repetify.Crosscutting.Time;

public static class Clock
{
	private static readonly AsyncLocal<IClock?> _current = new();

	public static IClock Current => _current.Value ??= new SystemClock();

	public static void Set(IClock clock) => _current.Value = clock;

	public static void Reset() => _current.Value = null;
}
