using LeamosColombiaProject.Models;
using LeamosColombiaProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeamosColombiaProject.Controllers
{
    [Authorize]
    [AutorizacionVista("Productos")]

    public class CategoriasController : Controller
    {
        private readonly LeamosColombiaProjectContext _context;

        public CategoriasController(LeamosColombiaProjectContext context)
        {
            _context = context;
        }

        // GET: Categorias
        [AutorizacionVista("Productos")]
        public async Task<IActionResult> Index()
        {
            var categorias = await _context.Categoria.ToListAsync();
            var categoriasViewModel = categorias.Select(c => new CategoriaViewModel
            {
                IdCategoria = c.IdCategoria,
                Categoria = c.Categoria,
            }).ToList();

            return View(categoriasViewModel);
        }


        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string nombreCategoria = viewModel.Categoria.ToLower();

                if (_context.Categoria.Any(c => c.Categoria.ToLower() == nombreCategoria))
                {
                    TempData["ToastrMessage"] = "Ya existe una categoría con este nombre.";
                    TempData["ToastrType"] = "danger";

                    return RedirectToAction(nameof(Index));
                }

                var categoria = new Categorias
                {
                    Categoria = viewModel.Categoria
                };

                _context.Add(categoria);
                await _context.SaveChangesAsync();

                TempData["ToastrMessage"] = "¡La categoría se ha creado correctamente!";
                TempData["ToastrType"] = "success";

                return RedirectToAction(nameof(Index));
            }

            return PartialView("_CreateCategoryModal", viewModel);
        }




        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categoria == null)
            {
                return NotFound();
            }

            var categorias = await _context.Categoria.FindAsync(id);
            if (categorias == null)
            {
                return NotFound();
            }
            return View(categorias);
        }

        // POST: Categorias/Edit/5
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(int id, string Categoria)
        {
            if (id <= 0)
            {
                return BadRequest("ID de categoría no válido");
            }

            var categoria = _context.Categoria.Find(id);

            if (categoria == null)
            {
                return NotFound("Categoría no encontrada");
            }

            if (string.IsNullOrEmpty(Categoria))
            {
                ViewData["Error"] = "El campo es obligatorio.";
                return RedirectToAction("Index");
            }

            string nombreCategoria = Categoria.ToLower();

            if (_context.Categoria.Any(c => c.IdCategoria != id && c.Categoria.ToLower() == nombreCategoria))
            {
                TempData["ToastrMessage"] = "Ya existe una categoría con este nombre.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction("Index");
            }

            categoria.Categoria = Categoria;

            try
            {
                _context.SaveChanges();
                TempData["ToastrMessage"] = "¡La categoría se ha editado correctamente!";
                TempData["ToastrType"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar los cambios: " + ex.Message);
                return View(categoria);
            }
        }




        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categoria == null)
            {
                return NotFound();
            }

            var categorias = await _context.Categoria
                .FirstOrDefaultAsync(m => m.IdCategoria == id);
            if (categorias == null)
            {
                return NotFound();
            }

            return View(categorias);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categoria.FindAsync(id);

            var productosAsociados = _context.Productos.Any(p => p.IdCategoria == id);

            if (categoria == null)
            {
                return NotFound();
            }

            if (productosAsociados)
            {
                TempData["ToastrMessage"] = "No se puede eliminar la categoría porque tiene productos asociados.";
                TempData["ToastrType"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            _context.Categoria.Remove(categoria);
            await _context.SaveChangesAsync();

            TempData["ToastrMessage"] = "¡La categoría se ha eliminado correctamente!";
            TempData["ToastrType"] = "success";
            return RedirectToAction(nameof(Index));
        }


        private bool CategoriasExists(int id)
        {
            return (_context.Categoria?.Any(e => e.IdCategoria == id)).GetValueOrDefault();
        }
    }
}