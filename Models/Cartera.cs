namespace LeamosColombiaProject.Models;

public partial class Cartera
{
    public int IdCartera { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFinal { get; set; }

    public int? Saldo { get; set; }

    public int? Monto { get; set; }

    public bool? Estado { get; set; }

    public DateTime? fechaUltimoAbono { get; set; }

    public int? IdVenta { get; set; }

    public virtual ICollection<AbonoCartera> AbonoCarteras { get; set; } = new List<AbonoCartera>();

    public virtual Venta? IdVentaNavigation { get; set; }
}
