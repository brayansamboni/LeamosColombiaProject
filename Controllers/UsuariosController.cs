using LeamosColombiaProject.Models;
using LeamosColombiaProject.resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LeamosColombiaProject.Controllers
{
    [Authorize]
    [AutorizacionVista("Usuarios")]
    public class UsuariosController : Controller
    {
        private readonly LeamosColombiaProjectContext _context;

        public UsuariosController(LeamosColombiaProjectContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        [AutorizacionVista("Usuarios")]
        public async Task<IActionResult> Index()
        {
            var leamosColombiaProjectContext = _context.Usuarios.Include(u => u.IdRolNavigation);
            return View(await leamosColombiaProjectContext.ToListAsync());
        }



        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            // Filtrar roles activos
            var rolesActivos = _context.Rols.Where(r => r.Estado == true).ToList();

            // Crear la lista de selección con roles activos
            ViewData["IdRol"] = new SelectList(rolesActivos, "IdRol", "Nombre");

            return View();
        }


        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,Nombre,Correo,Contraseña,Estado,IdRol")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (usuario.IdRol == 1)
                {
                    if (_context.Usuarios.Any(u => u.IdRol == 1))
                    {
                        TempData["ToastrMessage"] = "Ya existe un usuario con el rol Administrador";
                        TempData["ToastrType"] = "danger";
                        ViewData["IdRol"] = new SelectList(_context.Rols, "IdRol", "Nombre", usuario.IdRol);
                        return View(usuario);
                    }
                }

                usuario.Contraseña = Utilidades.EncriptarClave(usuario.Contraseña);
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                TempData["ToastrMessage"] = "Se ha creado el usuario correctamente";
                TempData["ToastrType"] = "success";
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdRol"] = new SelectList(_context.Rols, "IdRol", "Nombre", usuario.IdRol);
            return View(usuario);
        }



        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            if (usuario.IdRol == 1)
            {
                if (_context.Usuarios.Any(u => u.IdRol == 1 && u.IdUsuario != id))
                {
                    TempData["ToastrMessage"] = "Ya existe un usuario con el rol Administrador.";
                    TempData["ToastrType"] = "danger";
                    ViewData["IdRol"] = new SelectList(_context.Rols, "IdRol", "Nombre", usuario.IdRol);
                    return View(usuario);
                }
            }

            ViewData["IdRol"] = new SelectList(_context.Rols, "IdRol", "Nombre", usuario.IdRol);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,Nombre,Correo,Contraseña,Estado,IdRol")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (usuario.IdRol == 1)
                {
                    if (_context.Usuarios.Any(u => u.IdRol == 1 && u.IdUsuario != id))
                    {
                        TempData["ToastrMessage"] = "Ya existe un usuario con el rol Administrador.";
                        TempData["ToastrType"] = "danger";
                        ViewData["IdRol"] = new SelectList(_context.Rols, "IdRol", "Nombre", usuario.IdRol);
                        return View(usuario);
                    }
                }

                _context.Update(usuario);
                await _context.SaveChangesAsync();
                TempData["ToastrMessage"] = "Se ha editado el usuario correctamente";
                TempData["ToastrType"] = "success";
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdRol"] = new SelectList(_context.Rols, "IdRol", "IdRol", usuario.IdRol);
            return View(usuario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Habilitar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                var rolAsociado = await _context.Rols.FindAsync(usuario.IdRol);
                if (rolAsociado != null && (rolAsociado.Estado == null || rolAsociado.Estado == false))
                {
                    TempData["ToastrMessage"] = "No se puede habilitar el usuario porque el rol asociado está inhabilitado.";
                    TempData["ToastrType"] = "warning";
                    return RedirectToAction(nameof(Index));
                }

                usuario.Estado = true;
                _context.Update(usuario);
                await _context.SaveChangesAsync();
            }

            TempData["ToastrMessage"] = "¡El usuario se ha habilitado correctamente!";
            TempData["ToastrType"] = "success";

            return RedirectToAction(nameof(Index));
        }


        // POST: Productos/Deshabilitar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inhabilitar(int id)
        {
            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios != null)
            {
                usuarios.Estado = false;
                _context.Update(usuarios);
                await _context.SaveChangesAsync();
            }

            TempData["ToastrMessage"] = "¡El usuario se ha inhabilitado correctamente!";
            TempData["ToastrType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        // GET: Usuarios/VerificarCorreoExistente/
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

        private bool UsuarioExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.IdUsuario == id)).GetValueOrDefault();
        }

    }
}