using PapaiNoelN3.Core;
using PapaiNoelN3.Models;
using PapaiNoelN3.Participants;

namespace PapaiNoelN3.Simulations;

public sealed class SolucaoIngenua1
{
    public async Task ExecutarAsync()
    {
        Console.Clear();
        Console.WriteLine("=== PAPAI NOEL | SOLUÇÃO INGÊNUA 1 ===");
        Console.WriteLine("Demonstração de condição de corrida nos contadores compartilhados.\n");

        var estado = new EstadoCompartilhado();
        var tarefas = new List<Task>();

        // O Papai Noel, as 9 renas e os 10 elfos são participantes representados por Tasks.
        tarefas.Add(new PapaiNoel(estado).ExecutarAsync());

        for (int i = 1; i <= SimulationConfig.QuantidadeRenas; i++)
            tarefas.Add(new Rena(i, estado).ExecutarAsync());

        for (int i = 1; i <= SimulationConfig.QuantidadeElfos; i++)
            tarefas.Add(new Elfo(i, estado).ExecutarAsync());

        await Task.WhenAll(tarefas);

        Console.WriteLine("\n=== RESULTADO DA INGÊNUA 1 ===");
        Console.WriteLine($"Renas que deveriam estar registradas : {SimulationConfig.QuantidadeRenas}");
        Console.WriteLine($"Renas registradas no contador        : {estado.RenasRetornadas}");
        Console.WriteLine($"Elfos que deveriam estar registrados : {SimulationConfig.QuantidadeElfos}");
        Console.WriteLine($"Elfos registrados no contador        : {estado.ElfosAguardando}");

        bool corridaRenas = estado.RenasRetornadas != SimulationConfig.QuantidadeRenas;
        bool corridaElfos = estado.ElfosAguardando != SimulationConfig.QuantidadeElfos;

        if (corridaRenas || corridaElfos)
            Logger.Info("DIAGNÓSTICO", "[ERRO] Condição de corrida: uma ou mais atualizações foram perdidas.");
        else
            Logger.Info("DIAGNÓSTICO", "[ATENÇÃO] Esta execução não exibiu perda nos contadores. Execute novamente.");
    }
}
