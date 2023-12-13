namespace LeamosColombiaProject.Models;

public partial class Compra
{
    public int IdCompra { get; set; }

    public int? Total { get; set; }

    public DateTime? Fecha { get; set; }

    public bool? Estado { get; set; }

    public int? IdProveedor { get; set; }

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual Proveedor? IdProveedorNavigation { get; set; }
}
