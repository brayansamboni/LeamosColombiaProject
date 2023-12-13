using LeamosColombiaProject.Models;
using LeamosColombiaProject.resources;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeamosColombiaProject.Models.ViewModels;
using System.Net.Mail;
using System.Net;

namespace LeamosColombiaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly LeamosColombiaProjectContext _context;

        public ApiController(LeamosColombiaProjectContext context)
        {
            _context = context;
        }

        // GET: api/Api/Usuarios
        [HttpGet("Usuarios")]
        public IActionResult GetUsuarios()
        {
            var usuarios = _context.Usuarios.ToList();
            return Ok(usuarios);
        }

        // GET: api/Api/Productos
        [HttpGet("Productos")]
        public IActionResult GetProductos()
        {
            var productos = _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.EditorialNavigation) // Incluye la relación con la editorial
                .ToList();

            var productosConCategoria = productos.Select(producto => new
            {
                idProducto = producto.IdProducto,
                titulo = producto.Titulo,
                autor = producto.Autor,
                anio = producto.Anio,
                nPaginas = producto.NPaginas,
                sinopsis = producto.Sinopsis,
                imagen = producto.ImagenUrl,
                precio = producto.Precio,
                estado = producto.Estado,
                categoria = producto.IdCategoriaNavigation.Categoria,
                isbn = producto.Isbn,
                stock = producto.Cantidad,
                editorial = producto.EditorialNavigation.NombreEditorial,
                // Omitir otras propiedades según sea necesario
            });

            return Ok(productosConCategoria);
        }

        // GET: api/Api/Carteras
        [HttpGet("Carteras")]
        public IActionResult GetCarteras()
        {
            var carteras = _context.Carteras
                .Include(c => c.AbonoCarteras)
                .Include(c => c.IdVentaNavigation)  // Incluir la relación con la venta
                    .ThenInclude(v => v.IdClienteNavigation)  // Incluir la relación con el cliente de la venta
                .ToList();

            var carterasConAbonos = carteras.Select(cartera => new
            {
                idcartera = cartera.IdCartera,
                fechainicio = cartera.FechaInicio,
                fechafinal = cartera.FechaFinal,
                saldo = cartera.Saldo,
                monto = cartera.Monto,
                estado = cartera.Estado,
                fechaultimoabono = cartera.fechaUltimoAbono,
                IdVentaNavigation = cartera.IdVentaNavigation != null
                    ? new
                    {
                        idventa = cartera.IdVentaNavigation.IdClienteNavigation.Nombre,  // Obtener el nombre del cliente
                    }
                    : null,
                AbonoCarteras = cartera.AbonoCarteras.Select(abono => new
                {
                    idabonocartera = abono.IdAbonoCartera,
                    cuotas = abono.Cuotas,
                    fecha = abono.Fecha,
                    abono = abono.Abono,
                    idcartera = abono.IdCartera,
                    idcarteranavigation = abono.IdCarteraNavigation != null
                        ? new
                        {
                            idcartera = abono.IdCarteraNavigation.IdCartera,
                        }
                        : null
                }).ToList()
            }).ToList();

            return Ok(carterasConAbonos);
        }


        // POST: api/Api/AgregarAbono
        [HttpPost("AgregarAbono")]
        public async Task<IActionResult> AgregarAbono([FromBody] AbonoCartera abono)
        {
            try
            {
                // Verifica si la información del abono es nula
                if (abono == null)
                {
                    return BadRequest("La información del abono es nula");
                }

                // Busca la cartera asociada al abono
                var cartera = await _context.Carteras.FindAsync(abono.IdCartera);
                if (cartera == null)
                {
                    return NotFound("No se encontró la cartera asociada al abono.");
                }

                // Obtiene la cuota actual para la cartera y establece la nueva cuota para el abono
                var cuotaActual = _context.AbonoCarteras
                    .Where(a => a.IdCartera == abono.IdCartera)
                    .Max(a => (int?)a.Cuotas) ?? 0;

                abono.Cuotas = cuotaActual + 1;

                // Agrega el abono a la base de datos
                _context.AbonoCarteras.Add(abono);
                await _context.SaveChangesAsync();

                // Actualiza el saldo de la cartera después de agregar el abono
                cartera.Saldo -= abono.Abono;
                cartera.Monto += abono.Abono;

                _context.Entry(cartera).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                var venta = await _context.Venta
    .Include(v => v.IdClienteNavigation)
    .FirstOrDefaultAsync(v => v.IdVenta == cartera.IdVenta);

                if (venta != null && venta.IdClienteNavigation != null)
                {
                    var cliente = venta.IdClienteNavigation;
                    var cuerpoCorreo = CrearMensajeCorreo(abono, cartera, cliente);
                    await EnviarCorreo(cliente.Correo, "Confirmación de Abono", cuerpoCorreo);
                }
                // Retorna una respuesta exitosa
                return Ok("Abono agregado exitosamente");
            }
            catch (Exception ex)
            {
                // Maneja excepciones según tus necesidades
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al agregar el abono: {ex.Message}");
            }
        }

        private string CrearMensajeCorreo(AbonoCartera abonoCartera, Cartera cartera, Cliente cliente)
        {
            var saldoActualizado = cartera.Saldo;

            if (cartera.Saldo == 0)
            {
                return $"Estimado {cliente.Nombre},<br><br>" +
                       $"¡Felicidades! Su cartera ha sido completamente pagada. A continuación, se detallan algunos datos:<br>" +
                       $"Fecha límite de la cartera: {cartera.FechaFinal}<br>" +
                       $"Fecha de finalización de la cartera: {abonoCartera.Fecha}<br>" +
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

        // GET: api/Api/Usuarios/5
        [HttpGet("Usuarios/{id}")]
        public IActionResult GetUsuario(int id)
        {
            var usuario = _context.Usuarios.Find(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        // GET: api/Api/Productos/5
        [HttpGet("Productos/{id}")]
        public IActionResult GetProducto(int id)
        {
            var producto = _context.Productos.Find(id);

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }

        [HttpGet("CerrarSesion")]
        [Authorize] // Asegurar que solo usuarios autenticados pueden cerrar sesión
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Sesión cerrada exitosamente");
        }

        [HttpPost]
        [Route("IniciarSesion")]
        public async Task<IActionResult> IniciarSesion([FromBody] Usuario request)
        {
            if (string.IsNullOrEmpty(request.Correo) || string.IsNullOrEmpty(request.Contraseña))
            {
                return BadRequest("Los campos de correo y contraseña son obligatorios.");
            }

            Usuario usuario_encontrado = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Correo.Equals(request.Correo) &&
                                              u.Contraseña.Equals(Utilidades.EncriptarClave(request.Contraseña)));

            if (usuario_encontrado == null || (usuario_encontrado.Estado.HasValue && !usuario_encontrado.Estado.Value))
            {
                return Unauthorized("El usuario o la contraseña son incorrectos.");
            }

            // Resto de la lógica de autenticación

            return Ok("Usuario autenticado exitosamente"); // Puedes devolver cualquier otra información relevante
        }
    }
}
