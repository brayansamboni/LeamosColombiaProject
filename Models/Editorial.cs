namespace LeamosColombiaProject.Models;

public partial class Editorial
{
    public int IdEditorial { get; set; }

    public string? NombreEditorial { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
