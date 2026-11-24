using PapaiNoelN3.Core;
using PapaiNoelN3.Models;

namespace PapaiNoelN3.Participants;

public sealed class Rena
{
    private readonly int _id;
    private readonly EstadoCompartilhado _estado;

    public Rena(int id, EstadoCompartilhado estado)
    {
        _id = id;
        _estado = estado;
    }

    public Task ExecutarAsync()
    {
        // LongRunning cria uma Task com thread dedicada, sem usar barreiras ou semáforos.
        return Task.Factory.StartNew(() =>
        {
            Thread.Sleep(Random.Shared.Next(SimulationConfig.AtrasoMinimoMs, SimulationConfig.AtrasoMaximoMs));
            Logger.Info($"Rena {_id}", "Retornou da viagem.");

            // ERRO INTENCIONAL: leitura e gravação não formam uma operação indivisível.
            int valorLido = _estado.RenasRetornadas;
            Logger.Info($"Rena {_id}", $"Leu contador de renas = {valorLido}.");
            Thread.Sleep(SimulationConfig.JanelaCorridaMs);
            _estado.RenasRetornadas = valorLido + 1;
            Logger.Info($"Rena {_id}", $"Gravou contador de renas = {_estado.RenasRetornadas}.");
        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }
}
