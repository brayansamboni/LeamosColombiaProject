using LeamosColombiaProject.Models;
using LeamosColombiaProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeamosColombiaProject.Controllers
{
    [Authorize]
    [AutorizacionVista("Roles")]
    public class RolsController : Controller
    {
        private readonly LeamosColombiaProjectContext _context;

        public RolsController(LeamosColombiaProjectContext context)
        {
            _context = context;
        }

        // GET: Rols
        [AutorizacionVista("Roles")]
        public async Task<IActionResult> Index()
        {
            return _context.Rols != null ?
                        View(await _context.Rols.ToListAsync()) :
                        Problem("Entity set 'LeamosColombiaProjectContext.Rols'  is null.");
        }

        // GET: Rols/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Rols == null)
            {
                return NotFound();
            }

            var rol = await _context.Rols
                .FirstOrDefaultAsync(m => m.IdRol == id);
            if (rol == null)
            {
                return NotFound();
            }

            return View(rol);
        }

        // GET: Rols/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rols/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult Create(RolViewModel rolViewModel)
        {
            // Verifica si el modelo es válido antes de procesar los datos
            if (ModelState.IsValid)
            {
                try
                {
                    // Convierte el nombre del rol a minúsculas para la comparación sin distinción entre mayúsculas y minúsculas
                    var nombreRolMinusculas = rolViewModel.Nombre.ToLower();

                    // Verifica si ya existe un rol con el mismo nombre (ignorando mayúsculas y minúsculas)
                    if (_context.Rols.Any(r => r.Nombre.ToLower() == nombreRolMinusculas))
                    {
                        ModelState.AddModelError("Nombre", "Ya existe un rol con este nombre.");
                        return View(rolViewModel);
                    }

                    if (rolViewModel.Modulos == null || rolViewModel.Modulos.Count == 0)
                    {
                        TempData["ToastrMessage"] = "Seleccione al menos un módulo antes de crear el rol.";
                        TempData["ToastrType"] = "danger";
                        return View(rolViewModel);
                    }

                    // Crea un nuevo objeto Rol con los datos del formulario
                    var nuevoRol = new Rol
                    {
                        Nombre = rolViewModel.Nombre,
                        Estado = rolViewModel.Estado
                    };



                    // Guarda el nuevo rol en la base de datos
                    _context.Rols.Add(nuevoRol);
                    _context.SaveChanges();

                    // Ahora, procesa los permisos seleccionados y crea los registros en la tabla PermisoRol
                    foreach (var modulo in rolViewModel.Modulos)
                    {
                        var permiso = new Permiso
                        {
                            Modulo = modulo
                        };

                        // Guarda el nuevo permiso en la base de datos
                        _context.Permisos.Add(permiso);
                        _context.SaveChanges();  // Guarda después de agregar cada permiso

                        // Crea el registro en la tabla PermisoRol
                        var permisoRol = new PermisoRol
                        {
                            IdRolNavigation = nuevoRol,
                            IdPermisoNavigation = permiso
                        };

                        // Guarda el nuevo registro en la base de datos
                        _context.PermisoRols.Add(permisoRol);
                        _context.SaveChanges();  // Guarda después de agregar cada PermisoRol
                    }

                    // Agrega TempData específico
                    TempData["ToastrMessage"] = "¡El rol se ha creado correctamente!";
                    TempData["ToastrType"] = "success";

                    // Redirige a la página de inicio u otra página después de crear el rol
                    return RedirectToAction("Index", "Rols");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // Loguea el error en algún registro
                    // También podrías redirigir a una página de error o mostrar un mensaje de error en la vista
                    return View(rolViewModel);
                }
            }

            // Si el modelo no es válido, vuelve a mostrar el formulario con los errores
            return View(rolViewModel);
        }



        // GET: Rols/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Rols == null)
            {
                return NotFound();
            }

            var rol = await _context.Rols
                .Include(r => r.PermisoRols)
                .ThenInclude(pr => pr.IdPermisoNavigation)
                .FirstOrDefaultAsync(m => m.IdRol == id);

            if (rol == null)
            {
                return NotFound();
            }

            var rolViewModel = new RolViewModel
            {
                IdRol = rol.IdRol,
                Nombre = rol.Nombre,
                Estado = rol.Estado ?? false,
                Modulos = rol.PermisoRols.Select(pr => pr.IdPermisoNavigation.Modulo).ToList(),
            };

            return View(rolViewModel);
        }



        // POST: Rols/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RolViewModel rolViewModel)
        {
            if (id != rolViewModel.IdRol)
            {
                return NotFound();
            }

            // Convierte el nombre del rol a minúsculas para la comparación sin distinción entre mayúsculas y minúsculas
            var nombreRolMinusculas = rolViewModel.Nombre.ToLower();

            // Verifica si ya existe otro rol con el mismo nombre (ignorando mayúsculas y minúsculas)
            if (_context.Rols.Any(r => r.IdRol != id && r.Nombre.ToLower() == nombreRolMinusculas))
            {
                ModelState.AddModelError("Nombre", "Ya existe un rol con este nombre.");
                return View(rolViewModel);
            }

            if (rolViewModel.Modulos == null || rolViewModel.Modulos.Count == 0)
            {
                TempData["ToastrMessage"] = "Seleccione al menos un módulo antes de crear el rol.";
                TempData["ToastrType"] = "danger";
                return View(rolViewModel);
            }


            if (ModelState.IsValid)
            {
                try
                {

                    var rol = await _context.Rols
                        .Include(r => r.PermisoRols)
                        .ThenInclude(pr => pr.IdPermisoNavigation)
                        .FirstOrDefaultAsync(m => m.IdRol == id);

                    if (rol == null)
                    {
                        return NotFound();
                    }

                    rol.Nombre = rolViewModel.Nombre;
                    rol.Estado = rolViewModel.Estado;

                    var permisosActuales = rol.PermisoRols.Select(pr => pr.IdPermisoNavigation).ToList();
                    var permisosSeleccionados = rolViewModel.Modulos.Select(modulo => new Permiso { Modulo = modulo }).ToList();

                    var permisosEliminar = permisosActuales.Except(permisosSeleccionados).ToList();
                    foreach (var permisoEliminar in permisosEliminar)
                    {
                        var permisoRolEliminar = rol.PermisoRols.FirstOrDefault(pr => pr.IdPermiso == permisoEliminar.IdPermiso);
                        if (permisoRolEliminar != null)
                        {
                            _context.PermisoRols.Remove(permisoRolEliminar);
                        }
                    }

                    var permisosAgregar = permisosSeleccionados.Except(permisosActuales).ToList();
                    foreach (var permisoAgregar in permisosAgregar)
                    {
                        var permisoRolNuevo = new PermisoRol
                        {
                            IdRolNavigation = rol,
                            IdPermisoNavigation = permisoAgregar
                        };
                        _context.PermisoRols.Add(permisoRolNuevo);
                    }

                    await _context.SaveChangesAsync();

                    TempData["ToastrMessage"] = "¡El rol se ha editado correctamente!";
                    TempData["ToastrType"] = "success";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RolExists(rolViewModel.IdRol))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(rolViewModel);
        }


        // POST: Rols/Habilitar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Habilitar(int id)
        {
            var rol = await _context.Rols.FindAsync(id);
            if (rol != null)
            {
                rol.Estado = true;
                _context.Update(rol);
                await _context.SaveChangesAsync();

                var usuariosAsociados = await _context.Usuarios.Where(u => u.IdRol == id).ToListAsync();
                foreach (var usuario in usuariosAsociados)
                {
                    usuario.Estado = true;
                    _context.Update(usuario);
                }
                await _context.SaveChangesAsync();
            }

            TempData["ToastrMessage"] = "¡El rol se ha habilitado correctamente!";
            TempData["ToastrType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inhabilitar(int id)
        {
            var rol = await _context.Rols.FindAsync(id);
            if (rol != null)
            {
                rol.Estado = false;
                _context.Update(rol);
                await _context.SaveChangesAsync();

                var usuariosAsociados = await _context.Usuarios.Where(u => u.IdRol == id).ToListAsync();
                foreach (var usuario in usuariosAsociados)
                {
                    usuario.Estado = false;
                    _context.Update(usuario);
                }
                await _context.SaveChangesAsync();

                TempData["RolInhabilitado"] = true;
            }

            TempData["ToastrMessage"] = "¡El rol se ha inhabilitado correctamente!";
            TempData["ToastrType"] = "success";

            return RedirectToAction(nameof(Index));
        }



        private bool RolExists(int id)
        {
            return (_context.Rols?.Any(e => e.IdRol == id)).GetValueOrDefault();
        }
    }
}