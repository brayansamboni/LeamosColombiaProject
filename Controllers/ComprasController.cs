using LeamosColombiaProject.Models;
using LeamosColombiaProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;


namespace LeamosColombiaProject.Controllers
{
    [Authorize]
    [AutorizacionVista("Compras")]
    public class ComprasController : Controller
    {
        private readonly LeamosColombiaProjectContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ComprasController(LeamosColombiaProjectContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // GET: Compras
        [AutorizacionVista("Compras")]
        public IActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            var comprasQuery = _context.Compras.Include(c => c.IdProveedorNavigation).AsQueryable();

            if (startDate != null)
            {
                comprasQuery = comprasQuery.Where(c => c.Fecha >= startDate);
            }

            if (endDate != null)
            {
                // Ajustar la condición para incluir fechas hasta el final del día
                endDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                comprasQuery = comprasQuery.Where(c => c.Fecha <= endDate);
            }

            var compras = comprasQuery.ToList();

            return View(new Tuple<List<Compra>, DateTime?, DateTime?>(compras, startDate, endDate));
        }


        // GET: Compras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.IdProveedorNavigation)
                .Include(c => c.DetalleCompras)
                .ThenInclude(d => d.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdCompra == id);

            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // GET: Compras/Create
        public IActionResult Create()
        {
            var viewModel = new CompraViewModel
            {
                ProductosSelectList = ObtenerProductosSelectListItems(),
                ProveedoresSelectList = ObtenerProveedoresSelectListItems(),
                Detalles = new List<DetalleCompraViewModel>(),
            };

            return View(viewModel);
        }

        // POST: Compras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompraViewModel viewModel)
        {
            if (viewModel.Detalles == null || viewModel.Detalles.Count == 0)
            {
                TempData["ToastrMessage"] = "Debes agregar al menos un detalle de compra.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction(nameof(Create));
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var compra = new Compra
                    {
                        Total = (int?)viewModel.Detalles.Sum(d => d.Cantidad * d.Precio),
                        Fecha = DateTime.Now,
                        Estado = viewModel.Estado,
                        IdProveedor = viewModel.IdProveedor
                    };

                    _context.Add(compra);
                    await _context.SaveChangesAsync();

                    foreach (var detalleViewModel in viewModel.Detalles)
                    {
                        var detalleCompra = new DetalleCompra
                        {
                            IdCompra = compra.IdCompra,
                            IdProducto = detalleViewModel.IdProducto,
                            Cantidad = detalleViewModel.Cantidad,
                            Precio = (int?)detalleViewModel.Precio
                        };

                        _context.Add(detalleCompra);

                        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.IdProducto == detalleViewModel.IdProducto);
                        if (producto != null)
                        {
                            producto.Cantidad += detalleViewModel.Cantidad;
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["ToastrMessage"] = "¡La compra se ha creado correctamente!";
                    TempData["ToastrType"] = "success";

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    TempData["ToastrMessage"] = "Error al crear la compra. Por favor, inténtalo nuevamente.";
                    TempData["ToastrType"] = "danger";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ObtenerDetalleCompra(int idCompra)
        {
            var detallesCompra = _context.DetalleCompras
                .Where(dc => dc.IdCompra == idCompra)
                .ToList();

            return PartialView("_DetallesCompraPartial", detallesCompra);
        }
        private List<SelectListItem> ObtenerProductosSelectListItems()
        {
            var productos = _context.Productos.ToList();
            var productosSelectList = productos.Select(producto => new SelectListItem
            {
                Value = producto.IdProducto.ToString(),
                Text = producto.Titulo
            }).ToList();

            return productosSelectList;
        }

        private List<SelectListItem> ObtenerProveedoresSelectListItems()
        {
            var proveedoresHabilitados = _context.Proveedors.Where(p => p.Estado == true).ToList();

            var proveedoresSelectList = proveedoresHabilitados.Select(proveedor => new SelectListItem
            {
                Value = proveedor.IdProveedor.ToString(),
                Text = proveedor.Nombre
            }).ToList();

            return proveedoresSelectList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Habilitar(int id)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var compra = await _context.Compras
                        .Include(c => c.DetalleCompras)
                        .ThenInclude(dc => dc.IdProductoNavigation)
                        .FirstOrDefaultAsync(c => c.IdCompra == id);

                    if (compra != null)
                    {
                        var proveedor = await _context.Proveedors.FirstOrDefaultAsync(p => p.IdProveedor == compra.IdProveedor);

                        if (proveedor != null)
                        {
                            if (proveedor.Estado == false)
                            {
                                TempData["ToastrMessage"] = "¡No se puede habilitar la compra porque el proveedor está inhabilitado!";
                                TempData["ToastrType"] = "danger";
                            }
                            else
                            {
                                compra.Estado = true;

                                foreach (var detalleCompra in compra.DetalleCompras)
                                {
                                    var producto = detalleCompra.IdProductoNavigation;

                                    if (producto != null)
                                    {
                                        producto.Cantidad += detalleCompra.Cantidad;
                                    }
                                }

                                await _context.SaveChangesAsync();
                                TempData["ToastrMessage"] = "¡La compra se ha habilitado correctamente!";
                                TempData["ToastrType"] = "success";

                                transaction.Commit();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    TempData["ToastrMessage"] = "Error al habilitar la compra. Por favor, inténtalo nuevamente.";
                    TempData["ToastrType"] = "danger";
                }
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inhabilitar(int id)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var compra = await _context.Compras
                        .Include(c => c.DetalleCompras)
                            .ThenInclude(dc => dc.IdProductoNavigation)
                        .FirstOrDefaultAsync(c => c.IdCompra == id);

                    if (compra != null)
                    {
                        compra.Estado = false;

                        foreach (var detalleCompra in compra.DetalleCompras)
                        {
                            var producto = detalleCompra.IdProductoNavigation;

                            if (producto != null)
                            {
                                if (producto.Cantidad >= detalleCompra.Cantidad)
                                {
                                    producto.Cantidad -= detalleCompra.Cantidad;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    TempData["ToastrMessage"] = "Error al inhabilitar la compra. Cantidad insuficiente de productos.";
                                    TempData["ToastrType"] = "danger";
                                    return RedirectToAction(nameof(Index));
                                }
                            }
                            else
                            {
                                transaction.Rollback();
                                TempData["ToastrMessage"] = "Error al inhabilitar la compra. Uno o más productos están inhabilitados.";
                                TempData["ToastrType"] = "danger";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        await _context.SaveChangesAsync();

                        TempData["ToastrMessage"] = "¡La compra se ha inhabilitado correctamente!";
                        TempData["ToastrType"] = "success";

                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    TempData["ToastrMessage"] = "Error al inhabilitar la compra. Por favor, inténtalo nuevamente.";
                    TempData["ToastrType"] = "danger";
                }
            }

            return RedirectToAction(nameof(Index));
        }



        public IActionResult DescargarPDF(int id)
        {
            var compra = _context.Compras
                .Include(c => c.IdProveedorNavigation)
                .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.IdProductoNavigation)
                .FirstOrDefault(c => c.IdCompra == id);

            var data = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(30);

                    page.Header().ShowOnce().Row(row =>
                    {
                        var rutaImagen = Path.Combine(_hostingEnvironment.WebRootPath, "img/logo.png");
                        byte[] imageData = System.IO.File.ReadAllBytes(rutaImagen);

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().AlignCenter().Width(110).Image(imageData);
                            col.Item().AlignCenter().Text("Leamos Colombia").FontSize(20).Bold();
                            col.Item().PaddingVertical(11);
                            col.Item().LineHorizontal(0.5f);

                        });
                    });

                    page.Content().PaddingVertical(20).Column(col1 =>
                    {

                        col1.Item().AlignCenter().Text("Reporte del Detalle de la Compra").FontSize(17).Bold();

                        col1.Item().PaddingVertical(15);

                        col1.Item().Text("Información del Detalle de la Compra").Bold();

                        col1.Item().PaddingVertical(10);

                        col1.Item().Text(txt =>
                        {
                            txt.Span("Total: ").SemiBold().FontSize(10);
                            txt.Span("$" + string.Format("{0:N0}", compra.Total)).FontSize(10);

                        });

                        col1.Item().PaddingVertical(10);

                        col1.Item().Text(txt =>
                        {
                            txt.Span("Fecha: ").SemiBold().FontSize(10);
                            txt.Span(compra.Fecha.ToString()).FontSize(10);
                        });

                        col1.Item().PaddingVertical(10);

                        col1.Item().Text(txt =>
                        {
                            txt.Span("Proveedor: ").SemiBold().FontSize(10);
                            txt.Span(compra.IdProveedorNavigation.Nombre).FontSize(10);
                        });

                        col1.Item().PaddingVertical(10);

                        col1.Item().Text(txt =>
                        {
                            txt.Span("Estado: ").SemiBold().FontSize(10);
                            txt.Span((compra.Estado ?? false) ? "Activo" : "Inactivo").FontSize(10).FontColor((compra.Estado ?? false) ? "#00A000" : "#FF0000");
                        });

                        col1.Item().PaddingVertical(20);

                        col1.Item().LineHorizontal(0.5f);

                        col1.Item().PaddingVertical(20);

                        col1.Item().AlignCenter().Text("Tabla de Detalles de los Productos").FontSize(17).Bold();

                        col1.Item().PaddingVertical(15);

                        col1.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            tabla.Header(header =>
                            {
                                header.Cell().Background("#257272").Padding(2).Text("Producto").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).Text("Precio Unit").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).Text("Cantidad").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).Text("Total").FontColor("#fff");
                            });

                            foreach (var detalle in compra.DetalleCompras)
                            {
                                var cantidad = detalle.Cantidad;
                                var precio = detalle.Precio;
                                var total = cantidad * precio;

                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text(detalle.IdProductoNavigation.Titulo).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text(string.Format("$ {0:N0}", precio)).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text(cantidad.ToString()).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text(string.Format("$ {0:N0}", total)).FontSize(10);
                            }
                        });
                    });

                    page.Footer()
                        .AlignRight()
                        .Text(txt =>
                        {
                            txt.Span("Página ").FontSize(10);
                            txt.CurrentPageNumber().FontSize(10);
                            txt.Span(" de ").FontSize(10);
                            txt.TotalPages().FontSize(10);
                        });
                });
            }).GeneratePdf();

            Stream stream = new MemoryStream(data);
            string fileName = $"Detalle de la compra [{compra.IdCompra}].pdf";
            return File(stream, "application/pdf", fileName);
        }

        [HttpGet]
        public IActionResult VerificarRegistrosEnRango(DateTime startDate, DateTime endDate)
        {
            var registrosEnRango = _context.Compras
                .Where(c => c.Fecha >= startDate && c.Fecha <= endDate)
                .Any();

            return Json(registrosEnRango);
        }
        public IActionResult DescargarPDFPorRango(DateTime startDate, DateTime endDate)
        {

            var comprasEnRango = _context.Compras
                 .Include(c => c.IdProveedorNavigation)
                .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.IdProductoNavigation)

                .Where(c => c.Fecha >= startDate && c.Fecha <= endDate)
                .ToList();


            var data = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(30);

                    page.Header().ShowOnce().Row(row =>
                    {
                        var rutaImagen = Path.Combine(_hostingEnvironment.WebRootPath, "img/logo.png");
                        byte[] imageData = System.IO.File.ReadAllBytes(rutaImagen);

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().AlignCenter().Width(110).Image(imageData);
                            col.Item().AlignCenter().Text("Leamos Colombia").FontSize(20).Bold();
                            col.Item().PaddingVertical(11);
                            col.Item().LineHorizontal(0.5f);
                        });
                    });

                    page.Content().PaddingVertical(20).Column(col1 =>
                    {
                        col1.Item().AlignCenter().Text("Reporte de la Tabla de Compras").FontSize(17).Bold();
                        col1.Item().PaddingVertical(15);

                        col1.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            tabla.Header(header =>
                            {
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("ID").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Fecha").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Precio total").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Proveedor").FontColor("#fff");
                            });

                            foreach (var compras in comprasEnRango)
                            {
                                var ID = compras.IdCompra;
                                var Fecha = compras.Fecha;
                                var PrecioTotal = compras.Total;
                                var Proveedor = compras.IdProveedorNavigation.Nombre;



                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(ID.ToString()).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(Fecha.ToString()).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(string.Format("$ {0:N0}", PrecioTotal)).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(Proveedor).FontSize(10);

                            }
                        });

                    });

                    page.Footer()
                        .AlignRight()
                        .Text(txt =>
                        {
                            txt.Span("Página ").FontSize(10);
                            txt.CurrentPageNumber().FontSize(10);
                            txt.Span(" de ").FontSize(10);
                            txt.TotalPages().FontSize(10);
                        });
                });
            }).GeneratePdf();

            Stream stream = new MemoryStream(data);
            string fileName = $"Compras_{startDate:yyyy-MM-dd}_A_{endDate:yyyy-MM-dd}.pdf";
            return File(stream, "application/pdf", fileName);
        }

        private bool CompraExists(int id)
        {
            return (_context.Compras?.Any(e => e.IdCompra == id)).GetValueOrDefault();
        }
    }
}
