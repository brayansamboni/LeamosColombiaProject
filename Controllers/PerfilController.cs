using LeamosColombiaProject.Models;
using LeamosColombiaProject.Models.ViewModels;
using LeamosColombiaProject.resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LeamosColombiaProject.Controllers
{
    public class PerfilController : Controller
    {

        private readonly LeamosColombiaProjectContext _context;

        public PerfilController(LeamosColombiaProjectContext context)
        {
            _context = context;
        }

        // GET: Perfil
        public async Task<IActionResult> Perfil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario.ToString() == userId);


            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Perfil/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario.ToString() == userId);

            if (usuario == null)
            {
                return NotFound();
            }


            var usuariosViewModel = new UsuariosViewModel
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Estado = usuario.Estado,
            };

            return View(usuariosViewModel);
        }


        // POST: Perfil/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuariosViewModel usuariosViewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios.FindAsync(usuariosViewModel.IdUsuario);

                if (usuario == null)
                {
                    return NotFound();
                }

                usuario.Nombre = usuariosViewModel.Nombre;
                usuario.Correo = usuariosViewModel.Correo;
                usuario.Estado = usuariosViewModel.Estado;

                _context.Update(usuario);
                await _context.SaveChangesAsync();

                TempData["ToastrMessage"] = "¡Importante! - Debes cerrar sesión para que se guarden los cambios.";
                TempData["ToastrType"] = "info";

                return RedirectToAction("Edit", new { id = usuario.IdUsuario });
            }

            return View(usuariosViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarContrasena(string ContrasenaActual, string NuevaContrasena)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = await _context.Usuarios.FindAsync(int.Parse(userId));

            if (usuario != null)
            {
                if (Utilidades.EncriptarClave(ContrasenaActual) == usuario.Contraseña)
                {
                    usuario.Contraseña = Utilidades.EncriptarClave(NuevaContrasena);
                    await _context.SaveChangesAsync();

                    TempData["ToastrMessage"] = "Contraseña cambiada exitosamente.";
                    TempData["ToastrType"] = "success";
                }
                else
                {
                    TempData["ToastrMessage"] = "La contraseña actual es incorrecta.";
                    TempData["ToastrType"] = "danger";
                }
            }
            else
            {
                TempData["ToastrMessage"] = "Usuario no encontrado.";
                TempData["ToastrType"] = "danger";
            }

            return RedirectToAction("Edit", new { id = usuario.IdUsuario });
        }


        // GET: Perfil/VerificarCorreoExistente/
        [HttpGet]
        public async Task<IActionResult> VerificarCorreoExistente(int id, string correo)
        {
            var usuarioExistente = await _context.Usuarios
                .Where(c => c.IdUsuario != id && c.Correo == correo)
                .FirstOrDefaultAsync();

            if (usuarioExistente != null)
            {
                return Json("El correo electrónico ya existe en otro cliente.");
            }

            return Json(true);
        }



        [HttpGet]
        public IActionResult VerificarMayuscula(string contrasena)
        {
            if (string.IsNullOrEmpty(contrasena) || !HasUpperCase(contrasena))
            {
                return Json("La contraseña debe contener al menos una mayúscula.");
            }

            return Json(true);
        }


        private bool HasUpperCase(string input)
        {
            return input.Any(char.IsUpper);
        }
    }
}
