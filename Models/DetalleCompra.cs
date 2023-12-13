namespace LeamosColombiaProject.Models;

public partial class DetalleCompra
{
    public int IdDetalleCompra { get; set; }

    public int? Cantidad { get; set; }

    public int? Precio { get; set; }

    public int? IdCompra { get; set; }

    public int? IdProducto { get; set; }

    public virtual Compra? IdCompraNavigation { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }
}
