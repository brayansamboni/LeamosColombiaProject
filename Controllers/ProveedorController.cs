using LeamosColombiaProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeamosColombiaProject.Controllers
{
    [Authorize]
    [AutorizacionVista("Proveedores")]

    public class ProveedorController : Controller
    {
        private readonly LeamosColombiaProjectContext _context;

        public ProveedorController(LeamosColombiaProjectContext context)
        {
            _context = context;
        }

        // GET: Proveedor
        [AutorizacionVista("Proveedores")]
        public async Task<IActionResult> Index()
        {
            return _context.Proveedors != null ?
                        View(await _context.Proveedors.ToListAsync()) :
                        Problem("Entity set 'LeamosColombiaProjectContext.Proveedors'  is null.");
        }

        // GET: Proveedor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Proveedors == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedors
                .Include(p => p.Compras)
                .FirstOrDefaultAsync(m => m.IdProveedor == id);
            if (proveedor == null)
            {
                return NotFound();
            }
            proveedor.Compras = proveedor.Compras.Where(c => c.Estado == true).ToList();

            return View(proveedor);
        }

        // GET: Proveedor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Proveedor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProveedor,Nombre,Encargado,Identificacion,numeroIdentificacion,Correo,Direccion,Telefono,Estado")] Proveedor proveedor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(proveedor);
                await _context.SaveChangesAsync();

                TempData["ToastrMessage"] = "¡El proveedor se ha creado correctamente!";
                TempData["ToastrType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            return View(proveedor);
        }

        // GET: Proveedor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Proveedors == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedors.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }
            return View(proveedor);
        }

        // POST: Proveedor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProveedor,Nombre,Encargado,Identificacion,numeroIdentificacion,Correo,Direccion,Telefono,Estado")] Proveedor proveedor)
        {
            if (id != proveedor.IdProveedor)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proveedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProveedorExists(proveedor.IdProveedor))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["ToastrMessage"] = "¡El proveedor se ha editado correctamente!";
                TempData["ToastrType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            return View(proveedor);
        }

        // POST: Proveedor/Habilitar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Habilitar(int id)
        {
            var proveedor = await _context.Proveedors.FindAsync(id);
            if (proveedor != null)
            {
                proveedor.Estado = true;
                _context.Update(proveedor);
                await _context.SaveChangesAsync();
            }

            TempData["ToastrMessage"] = "¡El proveedor se ha habilitado correctamente!";
            TempData["ToastrType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        // POST: Proveedor/Inhabilitar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inhabilitar(int id)
        {
            var proveedor = await _context.Proveedors.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound();
            }


            proveedor.Estado = false;
            _context.Update(proveedor);
            await _context.SaveChangesAsync();

            TempData["ToastrMessage"] = "¡El proveedor se ha inhabilitado correctamente!";
            TempData["ToastrType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        private bool ProveedorExists(int id)
        {
            return (_context.Proveedors?.Any(e => e.IdProveedor == id)).GetValueOrDefault();
        }
    }
}