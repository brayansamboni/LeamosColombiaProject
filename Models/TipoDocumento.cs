namespace LeamosColombiaProject.Models;

public partial class TipoDocumento
{
    public int IdTipoDocumento { get; set; }

    public string? Documento { get; set; }

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
