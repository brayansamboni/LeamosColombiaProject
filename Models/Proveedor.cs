namespace LeamosColombiaProject.Models;

public partial class Proveedor
{
    public int IdProveedor { get; set; }

    public string? Nombre { get; set; }

    public string? Encargado { get; set; }

    public string? Identificacion { get; set; }

    public string? numeroIdentificacion { get; set; }
    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}