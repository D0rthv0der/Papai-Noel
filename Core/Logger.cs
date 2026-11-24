using System.Diagnostics;

namespace PapaiNoelN3.Core;

public static class Logger
{
    private static readonly object ConsoleLock = new();
    private static readonly Stopwatch Clock = Stopwatch.StartNew();

    public static void Reset() => Clock.Restart();

    public static void Log(string author, string message)
    {
        lock (ConsoleLock)
        {
            Console.WriteLine($"[{Clock.Elapsed:hh\\:mm\\:ss\\.fff}] {author} · {message}");
        }
    }

    public static Task RandomDelayAsync() =>
        Task.Delay(Random.Shared.Next(SimulationConfig.MinDelayMs, SimulationConfig.MaxDelayMs + 1));
}
