using PapaiNoelN3.Core;
using PapaiNoelN3.Models;

namespace PapaiNoelN3.Participants;

public sealed class PapaiNoel
{
    private readonly EstadoCompartilhado _estado;

    public PapaiNoel(EstadoCompartilhado estado) => _estado = estado;

    public Task ExecutarAsync()
    {
        return Task.Factory.StartNew(() =>
        {
            Logger.Info("Papai Noel", "Está dormindo e aguardará uma checagem do cenário.");
            Thread.Sleep(Random.Shared.Next(10, 30));
            Logger.Info("Papai Noel", $"Verificou o estado: {_estado.RenasRetornadas}/9 renas e {_estado.ElfosAguardando}/10 elfos registrados.");
            Logger.Info("Papai Noel", "Fim da observação da versão ingênua 1.");
        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }
}
