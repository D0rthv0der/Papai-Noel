namespace PapaiNoelN3.Models;

public sealed class SimulationReport
{
    public string Solution { get; init; } = "";
    public int ReindeerReturned { get; init; }
    public int ReindeerServed { get; init; }
    public int ElvesServed { get; init; }
    public int ElfGroupsServed { get; init; }
    public int MaxElvesWaiting { get; init; }
    public int MaxConcurrentGroups { get; init; }
    public TimeSpan Elapsed { get; init; }

    public void Print()
    {
        Console.WriteLine("\n========== RELATÓRIO · PAPAI NOEL ==========");
        Console.WriteLine($"Solução: {Solution}");
        Console.WriteLine($"Renas que retornaram ........ {ReindeerReturned}");
        Console.WriteLine($"Renas atendidas ............. {ReindeerServed}");
        Console.WriteLine($"Elfos atendidos ............. {ElvesServed}");
        Console.WriteLine($"Grupos de elfos atendidos ... {ElfGroupsServed}");
        Console.WriteLine($"Máximo de elfos esperando .. {MaxElvesWaiting}");
        Console.WriteLine($"Máximo de grupos simultâneos  {MaxConcurrentGroups}");
        Console.WriteLine($"Tempo total ................. {Elapsed.TotalMilliseconds:F2} ms");
    }
}
