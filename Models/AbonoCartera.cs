namespace LeamosColombiaProject.Models;

public partial class AbonoCartera
{
    public int IdAbonoCartera { get; set; }

    public int? Cuotas { get; set; }

    public DateTime? Fecha { get; set; }

    public int? Abono { get; set; }

    public int? IdCartera { get; set; }
    public virtual Cartera? IdCarteraNavigation { get; set; }
}
