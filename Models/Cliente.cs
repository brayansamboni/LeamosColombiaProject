namespace LeamosColombiaProject.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string? Documento { get; set; }

    public string? Nombre { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public string? Ciudad { get; set; }

    public string? Universidad { get; set; }

    public string? Facultad { get; set; }

    public int? Semestre { get; set; }

    public bool? Estado { get; set; }

    public int? IdTipoDocumento { get; set; }

    public virtual TipoDocumento? IdTipoDocumentoNavigation { get; set; }

    public virtual ICollection<Referencia> Referencia { get; set; } = new List<Referencia>();

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
