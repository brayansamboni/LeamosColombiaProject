using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeamosColombiaProject.Models.ViewModels
{
    public class VentaViewModel
    {
        // Propiedades para la compraa { get; set; }
        public bool Estado { get; set; }
        public DateTime Fecha { get; set; }
        public int IdCliente { get; set; }
        public string TipoVenta { get; set; }
        // Lista de detalles de compra
        public int? AbonoInicial { get; set; }
        public List<DetalleVentaViewModel> Detalles { get; set; }

        // Lista de opciones de productos
        public List<SelectListItem> ProductosSelectList { get; set; }


        // Lista de opciones de clientes
        public List<SelectListItem> ClientesSelectList { get; set; }

    }

    public class DetalleVentaViewModel
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string NombreProducto { get; set; }

    }
}
