namespace PapaiNoelN3.Simulations;

public static class PerformanceComparison
{
    public static async Task RunAsync()
    {
        Console.WriteLine("\n=== COMPARAÇÃO DE DESEMPENHO ===");
        Console.WriteLine("A comparação usa o tempo total de uma execução de cada versão. Os atrasos aleatórios tornam os valores variáveis.");
        TimeSpan naive = await NaiveBusyWaitSimulation.RunAsync(showReport: false);
        var ideal = await IdealSantaSimulation.RunAsync(showReport: false);

        Console.WriteLine("\nResultado:");
        Console.WriteLine($"Ingênua 2 (espera ocupada): {naive.TotalMilliseconds:F2} ms");
        Console.WriteLine($"Ideal (semáforos):          {ideal.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine("Observação: tempo total não mede sozinho o consumo de CPU; a diferença conceitual é que a espera ocupada consulta repetidamente os contadores, enquanto os semáforos suspendem as Tasks até existir um sinal.");
    }
}
