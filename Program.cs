using PapaiNoelN3.Core;
using PapaiNoelN3.Simulations;

Console.OutputEncoding = System.Text.Encoding.UTF8;

while (true)
{
    Console.WriteLine("\n========================================");
    Console.WriteLine("       PROBLEMA DO PAPAI NOEL");
    Console.WriteLine("========================================");
    Console.WriteLine("1 - Solução ingênua 1: condição de corrida");
    Console.WriteLine("2 - Solução ingênua 2: espera ocupada");
    Console.WriteLine("3 - Solução ideal: SemaphoreSlim + lock");
    Console.WriteLine("4 - Comparar desempenho (ingênua 2 x ideal)");
    Console.WriteLine("0 - Sair");
    Console.Write("Escolha: ");

    string? option = Console.ReadLine();
    if (option == "0") break;

    try
    {
        switch (option)
        {
            case "1":
                await NaiveRaceSimulation.RunAsync();
                break;
            case "2":
                await NaiveBusyWaitSimulation.RunAsync();
                break;
            case "3":
                await IdealSantaSimulation.RunAsync();
                break;
            case "4":
                await PerformanceComparison.RunAsync();
                break;
            default:
                Console.WriteLine("Opção inválida.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Falha na execução: {ex.Message}");
    }

    Console.WriteLine("\nPressione ENTER para voltar ao menu...");
    Console.ReadLine();
}
