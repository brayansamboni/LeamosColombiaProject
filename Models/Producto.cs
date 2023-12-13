using System.ComponentModel.DataAnnotations.Schema;

namespace LeamosColombiaProject.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string? Titulo { get; set; }

    public string? Autor { get; set; }

    public string? Anio { get; set; }

    public int? NPaginas { get; set; }

    public string? Sinopsis { get; set; }

    [NotMapped] // Esto indica a EF Core que esta propiedad no está mapeada a la base de datos
    public byte[] ImagenExistente { get; set; }

    [NotMapped]
    public IFormFile ImagenArchivo { get; set; }

    public string? ImagenUrl { get; set; }

    public int? Precio { get; set; }

    public bool? Estado { get; set; }

    public int? IdCategoria { get; set; }

    public string? Isbn { get; set; }

    public int? Cantidad { get; set; }

    public int? Editorial { get; set; }

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    public virtual Editorial? EditorialNavigation { get; set; }

    public virtual Categorias? IdCategoriaNavigation { get; set; }

}
