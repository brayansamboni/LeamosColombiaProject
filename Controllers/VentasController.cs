using LeamosColombiaProject.Models;
using LeamosColombiaProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Net;
using System.Net.Mail;

namespace LeamosColombiaProject.Controllers
{
    [Authorize]
    [AutorizacionVista("Ventas")]
    public class VentasController : Controller
    {
        private readonly LeamosColombiaProjectContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public VentasController(LeamosColombiaProjectContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        [AutorizacionVista("Ventas")]
        // GET: Ventas
        public IActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            var ventasQuery = _context.Venta
                .Include(v => v.IdClienteNavigation).AsQueryable();

            if (startDate != null)
            {
                ventasQuery = ventasQuery.Where(v => v.Fecha >= startDate);
            }

            if (endDate != null)
            {
                endDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                ventasQuery = ventasQuery.Where(v => v.Fecha <= endDate);
            }

            var ventas = ventasQuery.ToList();

            return View(new Tuple<List<Venta>, DateTime?, DateTime?>(ventas, startDate, endDate));
        }



        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Venta
                .Include(c => c.IdClienteNavigation)
                .Include(c => c.DetalleVenta)
                .ThenInclude(d => d.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdVenta == id);

            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // GET: Ventas/Create
        public IActionResult Create()
        {

            var viewModel = new VentaViewModel
            {
                ProductosSelectList = ObtenerProductosSelectListItems(),
                ClientesSelectList = ObtenerClientesSelectListItems(),
                Detalles = new List<DetalleVentaViewModel>(),
            };



            return View(viewModel);
        }

        // POST: Ventas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VentaViewModel viewModel)
        {
            if (viewModel.Detalles == null || viewModel.Detalles.Count == 0)
            {
                TempData["ToastrMessage"] = "Debes agregar al menos un detalle de compra.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction(nameof(Create));
            }


            var productosConInsuficienteStock = new List<string>();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var venta = new Venta
                    {
                        Total = (int?)viewModel.Detalles.Sum(d => d.Cantidad * d.Precio),
                        Fecha = DateTime.Now,
                        Estado = viewModel.Estado,
                        IdCliente = viewModel.IdCliente,
                        TipoVenta = viewModel.TipoVenta
                    };

                    _context.Add(venta);
                    await _context.SaveChangesAsync();

                    var productosConInsuficienteStockMessage = "Los siguientes productos no tienen suficiente stock:\n";
                    bool hayProductosSinStock = false;

                    foreach (var detalleViewModel in viewModel.Detalles)
                    {
                        var detalleCompra = new DetalleVenta
                        {
                            IdVenta = venta.IdVenta,
                            IdProducto = detalleViewModel.IdProducto,
                            Cantidad = detalleViewModel.Cantidad,
                            Precio = (int?)detalleViewModel.Precio
                        };

                        _context.Add(detalleCompra);

                        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.IdProducto == detalleViewModel.IdProducto);
                        if (producto != null)
                        {
                            if (detalleViewModel.Cantidad > producto.Cantidad)
                            {
                                // Agrega el nombre del producto con insuficiente stock al mensaje
                                productosConInsuficienteStock.Add($"{producto.Titulo} (disponibles: {producto.Cantidad})");
                                hayProductosSinStock = true;
                            }
                            else
                            {
                                producto.Cantidad -= detalleViewModel.Cantidad;
                            }
                        }

                    }

                    if (viewModel.TipoVenta == "Crédito")
                    {
                        var abonoInicial = viewModel.AbonoInicial ?? 0; // Si es nulo, asigna 0

                        var cartera = new Cartera
                        {
                            FechaInicio = DateTime.Now,
                            FechaFinal = DateTime.Now.AddMonths(6),
                            Saldo = venta.Total,
                            Monto = abonoInicial,
                            Estado = true,
                            IdVenta = venta.IdVenta
                        };

                        _context.Add(cartera);

                        if (abonoInicial > 0)
                        {
                            var abonoCartera = new AbonoCartera
                            {
                                Cuotas = 1,
                                Fecha = DateTime.Now,
                                Abono = abonoInicial,
                                IdCarteraNavigation = cartera
                            };

                            cartera.Saldo = venta.Total - abonoInicial;
                            cartera.Monto = abonoInicial;
                            _context.AbonoCarteras.Add(abonoCartera);
                        }

                        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.IdCliente == viewModel.IdCliente);
                        await EnviarCorreo(cliente?.Correo, "Creación de cartera", await ConstruirCuerpoCorreoCarteraAsync(venta, viewModel));
                    }



                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    TempData["ToastrMessage"] = "¡La venta se ha creado correctamente!";
                    TempData["ToastrType"] = "success";
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    TempData["ToastrMessage"] = "Error al crear la venta. Por favor, inténtalo nuevamente.";
                    TempData["ToastrType"] = "danger";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ValidarStock(VentaViewModel viewModel)
        {
            var productosConInsuficienteStock = new List<string>();

            foreach (var detalleViewModel in viewModel.Detalles)
            {
                var producto = await _context.Productos.FirstOrDefaultAsync(p => p.IdProducto == detalleViewModel.IdProducto);

                if (producto != null && detalleViewModel.Cantidad > producto.Cantidad)
                {
                    productosConInsuficienteStock.Add($"{producto.Titulo} (disponibles: {producto.Cantidad})");
                }
            }

            if (productosConInsuficienteStock.Any())
            {
                var mensaje = "Los siguientes productos no tienen suficiente stock:\n" + string.Join(", ", productosConInsuficienteStock);
                return Json(mensaje);
            }

            return Json(true);
        }

        [HttpGet]
        public async Task<IActionResult> ValidarCliente(VentaViewModel viewModel)
        {
            Console.WriteLine($"Tipo de Venta: {viewModel.TipoVenta}");
            Console.WriteLine($"Cliente: {viewModel.IdCliente}");

            // Validar cartera activa
            var carteraPendiente = await _context.Carteras
                .FirstOrDefaultAsync(c => c.Estado == true && c.IdVentaNavigation.IdCliente == viewModel.IdCliente && c.Saldo > 0);

            Console.WriteLine($"Cartera: {carteraPendiente}");

            if (carteraPendiente != null && viewModel.TipoVenta == "Crédito")
            {
                return Json("El cliente tiene una cartera activa con saldo pendiente.");
            }

            // Validar referencia activa
            var referenciaActiva = await _context.Referencia
                .FirstOrDefaultAsync(r => r.IdCliente == viewModel.IdCliente && r.Estado == true);

            Console.WriteLine($"Referencia: {referenciaActiva}");

            if (referenciaActiva == null && viewModel.TipoVenta == "Crédito")
            {
                Console.WriteLine("El cliente no tiene una referencia activa.");
                return Json("El cliente no tiene una referencia activa.");
            }

            return Json(true);
        }

        private async Task EnviarCorreo(string correoDestino, string asunto, string cuerpo)
        {
            try
            {
                if (correoDestino != null)
                {
                    string smtpServer = "smtp.gmail.com";
                    int smtpPort = 587;
                    string smtpUsername = "leamoscolombiarecovery@gmail.com";
                    string smtpPassword = "xxjx autl cvbl pfip";

                    var smtpClient = new SmtpClient(smtpServer)
                    {
                        Port = smtpPort,
                        Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                        EnableSsl = true,
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpUsername),
                        Subject = asunto,
                        Body = cuerpo,
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(correoDestino);

                    await smtpClient.SendMailAsync(mailMessage);
                }
                else
                {
                    Console.WriteLine("La dirección de destino del correo es nula.");
                }
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("Error al enviar el correo: " + ex.Message);
            }
        }


        private List<SelectListItem> ObtenerProductosSelectListItems()
        {
            var productosHabilitados = _context.Productos.Where(p => p.Estado == true).ToList();

            var productosSelectList = productosHabilitados.Select(producto => new SelectListItem
            {
                Value = producto.IdProducto.ToString(),
                Text = producto.Titulo
            }).ToList();

            return productosSelectList;
        }


        private List<SelectListItem> ObtenerClientesSelectListItems()
        {
            var clientes = _context.Clientes.Where(p => p.Estado == true).ToList();

            var clientesHabilitados = clientes.Select(cliente => new SelectListItem
            {
                Value = cliente.IdCliente.ToString(),
                Text = $"{cliente.Nombre} - ({cliente.Documento})"
            }).ToList();

            return clientesHabilitados;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Habilitar(int id)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var venta = await _context.Venta
                        .Include(c => c.DetalleVenta)
                        .ThenInclude(dc => dc.IdProductoNavigation)
                        .FirstOrDefaultAsync(c => c.IdVenta == id);

                    if (venta != null)
                    {
                        var cliente = await _context.Clientes.FirstOrDefaultAsync(p => p.IdCliente == venta.IdCliente);

                        if (cliente != null && cliente.Estado.HasValue && cliente.Estado.Value)
                        {
                            venta.Estado = true;

                            foreach (var detalleVenta in venta.DetalleVenta)
                            {
                                var producto = detalleVenta.IdProductoNavigation;

                                if (producto != null && producto.Cantidad >= detalleVenta.Cantidad)
                                {
                                    producto.Cantidad -= detalleVenta.Cantidad;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    TempData["ToastrMessage"] = "Error al habilitar la venta. Cantidad insuficiente de productos.";
                                    TempData["ToastrType"] = "danger";
                                    return RedirectToAction(nameof(Index));
                                }
                            }

                            await _context.SaveChangesAsync();
                            TempData["ToastrMessage"] = "¡La venta se ha habilitado correctamente!";
                            TempData["ToastrType"] = "success";

                            transaction.Commit();
                        }
                        else
                        {
                            TempData["ToastrMessage"] = "¡No se puede habilitar la venta porque el cliente está inhabilitado!";
                            TempData["ToastrType"] = "danger";
                        }
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    TempData["ToastrMessage"] = "Error al habilitar la venta. Por favor, inténtalo nuevamente.";
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
                    var venta = await _context.Venta
                        .Include(c => c.DetalleVenta)
                        .ThenInclude(dc => dc.IdProductoNavigation)
                        .FirstOrDefaultAsync(c => c.IdVenta == id);

                    if (venta != null)
                    {
                        venta.Estado = false;

                        foreach (var detalleCompra in venta.DetalleVenta)
                        {
                            var producto = detalleCompra.IdProductoNavigation;

                            if (producto != null)
                            {
                                producto.Cantidad += detalleCompra.Cantidad;
                            }
                        }

                        var cartera = await _context.Carteras.FirstOrDefaultAsync(c => c.IdVenta == id);
                        if (cartera != null)
                        {
                            cartera.Estado = false;
                        }

                        await _context.SaveChangesAsync();

                        TempData["ToastrMessage"] = "¡La venta se ha inhabilitado correctamente!";
                        TempData["ToastrType"] = "success";

                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    TempData["ToastrMessage"] = "Error al inhabilitar la venta y la cartera asociada. Por favor, inténtalo nuevamente.";
                    TempData["ToastrType"] = "danger";
                }
            }

            return RedirectToAction(nameof(Index));
        }


        public IActionResult DescargarPDF(int id)
        {
            var venta = _context.Venta
                .Include(c => c.IdClienteNavigation)
                .Include(c => c.DetalleVenta)
                .ThenInclude(d => d.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdVenta == id).Result;

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

                        col1.Item().AlignCenter().Text("Reporte del Detalle de Venta").FontSize(17).Bold();

                        col1.Item().PaddingVertical(15);

                        col1.Item().Text("Información del Detalle de Venta").Bold();

                        col1.Item().PaddingVertical(10);

                        col1.Item().Text(txt =>
                        {
                            txt.Span("Total: ").SemiBold().FontSize(10);
                            txt.Span("$" + string.Format("{0:N0}", venta.Total)).FontSize(10);
                        });

                        col1.Item().PaddingVertical(10);

                        col1.Item().Text(txt =>
                        {
                            txt.Span("Fecha: ").SemiBold().FontSize(10);
                            txt.Span(venta.Fecha.ToString()).FontSize(10);
                        });

                        col1.Item().PaddingVertical(10);

                        col1.Item().Text(txt =>
                        {
                            txt.Span("Cliente: ").SemiBold().FontSize(10);
                            txt.Span(venta.IdClienteNavigation.Nombre).FontSize(10);
                        });

                        col1.Item().PaddingVertical(10);

                        col1.Item().Text(txt =>
                        {
                            txt.Span("Estado: ").SemiBold().FontSize(10);
                            txt.Span((venta.Estado ?? false) ? "Activo" : "Inactivo").FontSize(10).FontColor((venta.Estado ?? false) ? "#00A000" : "#FF0000");
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

                            foreach (var detalle in venta.DetalleVenta)
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
            string fileName = $"Detalle de venta [{venta.IdVenta}].pdf";
            return File(stream, "application/pdf", fileName);
        }

        [HttpGet]
        public IActionResult VerificarRegistrosEnRango(DateTime startDate, DateTime endDate)
        {
            var registrosEnRango = _context.Venta
                .Where(c => c.Fecha >= startDate && c.Fecha <= endDate)
                .Any();

            return Json(registrosEnRango);
        }
        public IActionResult DescargarPDFPorRango(DateTime startDate, DateTime endDate)
        {

            var ventaEnRango = _context.Venta
                .Include(c => c.IdClienteNavigation)
                .Include(c => c.DetalleVenta)
                .ThenInclude(d => d.IdProductoNavigation)
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
                        col1.Item().AlignCenter().Text("Reporte de la Tabla de Ventas").FontSize(17).Bold();
                        col1.Item().PaddingVertical(15);

                        col1.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);

                            });

                            tabla.Header(header =>
                            {
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("ID").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Tipo de venta").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Fecha").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Precio total").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Cliente").FontColor("#fff");
                            });

                            foreach (var ventas in ventaEnRango)
                            {
                                var ID = ventas.IdVenta;
                                var TipoVenta = ventas.TipoVenta;
                                var Fecha = ventas.Fecha;
                                var PrecioTotal = ventas.Total;
                                var Cliente = ventas.IdClienteNavigation.Nombre;



                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(ID.ToString()).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(TipoVenta).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(Fecha.ToString()).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(string.Format("$ {0:N0}", PrecioTotal)).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(Cliente).FontSize(10);
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
            string fileName = $"Ventas_{startDate:yyyy-MM-dd}_A_{endDate:yyyy-MM-dd}.pdf";
            return File(stream, "application/pdf", fileName);
        }

        private async Task<string> ConstruirCuerpoCorreoCarteraAsync(Venta venta, VentaViewModel viewModel)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.IdCliente == viewModel.IdCliente);

            return $"Estimado {cliente.Nombre},<br><br>" +
                   $"Hemos creado una cartera para su compra. A continuación, se detallan algunos datos:<br>" +
                   $"Número de venta: {venta.IdVenta}<br>" +
                   $"Fecha de la venta: {venta.Fecha}<br>" +
                   $"Tipo de venta: {viewModel.TipoVenta}<br><br>" +
                   $"Gracias por elegir nuestros servicios.<br><br>" +
                   "Saludos,<br>" +
                   "Leamos Colombia Project";
        }
        private bool VentumExists(int id)
        {
            return (_context.Venta?.Any(e => e.IdVenta == id)).GetValueOrDefault();
        }
    }
}
