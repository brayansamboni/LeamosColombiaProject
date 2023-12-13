using LeamosColombiaProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace LeamosColombiaProject.Controllers
{
    [Authorize]
    [AutorizacionVista("Carteras")]
    public class AbonoCarterasController : Controller
    {
        private readonly LeamosColombiaProjectContext _context;

        public AbonoCarterasController(LeamosColombiaProjectContext context)
        {
            _context = context;
        }

        // GET: AbonoCarteras
        [AutorizacionVista("Carteras")]
        public async Task<IActionResult> Index()
        {
            var leamosColombiaProjectContext = _context.AbonoCarteras
                .Include(a => a.IdCarteraNavigation)
                .ThenInclude(c => c.IdVentaNavigation)
                .ThenInclude(v => v.IdClienteNavigation);

            return View(await leamosColombiaProjectContext.ToListAsync());
        }


        // GET: AbonoCarteras/Create
        public IActionResult Create()
        {
            var carterasConSaldoYActivas = _context.Carteras
                .Include(c => c.IdVentaNavigation)
                .ThenInclude(v => v.IdClienteNavigation)
                .Where(c => c.Saldo > 0 && c.Estado == true)
                .ToList();

            var viewModelList = carterasConSaldoYActivas.Select(c => new
            {
                IdCartera = c.IdCartera,
                DisplayText = $"{c.IdVentaNavigation.IdClienteNavigation.Nombre} - ({c.IdVentaNavigation.IdClienteNavigation.Documento}) - Cartera: {c.IdCartera}"
            }).ToList();

            ViewData["IdCartera"] = new SelectList(viewModelList, "IdCartera", "DisplayText");
            return View();
        }


        // POST: AbonoCarteras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAbonoCartera,Fecha,Abono,IdCartera")] AbonoCartera abonoCartera)
        {
            if (ModelState.IsValid)
            {
                var cartera = await _context.Carteras
                    .Include(c => c.IdVentaNavigation)
                        .ThenInclude(v => v.IdClienteNavigation)
                    .FirstOrDefaultAsync(c => c.IdCartera == abonoCartera.IdCartera);

                if (cartera != null)
                {
                    var siguienteCuota = _context.AbonoCarteras
                        .Where(a => a.IdCartera == cartera.IdCartera)
                        .Max(a => (int?)a.Cuotas) + 1;

                    abonoCartera.Cuotas = siguienteCuota ?? 1;

                    cartera.Saldo -= abonoCartera.Abono;
                    cartera.Monto += abonoCartera.Abono;

                    _context.Entry(cartera).State = EntityState.Modified;
                }

                abonoCartera.Fecha = DateTime.Now;

                _context.Add(abonoCartera);
                await _context.SaveChangesAsync();

                TempData["ToastrMessage"] = "¡El abono se ha creado correctamente!";
                TempData["ToastrType"] = "success";

                var cliente = cartera.IdVentaNavigation?.IdClienteNavigation;

                await EnviarCorreo(cliente?.Correo, "Actualización de Cartera", await ConstruirCuerpoCorreoAsync(abonoCartera, cartera));

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdCartera"] = new SelectList(_context.Carteras, "IdCartera", "IdCartera", abonoCartera.IdCartera);
            return View(abonoCartera);
        }

        [HttpGet]
        public async Task<IActionResult> ValidarSaldo(int idCartera, decimal abono)
        {
            var cartera = await _context.Carteras
                .FirstOrDefaultAsync(c => c.IdCartera == idCartera);

            if (cartera == null)
            {
                return Json("No se encontró la cartera correspondiente al identificador proporcionado.");
            }

            if (abono > cartera.Saldo)
            {
                return Json("El abono excede el saldo disponible en la cartera.");
            }

            return Json(true);
        }

        private async Task<string> ConstruirCuerpoCorreoAsync(AbonoCartera abonoCartera, Cartera cartera)
        {
            var clienteId = cartera.IdVentaNavigation?.IdCliente;
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.IdCliente == clienteId);

            var saldoActualizado = cartera.Saldo;

            if (cartera.Saldo == 0)
            {
                return $"Estimado {cliente.Nombre},<br><br>" +
                       $"¡Felicidades! Su cartera ha sido completamente pagada. A continuación, se detallan algunos datos:<br>" +
                       $"Fecha límite de la cartera: {cartera.FechaFinal}<br>" +
                       $"Fecha del finalización de la cartera: {abonoCartera.Fecha}<br>" +
                       $"Saldo actualizado: ${saldoActualizado}<br><br>" +
                       $"¡Gracias por completar el pago de su cartera!<br><br>" +
                       "Saludos,<br>" +
                       "Leamos Colombia Project";
            }

            return $"Estimado {cliente.Nombre},<br><br>" +
                   $"Hemos registrado un abono de ${abonoCartera.Abono} en su cartera. A continuación, se detallan algunos datos:<br>" +
                   $"Fecha límite de la cartera: {cartera.FechaFinal}<br>" +
                   $"Fecha del abono: {abonoCartera.Fecha}<br>" +
                   $"Saldo actualizado: ${saldoActualizado}<br><br>" +
                   $"Gracias por su pago.<br><br>" +
                   "Saludos,<br>" +
                   "Leamos Colombia Project";
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


        private async Task<string> ConstruirCuerpoCorreoEliminacionAsync(AbonoCartera abonoCartera, Cartera cartera)
        {
            var clienteId = cartera.IdVentaNavigation?.IdCliente;
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.IdCliente == clienteId);

            if (cliente != null)
            {
                return $"Estimado {cliente.Nombre},<br><br>" +
                       $"Le informamos que el abono de ${abonoCartera.Abono} en su cartera ha sido eliminado. A continuación, se detallan algunos datos:<br>" +
           $"Fecha del abono eliminado: {abonoCartera.Fecha}<br>" +
                       $"Fecha límite de la cartera: {cartera.FechaFinal}<br>" +
                       $"Fecha de eliminación del abono: {DateTime.Now}<br><br>" +
                       $"Gracias por su comprensión.<br><br>" +
                       "Saludos,<br>" +
                       "Leamos Colombia Project";
            }
            else
            {
                return $"Estimado Cliente,<br><br>" +
                       $"Le informamos que el abono de ${abonoCartera.Abono} en su cartera ha sido eliminado. A continuación, se detallan algunos datos:<br>" +
           $"Fecha del abono eliminado: {abonoCartera.Fecha}<br>" +
                       $"Fecha límite de la cartera: {cartera.FechaFinal}<br>" +
                       $"Fecha de eliminación del abono: {DateTime.Now}<br><br>" +
                       $"Gracias por su comprensión.<br><br>" +
                       "Saludos,<br>" +
                       "Leamos Colombia Project";
            }
        }

        [HttpGet]
        public IActionResult GetCarteraSaldo(int idCartera)
        {
            var saldo = _context.Carteras
                .Where(c => c.IdCartera == idCartera)
                .Select(c => c.Saldo)
                .FirstOrDefault();

            return Json(saldo);
        }

        // POST: AbonoCarteras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var abono = await _context.AbonoCarteras.FindAsync(id);
            if (abono != null)
            {
                var cartera = await _context.Carteras.FindAsync(abono.IdCartera);

                cartera.Saldo += abono.Abono;
                cartera.Monto -= abono.Abono;

                _context.Update(cartera);
                _context.Remove(abono);

                await _context.SaveChangesAsync();

                TempData["ToastrMessage"] = "¡El abono se ha eliminado correctamente!";
                TempData["ToastrType"] = "success";

                var cliente = await _context.Clientes
                    .Include(c => c.Venta)
                    .ThenInclude(v => v.Carteras)
                    .FirstOrDefaultAsync(c => c.Venta.Any(v => v.Carteras.Any(ca => ca.IdCartera == cartera.IdCartera)));

                if (cliente != null && !string.IsNullOrEmpty(cliente.Correo))
                {
                    await EnviarCorreo(cliente.Correo, "Eliminación de abono", await ConstruirCuerpoCorreoEliminacionAsync(abono, cartera));
                }
                else
                {
                    Console.WriteLine("La información del cliente o la dirección de correo es nula o vacía.");
                }
            }
            else
            {
                TempData["ToastrMessage"] = "¡No se ha encontrado el abono!";
                TempData["ToastrType"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }


        private bool AbonoCarteraExists(int id)
        {
            return (_context.AbonoCarteras?.Any(e => e.IdAbonoCartera == id)).GetValueOrDefault();
        }
    }
}
