using LeamosColombiaProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LeamosColombiaProject.Controllers
{
    [Authorize]
    [AutorizacionVista("Productos")]
    public class ProductosController : Controller
    {
        private readonly LeamosColombiaProjectContext _context;

        public ProductosController(LeamosColombiaProjectContext context)
        {
            _context = context;
        }

        // GET: Productos
        [AutorizacionVista("Productos")]
        public async Task<IActionResult> Index()
        {
            var leamosColombiaProjectContext = _context.Productos.Include(p => p.EditorialNavigation).Include(p => p.IdCategoriaNavigation);
            return View(await leamosColombiaProjectContext.ToListAsync());
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["Editorial"] = new SelectList(_context.Editorial, "IdEditorial", "NombreEditorial");
            ViewData["IdCategoria"] = new SelectList(_context.Categoria, "IdCategoria", "Categoria");
            return View();
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (producto.ImagenArchivo != null)
            {
                producto.ImagenUrl = await GuardarImagen(producto.ImagenArchivo);
            }

            _context.Add(producto);
            await _context.SaveChangesAsync();
            TempData["ToastrMessage"] = "¡El producto se ha creado correctamente!";
            TempData["ToastrType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["Editorial"] = new SelectList(_context.Editorial, "IdEditorial", "NombreEditorial", producto.Editorial);
            ViewData["IdCategoria"] = new SelectList(_context.Categoria, "IdCategoria", "Categoria", producto.IdCategoria);
            return View(producto);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile nuevaImagen, [Bind("IdProducto,Titulo,Autor,Anio,NPaginas,Sinopsis,Precio,Estado,IdCategoria,Cantidad,Isbn,Editorial")] Producto producto)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            if (nuevaImagen != null)
            {
                producto.ImagenUrl = await GuardarImagen(nuevaImagen);
            }

            try
            {
                var existingProduct = await _context.Productos.FindAsync(id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Asigna la URL de la imagen existente al nuevo producto
                producto.ImagenUrl = existingProduct.ImagenUrl;

                _context.Entry(existingProduct).CurrentValues.SetValues(producto);

                if (nuevaImagen != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await nuevaImagen.CopyToAsync(memoryStream);
                        existingProduct.ImagenUrl = await GuardarImagen(nuevaImagen);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(producto.IdProducto))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["ToastrMessage"] = "¡El producto se ha editado correctamente!";
            TempData["ToastrType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        private async Task<string> GuardarImagen(IFormFile imagenArchivo)
        {
            try
            {
                if (imagenArchivo != null && imagenArchivo.Length > 0)
                {
                    var extension = Path.GetExtension(imagenArchivo.FileName).ToLower();
                    if (!extension.Equals(".jpg") && !extension.Equals(".jpeg") && !extension.Equals(".png"))
                    {
                        // Manejar el error de extensión no válida
                        Console.WriteLine("Extensión de archivo no válida.");
                        return null;
                    }


                    var nombreArchivo = $"{Guid.NewGuid().ToString("N")}{Path.GetExtension(imagenArchivo.FileName).ToLower()}";
                    var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagenesProductos", nombreArchivo);


                    using (var fileStream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await imagenArchivo.CopyToAsync(fileStream);
                    }

                    // Retornar la URL completa de la imagen
                    var baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value}";
                    var imageUrl = $"{baseUrl}/ImagenesProductos/{nombreArchivo}";

                    return imageUrl;
                }
                else
                {
                    Console.WriteLine("La imagen está vacía o nula.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Loguea la excepción para obtener más información sobre el problema
                Console.WriteLine($"Error al guardar la imagen: {ex.Message}");
                return null; // O maneja la excepción de acuerdo a tus necesidades
            }
        }


        // GET: Productos/VerificarISBNExistente
        [HttpGet]
        public async Task<IActionResult> VerificarISBNExistente(int id, string documento)
        {
            var isbnExistente = await _context.Productos
                .Where(c => c.IdProducto != id && c.Isbn == documento)
                .FirstOrDefaultAsync();

            if (isbnExistente != null)
            {
                return Json("El ISBN ya existe en otro producto.");
            }

            return Json(true);
        }


        // GET: Productos/ObtenerPrecioProducto
        [HttpGet]
        public IActionResult ObtenerPrecioProducto(int id)
        {
            var producto = _context.Productos.FirstOrDefault(p => p.IdProducto == id);

            if (producto != null)
            {
                return Json(new { precio = producto.Precio });
            }

            return Json(new { precio = 0 });
        }

        // POST: Productos/EliminarImagen/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarImagen(int id)
        {
            var producto = _context.Productos.Find(id);
            if (producto != null)
            {
                producto.ImagenUrl = null;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // POST: Productos/Habilitar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Habilitar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                producto.Estado = true;
                _context.Update(producto);
                await _context.SaveChangesAsync();
            }

            TempData["ToastrMessage"] = "¡El producto se ha habilitado correctamente!";
            TempData["ToastrType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        // POST: Productos/Deshabilitar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inhabilitar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                producto.Estado = false;
                _context.Update(producto);
                await _context.SaveChangesAsync();
            }

            TempData["ToastrMessage"] = "¡El producto se ha inhabilitado correctamente!";
            TempData["ToastrType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return (_context.Productos?.Any(e => e.IdProducto == id)).GetValueOrDefault();
        }
    }
}
