using System.Diagnostics;
using PapaiNoelN3.Core;
using PapaiNoelN3.Models;

namespace PapaiNoelN3.Simulations;

public static class IdealSantaSimulation
{
    public static async Task<SimulationReport> RunAsync(bool showReport = true)
    {
        Console.WriteLine("\n--- SOLUÇÃO IDEAL: SemaphoreSlim + lock ---");
        Logger.Reset();
        var timer = Stopwatch.StartNew();
        var state = new SharedState();
        using var cancellation = new CancellationTokenSource();
        CancellationToken token = cancellation.Token;

        Task[] reindeerTasks = Enumerable.Range(1, SimulationConfig.ReindeerCount)
            .Select(id => ReindeerAsync(id, state, token)).ToArray();
        Task[] elfTasks = Enumerable.Range(1, SimulationConfig.ElfCount)
            .Select(id => ElfAsync(id, state, token)).ToArray();
        Task santaTask = SantaAsync(state, reindeerTasks, token);

        await santaTask;
        cancellation.Cancel(); // encerra naturalmente os elfos que não formaram um grupo completo
        try { await Task.WhenAll(elfTasks); }
        catch (OperationCanceledException) { }
        await Task.WhenAll(reindeerTasks);
        timer.Stop();

        var report = new SimulationReport
        {
            Solution = "IDEAL (SemaphoreSlim + lock)",
            ReindeerReturned = state.ReindeerReturned,
            ReindeerServed = state.ReindeerServed,
            ElvesServed = state.ElvesServed,
            ElfGroupsServed = state.ElvesServed / SimulationConfig.ElfGroupSize,
            MaxElvesWaiting = state.MaxElvesWaiting,
            MaxConcurrentGroups = state.MaxConcurrentGroups,
            Elapsed = timer.Elapsed
        };

        if (showReport)
        {
            report.Print();
            PrintChecks(report);
        }
        return report;
    }

    private static async Task ReindeerAsync(int id, SharedState state, CancellationToken token)
    {
        await Logger.RandomDelayAsync();
        bool last;
        lock (state.Gate)
        {
            state.ReindeerReturned++;
            last = state.ReindeerReturned == SimulationConfig.ReindeerCount;
            Logger.Log($"Rena {id}", $"retornou ({state.ReindeerReturned}/{SimulationConfig.ReindeerCount})");
        }
        if (last) state.SantaWake.Release();

        await state.ReindeerRelease.WaitAsync(token);
        lock (state.Gate)
        {
            state.ReindeerServed++;
            Logger.Log($"Rena {id}", "foi atrelada ao trenó");
        }
    }

    private static async Task ElfAsync(int id, SharedState state, CancellationToken token)
    {
        await Logger.RandomDelayAsync();
        await state.ElfSlots.WaitAsync(token);
        bool admitted;
        lock (state.Gate)
        {
            admitted = !state.ReindeerReleased;
            if (admitted)
            {
                state.ElvesWaiting++;
                state.MaxElvesWaiting = Math.Max(state.MaxElvesWaiting, state.ElvesWaiting);
                Logger.Log($"Elfo {id}", $"aguarda atendimento ({state.ElvesWaiting}/{SimulationConfig.ElfGroupSize})");
            }
        }

        if (!admitted)
        {
            state.ElfSlots.Release();
            return;
        }

        // O terceiro elfo acorda o Papai Noel. O sinal fica armazenado no semáforo.
        bool wakeSanta;
        lock (state.Gate) wakeSanta = state.ElvesWaiting == SimulationConfig.ElfGroupSize;
        if (wakeSanta) state.SantaWake.Release();

        await state.ElfRelease.WaitAsync(token);
        lock (state.Gate)
        {
            state.ElvesServed++;
            Logger.Log($"Elfo {id}", "recebeu ajuda do Papai Noel");
            if (state.ElvesServed % SimulationConfig.ElfGroupSize == 0)
                state.ElfGroupDone.Release();
        }
        state.ElfSlots.Release();
    }

    private static async Task SantaAsync(SharedState state, Task[] reindeerTasks, CancellationToken token)
    {
        while (true)
        {
            await state.SantaWake.WaitAsync(token);
            bool helpElves = false;
            bool prepareSleigh = false;

            lock (state.Gate)
            {
                // Prioridade das renas: se as nove chegaram, não inicia outro grupo de elfos.
                if (state.ReindeerReturned == SimulationConfig.ReindeerCount && !state.ReindeerReleased)
                {
                    state.ReindeerReleased = true;
                    prepareSleigh = true;
                }
                else if (state.ElvesWaiting == SimulationConfig.ElfGroupSize)
                {
                    state.ElvesWaiting -= SimulationConfig.ElfGroupSize;
                    state.ActiveGroups++;
                    state.MaxConcurrentGroups = Math.Max(state.MaxConcurrentGroups, state.ActiveGroups);
                    helpElves = true;
                }
            }

            if (prepareSleigh)
            {
                Logger.Log("Papai Noel", "as 9 renas chegaram; preparando o trenó (prioridade máxima)");
                state.ReindeerRelease.Release(SimulationConfig.ReindeerCount);
                await Task.WhenAll(reindeerTasks);
                Logger.Log("Papai Noel", "partiu para a entrega dos presentes");
                return;
            }

            if (helpElves)
            {
                Logger.Log("Papai Noel", "atendendo exatamente 3 elfos");
                state.ElfRelease.Release(SimulationConfig.ElfGroupSize);
                await state.ElfGroupDone.WaitAsync(token);
                lock (state.Gate) state.ActiveGroups--;
            }
        }
    }

    private static void PrintChecks(SimulationReport report)
    {
        Console.WriteLine("\n--- VERIFICAÇÕES AUTOMÁTICAS ---");
        Check(report.ReindeerReturned == SimulationConfig.ReindeerCount &&
              report.ReindeerServed == SimulationConfig.ReindeerCount,
              "As 9 renas retornaram e foram atendidas");
        Check(report.ElvesServed % SimulationConfig.ElfGroupSize == 0,
              "Todos os elfos atendidos pertencem a grupos completos de 3");
        Check(report.MaxElvesWaiting <= SimulationConfig.ElfGroupSize && report.MaxConcurrentGroups <= 1,
              "No máximo 3 elfos aguardam e apenas 1 grupo é atendido por vez");
    }

    private static void Check(bool condition, string description) =>
        Console.WriteLine(condition ? $"[OK] {description}" : $"[ERRO] {description}");

    private sealed class SharedState
    {
        public readonly object Gate = new();
        public readonly SemaphoreSlim SantaWake = new(0);
        public readonly SemaphoreSlim ReindeerRelease = new(0);
        public readonly SemaphoreSlim ElfRelease = new(0);
        public readonly SemaphoreSlim ElfGroupDone = new(0);
        public readonly SemaphoreSlim ElfSlots = new(SimulationConfig.ElfGroupSize, SimulationConfig.ElfGroupSize);
        public int ReindeerReturned;
        public int ReindeerServed;
        public int ElvesWaiting;
        public int ElvesServed;
        public int MaxElvesWaiting;
        public int ActiveGroups;
        public int MaxConcurrentGroups;
        public bool ReindeerReleased;
    }
}
