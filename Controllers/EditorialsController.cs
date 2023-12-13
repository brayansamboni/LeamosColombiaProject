using LeamosColombiaProject.Models;
using LeamosColombiaProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeamosColombiaProject.Controllers
{
    [Authorize]

    public class EditorialsController : Controller
    {
        private readonly LeamosColombiaProjectContext _context;

        public EditorialsController(LeamosColombiaProjectContext context)
        {
            _context = context;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            var editoriales = await _context.Editorial.ToListAsync();
            var editorialesViewModel = editoriales.Select(c => new EditorialViewModel
            {
                IdEditorial = c.IdEditorial,
                NombreEditorial = c.NombreEditorial,
            }).ToList();

            return View(editorialesViewModel);
        }


        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditorialViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string nombreEditorial = viewModel.NombreEditorial.ToLower();

                if (_context.Editorial.Any(c => c.NombreEditorial.ToLower() == nombreEditorial))
                {
                    TempData["ToastrMessage"] = "Ya existe una editorial con este nombre.";
                    TempData["ToastrType"] = "danger";

                    return RedirectToAction(nameof(Index));
                }

                var editorial = new Editorial
                {
                    NombreEditorial = viewModel.NombreEditorial
                };

                _context.Add(editorial);
                await _context.SaveChangesAsync();

                TempData["ToastrMessage"] = "¡La editorial se ha creado correctamente!";
                TempData["ToastrType"] = "success";

                return RedirectToAction(nameof(Index));
            }

            return PartialView("_CreateCategoryModal", viewModel);
        }




        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Editorial == null)
            {
                return NotFound();
            }

            var categorias = await _context.Editorial.FindAsync(id);
            if (categorias == null)
            {
                return NotFound();
            }
            return View(categorias);
        }

        // POST: Categorias/Edit/5
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(int id, string Editorial)
        {
            if (id <= 0)
            {
                return BadRequest("ID de editorial no válido");
            }

            var editorial = _context.Editorial.Find(id);

            if (editorial == null)
            {
                return NotFound("Editorial no encontrada");
            }

            if (string.IsNullOrEmpty(Editorial))
            {
                ViewData["Error"] = "El campo es obligatorio.";
                return RedirectToAction("Index");
            }

            string nombreEditorial = Editorial.ToLower();

            if (_context.Editorial.Any(c => c.IdEditorial != id && c.NombreEditorial.ToLower() == nombreEditorial))
            {
                TempData["ToastrMessage"] = "Ya existe una editorial con este nombre.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction("Index");
            }

            editorial.NombreEditorial = Editorial;

            try
            {
                _context.SaveChanges();
                TempData["ToastrMessage"] = "¡La editorial se ha editado correctamente!";
                TempData["ToastrType"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar los cambios: " + ex.Message);
                return View(editorial);
            }
        }




        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Editorial == null)
            {
                return NotFound();
            }

            var editoriales = await _context.Editorial
                .FirstOrDefaultAsync(m => m.IdEditorial == id);
            if (editoriales == null)
            {
                return NotFound();
            }

            return View(editoriales);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var editoriales = await _context.Editorial.FindAsync(id);

            var productosAsociados = _context.Productos.Any(p => p.Editorial == id);

            if (editoriales == null)
            {
                return NotFound();
            }

            if (productosAsociados)
            {
                TempData["ToastrMessage"] = "No se puede eliminar la editorial porque tiene productos asociados.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            _context.Editorial.Remove(editoriales);
            await _context.SaveChangesAsync();

            TempData["ToastrMessage"] = "¡La editorial se ha eliminado correctamente!";
            TempData["ToastrType"] = "success";
            return RedirectToAction(nameof(Index));
        }


        private bool EditorialsExists(int id)
        {
            return (_context.Editorial?.Any(e => e.IdEditorial == id)).GetValueOrDefault();
        }
    }
}