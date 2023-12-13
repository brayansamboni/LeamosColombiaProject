using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeamosColombiaProject.Models.ViewModels
{
    public class CompraViewModel
    {
        // Propiedades para la compraa { get; set; }
        public bool Estado { get; set; }
        public DateTime Fecha { get; set; }
        public int IdProveedor { get; set; }

        // Lista de detalles de compra
        public List<DetalleCompraViewModel> Detalles { get; set; }

        // Lista de opciones de productos
        public List<SelectListItem> ProductosSelectList { get; set; }

        // Lista de opciones de clientes
        public List<SelectListItem> ProveedoresSelectList { get; set; }

    }

    public class DetalleCompraViewModel
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
