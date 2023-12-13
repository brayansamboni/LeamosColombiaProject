using LeamosColombiaProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace LeamosColombiaProject.Controllers
{
    [Authorize]
    [AutorizacionVista("Carteras")]
    public class CarterasController : Controller
    {
        private readonly LeamosColombiaProjectContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CarterasController(LeamosColombiaProjectContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // GET: Carteras
        [AutorizacionVista("Carteras")]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var carterasQuery = _context.Carteras
                .Include(c => c.IdVentaNavigation)
                                    .Include(c => c.IdVentaNavigation.IdClienteNavigation)

                .Include(c => c.AbonoCarteras)
                .OrderByDescending(c => c.AbonoCarteras.Max(a => (DateTime?)a.Fecha))
                .AsQueryable();

            if (startDate != null)
            {
                carterasQuery = carterasQuery.Where(c => c.FechaInicio >= startDate);
            }

            if (endDate != null)
            {
                endDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                carterasQuery = carterasQuery.Where(c => c.FechaInicio <= endDate);
            }

            foreach (var cartera in carterasQuery)
            {
                cartera.fechaUltimoAbono = cartera.AbonoCarteras.Max(a => a.Fecha);
            }

            var carteras = await carterasQuery.ToListAsync();

            return View(new Tuple<List<Cartera>, DateTime?, DateTime?>(carteras, startDate, endDate));
        }


        // GET: Carteras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartera = await _context.Carteras
                .Include(c => c.IdVentaNavigation)
                                    .Include(c => c.IdVentaNavigation.IdClienteNavigation)

                .Include(c => c.AbonoCarteras)
                .FirstOrDefaultAsync(m => m.IdCartera == id);

            if (cartera == null)
            {
                return NotFound();
            }
            var fechaUltimoAbono = cartera.AbonoCarteras.Max(a => a.Fecha);

            cartera.fechaUltimoAbono = fechaUltimoAbono;

            return View(cartera);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Habilitar(int id)
        {
            var cartera = await _context.Carteras
                .Include(c => c.IdVentaNavigation)
                    .ThenInclude(v => v.IdClienteNavigation)
                .FirstOrDefaultAsync(c => c.IdCartera == id);

            if (cartera == null)
            {
                return NotFound();
            }

            if (cartera.IdVentaNavigation != null && cartera.IdVentaNavigation.IdClienteNavigation != null)
            {
                var cliente = cartera.IdVentaNavigation.IdClienteNavigation;

                // Check if the client's state is true
                if (cliente.Estado == true)
                {
                    var clienteId = cliente.IdCliente;

                    // Verificar si hay una referencia activa
                    var referenciaActiva = await _context.Referencia
                        .FirstOrDefaultAsync(r => r.IdCliente == clienteId && r.Estado == true);

                    if (referenciaActiva == null)
                    {
                        TempData["ToastrMessage"] = "No se puede habilitar la cartera porque el cliente no tiene una referencia activa.";
                        TempData["ToastrType"] = "danger";
                        return RedirectToAction(nameof(Index));
                    }

                    // Verificar si hay otra cartera activa con saldo pendiente para el mismo cliente
                    bool hayOtraCarteraActivaConSaldo = _context.Carteras
                        .Any(c => c.Estado == true && c.Saldo > 0 && c.IdCartera != id &&
                                  c.IdVentaNavigation != null &&
                                  c.IdVentaNavigation.IdClienteNavigation != null &&
                                  c.IdVentaNavigation.IdClienteNavigation.IdCliente == clienteId);

                    if (hayOtraCarteraActivaConSaldo)
                    {
                        TempData["ToastrMessage"] = "No se puede habilitar la cartera porque hay otra cartera activa con saldo pendiente para el mismo cliente.";
                        TempData["ToastrType"] = "danger";
                        return RedirectToAction(nameof(Index));
                    }

                    cartera.Estado = true;
                    _context.Update(cartera);
                    await _context.SaveChangesAsync();

                    TempData["ToastrMessage"] = "¡La cartera se ha habilitado correctamente!";
                    TempData["ToastrType"] = "success";

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ToastrMessage"] = "No se puede habilitar la cartera porque el cliente está inhabilitado .";
                    TempData["ToastrType"] = "danger";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                TempData["ToastrMessage"] = "Error al obtener información de la cartera, venta o cliente asociado.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }





        // POST: Carteras/Inhabilitar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inhabilitar(int id)
        {
            var cartera = await _context.Carteras.FindAsync(id);
            if (cartera == null)
            {
                return NotFound();
            }

            cartera.Estado = false;
            _context.Update(cartera);

            await _context.SaveChangesAsync();

            TempData["ToastrMessage"] = "¡La cartera se ha inhabilitado correctamente!";
            TempData["ToastrType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DescargarPDF(int id)
        {
            var cartera = _context.Carteras
                .Include(c => c.IdVentaNavigation)
                                                    .Include(c => c.IdVentaNavigation.IdClienteNavigation)

                .Include(c => c.AbonoCarteras)
                .FirstOrDefaultAsync(m => m.IdCartera == id).Result;

            var fechaUltimoAbono = cartera.AbonoCarteras.Max(a => a.Fecha);
            cartera.fechaUltimoAbono = fechaUltimoAbono;

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

                        col1.Item().AlignCenter().Text("Reporte del Detalle de la Cartera").FontSize(17).Bold();

                        col1.Item().PaddingVertical(15);

                        col1.Item().Text("Información del Detalle de la Cartera").Bold();

                        col1.Item().PaddingVertical(10);
                        col1.Item().Text(txt =>
                        {
                            txt.Span("Cliente: ").SemiBold().FontSize(10);
                            txt.Span(cartera.IdVentaNavigation.IdClienteNavigation.Nombre.ToString()).FontSize(10);

                        });

                        col1.Item().PaddingVertical(10);

                        col1.Item().Text(txt =>
                        {
                            txt.Span("Monto: ").SemiBold().FontSize(10);
                            txt.Span("$" + string.Format("{0:N0}", cartera.Monto)).FontSize(10);

                        });

                        col1.Item().PaddingVertical(10);

                        col1.Item().Text(txt =>
                        {
                            txt.Span("Saldo: ").SemiBold().FontSize(10);
                            txt.Span("$" + string.Format("{0:N0}", cartera.Saldo)).FontSize(10);

                        });

                        col1.Item().PaddingVertical(10);
                        col1.Item().Text(txt =>
                        {
                            txt.Span("Progreso: ").SemiBold().FontSize(10);

                            if (cartera.Saldo == 0)
                            {
                                txt.Span("Completado").SemiBold().FontSize(10).FontColor("#00A000");
                            }
                            else if (cartera.Saldo > 0)
                            {
                                txt.Span("Pendiente").SemiBold().FontSize(10).FontColor("#FFFF00");

                            }
                            else
                            {
                                txt.Span((cartera.Estado ?? false) ? "Cancelado" : "").FontSize(10).FontColor((cartera.Estado ?? false) ? "#FF0000" : "#");
                            }
                        });

                        col1.Item().PaddingVertical(10);

                        col1.Item().Text(txt =>
                        {
                            txt.Span("Fecha Inicial: ").SemiBold().FontSize(10);
                            txt.Span(cartera.FechaInicio.ToString()).FontSize(10);
                        });

                        col1.Item().PaddingVertical(10);

                        col1.Item().Text(txt =>
                        {
                            txt.Span("Fecha del Ultimo Abono: ").SemiBold().FontSize(10);
                            txt.Span(cartera.fechaUltimoAbono.ToString()).FontSize(10);
                        });

                        col1.Item().PaddingVertical(10);

                        col1.Item().Text(txt =>
                        {
                            txt.Span("Fecha Final: ").SemiBold().FontSize(10);
                            txt.Span(cartera.FechaFinal.ToString()).FontSize(10);
                        });

                        col1.Item().PaddingVertical(10);

                        col1.Item().Text(txt =>
                        {
                            txt.Span("Estado: ").SemiBold().FontSize(10);
                            txt.Span((cartera.Estado ?? false) ? "Activo" : "Inactivo").FontSize(10).FontColor((cartera.Estado ?? false) ? "#00A000" : "#FF0000");
                        });

                        col1.Item().PaddingVertical(20);

                        col1.Item().LineHorizontal(0.5f);

                        col1.Item().PaddingVertical(20);

                        col1.Item().AlignCenter().Text("Tabla de Detalles de los Abonos").FontSize(17).Bold();

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
                                header.Cell().Background("#257272").Padding(2).Text("ID").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).Text("Cuota").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).Text("Abono").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).Text("Fecha").FontColor("#fff");
                            });

                            foreach (var detalle in cartera.AbonoCarteras)
                            {
                                var ID = detalle.IdAbonoCartera;
                                var cuota = detalle.Cuotas;
                                var abono = detalle.Abono;
                                var fecha = detalle.Fecha;

                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text(detalle.IdAbonoCartera).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text(detalle.Cuotas.ToString()).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text(string.Format("$ {0:N0}", detalle.Abono)).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text(detalle.Fecha.ToString()).FontSize(10);
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
            string fileName = $"Detalle de la cartera [{cartera.IdCartera}].pdf";
            return File(stream, "application/pdf", fileName);
        }

        [HttpGet]
        public IActionResult VerificarRegistrosEnRango(DateTime startDate, DateTime endDate)
        {
            var registrosEnRango = _context.Carteras
                .Where(c => c.FechaInicio >= startDate && c.FechaInicio <= endDate)
                .Any();

            return Json(registrosEnRango);
        }
        public IActionResult DescargarPDFPorRango(DateTime startDate, DateTime endDate)
        {

            var carterasEnRango = _context.Carteras
                .Include(c => c.IdVentaNavigation)
                .Include(c => c.IdVentaNavigation.IdClienteNavigation)
                    .Include(c => c.AbonoCarteras)

                .Where(c => c.FechaInicio >= startDate && c.FechaInicio <= endDate)
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
                        col1.Item().AlignCenter().Text("Reporte de la Tabla de Carteras").FontSize(17).Bold();
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
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn(1);
                            });

                            tabla.Header(header =>
                            {
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("ID").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Venta").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Cliente").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Fecha Inicio").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Fecha de último pago").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Fecha límite").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Saldo").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Monto").FontColor("#fff");
                                header.Cell().Background("#257272").Padding(2).AlignCenter().Text("Progreso").FontColor("#fff");
                            });

                            foreach (var cartera in carterasEnRango)
                            {
                                var ID = cartera.IdCartera;
                                var venta = cartera.IdVentaNavigation.IdVenta;
                                var cliente = cartera.IdVentaNavigation.IdClienteNavigation != null
                                    ? cartera.IdVentaNavigation.IdClienteNavigation.Nombre
                                    : "Cliente no disponible";
                                var fechaInicio = cartera.FechaInicio;
                                var fechaUltimoAbono = cartera.AbonoCarteras.Any() ? cartera.AbonoCarteras.Max(a => (DateTime?)a.Fecha) : null;

                                var fechaLimite = cartera.FechaFinal;
                                var saldo = cartera.Saldo;
                                var monto = cartera.Monto;
                                var estado = cartera.Estado;

                                var progreso = "Completado"; // Por defecto, asumimos completado



                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(ID.ToString()).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(venta).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(cliente).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(fechaInicio.ToString()).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(fechaUltimoAbono.HasValue ? fechaUltimoAbono.Value.ToString() : "N/A").FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(fechaLimite.ToString()).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(string.Format("$ {0:N0}", saldo)).FontSize(10);
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(string.Format("$ {0:N0}", monto)).FontSize(10);
                                if (saldo > 0 && estado == true)
                                {
                                    progreso = "Pendiente";
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(progreso).FontSize(10).FontColor("#FFFF00"); // Amarillo
                                }
                                else if (saldo < 0 || estado == false)
                                {
                                    progreso = "Cancelado";
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(progreso).FontSize(10).FontColor("#FF0000"); // Rojo
                                }
                                else
                                {
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignCenter().Text(progreso).FontSize(10).FontColor("#00FF00"); // Verde
                                }
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
            string fileName = $"Carteras_{startDate:yyyy-MM-dd}_A_{endDate:yyyy-MM-dd}.pdf";
            return File(stream, "application/pdf", fileName);
        }



        private bool CarteraExists(int id)
        {
            return (_context.Carteras?.Any(e => e.IdCartera == id)).GetValueOrDefault();
        }
    }
}
