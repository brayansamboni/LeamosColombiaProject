namespace LeamosColombiaProject.Models;

public partial class Categorias
{
    public int IdCategoria { get; set; }

    public string? Categoria { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
