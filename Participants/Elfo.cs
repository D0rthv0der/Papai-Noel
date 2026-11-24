using PapaiNoelN3.Core;
using PapaiNoelN3.Models;

namespace PapaiNoelN3.Participants;

public sealed class Elfo
{
    private readonly int _id;
    private readonly EstadoCompartilhado _estado;

    public Elfo(int id, EstadoCompartilhado estado)
    {
        _id = id;
        _estado = estado;
    }

    public Task ExecutarAsync()
    {
        return Task.Factory.StartNew(() =>
        {
            Thread.Sleep(Random.Shared.Next(SimulationConfig.AtrasoMinimoMs, SimulationConfig.AtrasoMaximoMs));
            Logger.Info($"Elfo {_id}", "Chegou à oficina e pediu ajuda.");

            // ERRO INTENCIONAL: vários elfos podem ler o mesmo valor antes de gravar.
            int valorLido = _estado.ElfosAguardando;
            Logger.Info($"Elfo {_id}", $"Leu contador de elfos = {valorLido}.");
            Thread.Sleep(SimulationConfig.JanelaCorridaMs);
            _estado.ElfosAguardando = valorLido + 1;
            Logger.Info($"Elfo {_id}", $"Gravou contador de elfos = {_estado.ElfosAguardando}.");
        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }
}
