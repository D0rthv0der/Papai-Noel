namespace PapaiNoelN3.Models;

// Nesta primeira versão, os campos são compartilhados sem lock ou operações atômicas.
public sealed class EstadoCompartilhado
{
    public int RenasRetornadas;
    public int ElfosAguardando;
}
