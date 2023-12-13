namespace LeamosColombiaProject.Models;

public partial class Referencia
{
    public int IdReferencia { get; set; }

    public string? Documento { get; set; }

    public string? Nombre { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public string? Ciudad { get; set; }

    public bool? Estado { get; set; }

    public int? IdCliente { get; set; }

    public virtual Cliente? IdClienteNavigation { get; set; }
}
