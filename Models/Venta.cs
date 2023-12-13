namespace LeamosColombiaProject.Models;

public partial class Venta
{
    public int IdVenta { get; set; }

    public DateTime? Fecha { get; set; }

    public string? TipoVenta { get; set; }

    public int? Total { get; set; }

    public bool? Estado { get; set; }

    public int? IdCliente { get; set; }

    public virtual ICollection<Cartera> Carteras { get; set; } = new List<Cartera>();

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    public virtual Cliente? IdClienteNavigation { get; set; }
}
