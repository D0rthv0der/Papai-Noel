using System.Diagnostics;

namespace PapaiNoelN3.Core;

/// <summary>
/// Centraliza o registro dos eventos da simulação.
/// </summary>
public static class Logger
{
    private static readonly object ConsoleLock = new();
    private static readonly Stopwatch Clock = Stopwatch.StartNew();

    public static void Reset()
    {
        lock (ConsoleLock)
        {
            Clock.Restart();
        }
    }

    public static void Log(string author, string message)
    {
        lock (ConsoleLock)
        {
            Console.WriteLine($"[{Clock.Elapsed:hh\\:mm\\:ss\\.fff}] {author} · {message}");
        }
    }

    // Alias mantido para compatibilidade com versões anteriores do projeto.
    public static void Info(string author, string message) => Log(author, message);

    public static Task RandomDelayAsync()
    {
        int delay = Random.Shared.Next(
            SimulationConfig.MinDelayMs,
            SimulationConfig.MaxDelayMs + 1);

        return Task.Delay(delay);
    }
}
