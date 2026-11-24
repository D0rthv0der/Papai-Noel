namespace PapaiNoelN3.Core;

/// <summary>
/// Configurações centrais do cenário Papai Noel.
/// </summary>

public static class SimulationConfig
{
    public const int ReindeerCount = 9;
    public const int ElfCount = 10;
    public const int JanelaCorridaMs = 1;
    public const int ElfGroupSize = 3;
    public const int MinDelayMs = 10;
    public const int MaxDelayMs = 80;

    // Nomes alternativos mantidos para compatibilidade com versões anteriores.
    public const int QuantidadeRenas = ReindeerCount;
    public const int QuantidadeElfos = ElfCount;
    public const int TamanhoGrupoElfos = ElfGroupSize;
    public const int AtrasoMinimoMs = MinDelayMs;
    public const int AtrasoMaximoMs = MaxDelayMs;
}
