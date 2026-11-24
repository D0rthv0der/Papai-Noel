using System.Diagnostics;
using System.Threading;
using PapaiNoelN3.Core;

namespace PapaiNoelN3.Simulations;

public static class NaiveRaceSimulation
{
    public static async Task RunAsync()
    {
        Console.WriteLine("\n--- INGÊNUA 1: condição de corrida no contador das renas ---");
        Console.WriteLine("A barreira é usada apenas para alinhar a demonstração: todas as Tasks leem o contador antes de gravar.");
        Logger.Reset();
        var timer = Stopwatch.StartNew();
        int reindeerReturned = 0;
        using var barrier = new Barrier(SimulationConfig.ReindeerCount);

        Task[] reindeerTasks = Enumerable.Range(1, SimulationConfig.ReindeerCount)
            .Select(id => Task.Factory.StartNew(() =>
            {
                Thread.Sleep(Random.Shared.Next(1, 15));
                int previous = reindeerReturned; // leitura sem proteção
                Thread.Sleep(1);                 // amplia a janela da corrida
                barrier.SignalAndWait();         // força todas a lerem o mesmo valor
                reindeerReturned = previous + 1; // gravação sem proteção
                Logger.Log($"Rena {id}", $"retornou; contador gravado = {reindeerReturned}");
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default)).ToArray();

        await Task.WhenAll(reindeerTasks);
        timer.Stop();
        Logger.Log("Papai Noel", $"verifica o contador: {reindeerReturned}/{SimulationConfig.ReindeerCount}");
        bool failed = reindeerReturned != SimulationConfig.ReindeerCount;
        Console.WriteLine(failed
            ? "[ERRO] Condição de corrida: atualizações foram perdidas; o Papai Noel não reconhece a chegada das 9 renas."
            : "[OK] A corrida não apareceu nesta execução; execute novamente.");
        Console.WriteLine($"Tempo total: {timer.Elapsed.TotalMilliseconds:F2} ms");
    }
}
