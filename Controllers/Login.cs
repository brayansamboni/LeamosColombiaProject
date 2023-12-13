using LeamosColombiaProject.Models;
using LeamosColombiaProject.resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace admin.Controllers
{
    public class InicioSesionController : Controller
    {
        private readonly LeamosColombiaProjectContext _context;

        public InicioSesionController(LeamosColombiaProjectContext context)
        {
            _context = context;
        }

        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string correo, string contraseña)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contraseña))
            {
                TempData["ToastrMessage"] = "Los campos de correo y contraseña son obligatorios.";
                TempData["ToastrType"] = "danger";
                return View();
            }
            Usuario usuario_encontrado = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Correo.Equals(correo) &&
                                              u.Contraseña.Equals(Utilidades.EncriptarClave(contraseña)));

            if (usuario_encontrado == null || (usuario_encontrado.Estado.HasValue && !usuario_encontrado.Estado.Value))
            {
                TempData["ToastrMessage"] = "El usuario o la contraseña son incorrectos.";
                TempData["ToastrType"] = "danger";
                return View();
            }

            var rol = await _context.Rols
                .Where(r => r.IdRol == usuario_encontrado.IdRol)
                .Select(r => r.Nombre)
                .FirstOrDefaultAsync();

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.IdUsuario.ToString()),
                new Claim("IdUsuario", usuario_encontrado.IdUsuario.ToString()),
                new Claim("Rol", rol)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            HttpContext.User = new ClaimsPrincipal(claimsIdentity);

            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("IniciarSesion", "InicioSesion");
        }

        public IActionResult RecuperacionEnviada()
        {
            return View();
        }

        public async Task<IActionResult> RestablecerContrasena(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ToastrMessage"] = "El token es nulo o inválido.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction("IniciarSesion");
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.RecoveryToken == token &&
                                          u.RecoveryTokenExpirationDate >= DateTime.UtcNow);

            if (usuario == null)
            {
                TempData["ToastrMessage"] = "No se pudo restablecer la contraseña. El enlace ha expirado o es inválido.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction("IniciarSesion");
            }

            ViewBag.Token = token;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerContrasena(string token, string nuevaContrasena)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ToastrMessage"] = "El token es nulo o inválido.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction("IniciarSesion");
            }

            if (string.IsNullOrEmpty(nuevaContrasena))
            {
                TempData["ToastrMessage"] = "Debes ingresar la nueva contraseña.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction("RestablecerContrasena", new { token });
            }

            if (nuevaContrasena.Length < 10 || nuevaContrasena.Length > 30)
            {
                TempData["ToastrMessage"] = "La longitud de tu contraseña debe ser mayor a 10 y menor a 30.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction("RestablecerContrasena", new { token });
            }

            if (!HasUpperCase(nuevaContrasena))
            {
                TempData["ToastrMessage"] = "La contraseña debe tener una mayúscula";
                TempData["ToastrType"] = "danger";
                return RedirectToAction("RestablecerContrasena", new { token });
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.RecoveryToken == token &&
                                          u.RecoveryTokenExpirationDate >= DateTime.UtcNow);

            if (usuario == null)
            {
                TempData["ToastrMessage"] = "No se pudo restablecer la contraseña. El enlace ha expirado o es inválido.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction("IniciarSesion");
            }

            usuario.Contraseña = Utilidades.EncriptarClave(nuevaContrasena); ;
            usuario.RecoveryToken = null;
            usuario.RecoveryTokenExpirationDate = null;

            await _context.SaveChangesAsync();

            TempData["ToastrMessage"] = "La contraseña se ha restablecido con éxito.";
            TempData["ToastrType"] = "success";

            return RedirectToAction("IniciarSesion");
        }

        private bool HasUpperCase(string input)
        {
            return input.Any(char.IsUpper);
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarContrasena(string correo)
        {
            if (string.IsNullOrEmpty(correo))
            {
                TempData["ToastrMessage"] = "El correo proporcionado es inválido.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction("IniciarSesion");
            }

            Usuario usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo != null && u.Correo == correo);

            if (usuario != null)
            {
                int userId = usuario.IdUsuario;

                string recoveryToken = Guid.NewGuid().ToString();

                usuario.RecoveryToken = recoveryToken;
                usuario.RecoveryTokenExpirationDate = DateTime.UtcNow.AddHours(1);

                await _context.SaveChangesAsync();

                var recoveryLink = Url.Action("RestablecerContrasena", "InicioSesion", new { token = recoveryToken }, protocol: HttpContext.Request.Scheme);

                var subject = "Recuperación de Contraseña";
                var body = $"Hola {usuario.Nombre},<br><br>" +
                           "Recibes este correo porque has solicitado restablecer tu contraseña en nuestro sitio web. Estamos aquí para ayudarte a recuperar el acceso a tu cuenta de manera segura y sencilla.<br><br>" +
                           $"Por favor, haz clic en el siguiente enlace para restablecer tu contraseña:<br><a href=\"{recoveryLink}\">{recoveryLink}</a><br><br>" +
                           "Si no has solicitado este restablecimiento de contraseña, puedes ignorar este mensaje. Tu cuenta seguirá siendo segura.<br><br>" +
                           "¡Gracias por confiar en nosotros!<br><br>";


                await EnviarCorreo(correo, subject, body);

                TempData["ToastrMessage"] = "Se ha enviado un correo de recuperación de contraseña. Por favor, revisa tu bandeja de entrada.";
                TempData["ToastrType"] = "success";
                return RedirectToAction("IniciarSesion");
            }
            else
            {
                TempData["ToastrMessage"] = "No se pudo procesar la solicitud de recuperación de contraseña.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction("IniciarSesion");
            }
        }

        private async Task EnviarCorreo(string correoDestino, string asunto, string cuerpo)
        {
            try
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
            catch (SmtpException ex)
            {
                Console.WriteLine("Error al enviar el correo: " + ex.Message);
            }
        }
    }
}
