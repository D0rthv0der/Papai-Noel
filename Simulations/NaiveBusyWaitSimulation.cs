using System.Diagnostics;
using System.Threading;
using PapaiNoelN3.Core;

namespace PapaiNoelN3.Simulations;

public static class NaiveBusyWaitSimulation
{
    public static async Task<TimeSpan> RunAsync(bool showReport = true)
    {
        Console.WriteLine("\n--- INGÊNUA 2: espera ocupada ---");
        Logger.Reset();
        var timer = Stopwatch.StartNew();
        int reindeerReturned = 0;
        int elvesArrived = 0;
        int elvesWaiting = 0;
        int elvesServed = 0;
        int maxWaiting = 0;
        int reindeerServed = 0;

        Task[] reindeerTasks = Enumerable.Range(1, SimulationConfig.ReindeerCount)
            .Select(async id =>
            {
                await Logger.RandomDelayAsync();
                int total = Interlocked.Increment(ref reindeerReturned);
                Logger.Log($"Rena {id}", $"retornou ({total}/{SimulationConfig.ReindeerCount})");
            }).ToArray();

        Task[] elfTasks = Enumerable.Range(1, SimulationConfig.ElfCount)
            .Select(async id =>
            {
                await Logger.RandomDelayAsync();
                int ticket = Interlocked.Increment(ref elvesArrived);
                int waiting = Interlocked.Increment(ref elvesWaiting);
                UpdateMaximum(ref maxWaiting, waiting);
                Logger.Log($"Elfo {id}", $"aguarda ajuda; posição {ticket}");

                var spin = new SpinWait();
                while (Volatile.Read(ref elvesServed) < ticket &&
                       Volatile.Read(ref reindeerReturned) < SimulationConfig.ReindeerCount)
                {
                    spin.SpinOnce(); // consome CPU enquanto aguarda
                }

                if (Volatile.Read(ref elvesServed) >= ticket)
                    Logger.Log($"Elfo {id}", "recebeu ajuda");
                else
                    Logger.Log($"Elfo {id}", "ficou sem atendimento quando o Papai Noel partiu com as renas");
            }).ToArray();

        // LongRunning deixa explícito o trabalho contínuo de espera ocupada do Papai Noel.
        Task santaTask = Task.Factory.StartNew(() =>
        {
            var spin = new SpinWait();
            while (Volatile.Read(ref reindeerReturned) < SimulationConfig.ReindeerCount)
            {
                if (Volatile.Read(ref elvesWaiting) >= SimulationConfig.ElfGroupSize &&
                    Volatile.Read(ref reindeerReturned) < SimulationConfig.ReindeerCount)
                {
                    Interlocked.Add(ref elvesWaiting, -SimulationConfig.ElfGroupSize);
                    int served = Interlocked.Add(ref elvesServed, SimulationConfig.ElfGroupSize);
                    Logger.Log("Papai Noel", $"atendeu um grupo de 3 elfos (total atendido: {served})");
                }
                else
                {
                    spin.SpinOnce(); // não dorme nem aguarda sinal: fica consultando os contadores
                }
            }

            reindeerServed = SimulationConfig.ReindeerCount;
            Logger.Log("Papai Noel", "as 9 renas chegaram; prioridade para a viagem de Natal");
        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        await Task.WhenAll(reindeerTasks);
        await santaTask;
        await Task.WhenAll(elfTasks);
        timer.Stop();

        int groups = elvesServed / SimulationConfig.ElfGroupSize;
        if (showReport)
        {
            Console.WriteLine("[PROBLEMA] O Papai Noel consulta os contadores continuamente, gastando CPU sem realizar trabalho útil.");
            Console.WriteLine($"Renas atendidas: {reindeerServed}; elfos atendidos: {elvesServed}; máximo de elfos na fila observado: {maxWaiting}.");
            Console.WriteLine($"Tempo total: {timer.Elapsed.TotalMilliseconds:F2} ms");
        }
        return timer.Elapsed;
    }

    private static void UpdateMaximum(ref int maximum, int value)
    {
        int current;
        do
        {
            current = Volatile.Read(ref maximum);
            if (value <= current) return;
        } while (Interlocked.CompareExchange(ref maximum, value, current) != current);
    }
}
