using LeamosColombiaProject.Models;
using LeamosColombiaProject.Models.ViewModels.LeamosColombiaProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;

namespace LeamosColombiaProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly LeamosColombiaProjectContext _context;

        public HomeController(LeamosColombiaProjectContext context)
        {
            _context = context;
        }
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult ResumenVenta()
        {
            DateTime FechaInicio = DateTime.Today.AddDays(-DateTime.Today.Day + 1); // Primer día del mes actual
            DateTime FechaFin = DateTime.Today; // Último día del mes actual

            List<VMVentas> Lista = (from tbventas in _context.Venta
                                    where tbventas.Fecha.Value.Date >= FechaInicio.Date && tbventas.Estado == true
                                    group tbventas by tbventas.Fecha.Value.Date into grupo
                                    orderby grupo.Key ascending // Order by date in ascending order
                                    select new VMVentas
                                    {
                                        Fecha = grupo.Key.ToString("dd/MM/yyyy"),
                                        Cantidad = grupo.Sum(v => 1), // Suma de las cantidades de ventas en el mismo día
                                    })
                                    .Take(10) // Take the first 10 records
                                    .ToList();

            return StatusCode(StatusCodes.Status200OK, Lista);
        }



        public IActionResult ResumenVentaPorMes()
        {
            // Obtener las ventas por mes y sumar el total de cada mes
            var salesByMonth = _context.Venta
                .Where(v => v.Estado == true && v.Fecha != null) // Filtrar ventas activas y asegurarse de que Fecha no sea nulo
                .GroupBy(v => v.Fecha.Value.Month) // Agrupar por mes
                .OrderBy(g => g.Key) // Ordenar por mes
                .Select(g => new { Month = g.Key, TotalSales = g.Sum(v => v.Total) })
                .ToList();

            // Crear una lista de objeto anónimo para almacenar los datos
            var salesData = new List<object>();

            // Llenar la lista con los datos de ventas por mes
            foreach (var item in salesByMonth)
            {
                salesData.Add(new { Mes = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Month), Cantidad = item.TotalSales });
            }

            return StatusCode(StatusCodes.Status200OK, salesData);
        }


        public IActionResult Index()
        {
            var salesByMonth = _context.Venta
                .Where(v => v.Estado == true && v.Fecha != null) 
                .GroupBy(v => v.Fecha.Value.Month) 
                .OrderBy(g => g.Key)
                .Select(g => new { Month = g.Key, TotalSales = g.Sum(v => v.Total) })
                .ToList();

            var months = new string[12];
            var salesData = new double[12];

            for (int i = 0; i < 12; i++)
            {
                months[i] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i + 1);
                salesData[i] = 0.0;
            }

            foreach (var item in salesByMonth)
            {
                salesData[item.Month - 1] = (double)item.TotalSales;
            }

            var clientCount = _context.Clientes.Count(c => c.Estado == true);

            var productCount = _context.Productos.Count(c => c.Estado == true);

            var providerCount = _context.Proveedors.Count(c => c.Estado == true);

            var salesCount = _context.Venta.Count(c => c.Estado == true);

            var totalVentasActivas = _context.Venta
                .Where(v => v.Estado == true)
                .Sum(v => v.Total);


            var totalComprasActivas = _context.Compras
                .Where(v => v.Estado == true)
                .Sum(v => v.Total);


            var topClients = _context.Clientes
                .OrderByDescending(c => c.Venta.Count())
                .Take(5)
                .ToList();

            var topProviders = _context.Proveedors
                .OrderByDescending(l => l.Compras.Count())
                .Take(5)
                .ToList();


            var quantitiesClients = new List<int>();
            foreach (var client in topClients)
            {
                var totalQuantityClients = _context.Venta
                    .Count(v => v.IdCliente == client.IdCliente);
                quantitiesClients.Add(totalQuantityClients);
            }

            var quantitiesProviders = new List<int>();
            foreach (var provider in topProviders)
            {
                var totalQuantityProviders = _context.Productos
                    .Count(v => v.IdProducto == provider.IdProveedor);
                quantitiesProviders.Add(totalQuantityProviders);
            }


            ViewData["ProductCount"] = productCount;
            ViewData["ClientCount"] = clientCount;
            ViewData["SalesCount"] = salesCount;
            ViewData["totalQuantityClients"] = quantitiesClients;
            ViewData["totalQuantityProviders"] = quantitiesProviders;
            ViewData["topClients"] = topClients.Select(c => c.Documento).ToList();
            ViewData["topProviders"] = topProviders.Select(l => l.IdProveedor).ToList();
            ViewData["TotalVentasActivas"] = totalVentasActivas;
            ViewData["TotalComprasActivas"] = totalComprasActivas;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
