using LeamosColombiaProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LeamosColombiaProject.Controllers
{
    [Authorize]
    [AutorizacionVista("Clientes")]
    public class ReferenciasController : Controller
    {
        private readonly LeamosColombiaProjectContext _context;

        public ReferenciasController(LeamosColombiaProjectContext context)
        {
            _context = context;
        }

        // GET: Referencias
        [AutorizacionVista("Clientes")]
        public async Task<IActionResult> Index()
        {
            var leamosColombiaWebContext = _context.Referencia.Include(r => r.IdClienteNavigation);
            return View(await leamosColombiaWebContext.ToListAsync());
        }

        // GET: Referencias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Referencia == null)
            {
                return NotFound();
            }

            var referencium = await _context.Referencia
                .Include(r => r.IdClienteNavigation)
                .FirstOrDefaultAsync(m => m.IdReferencia == id);
            if (referencium == null)
            {
                return NotFound();
            }

            return View(referencium);
        }

        // GET: Referencias/Create

        public async Task<IActionResult> Create()
        {
            var activeClientes = await _context.Clientes
                .Where(c => c.Estado == true)
                .Select(c => new SelectListItem
                {
                    Value = c.IdCliente.ToString(),
                    Text = $"{c.Nombre} - ({c.Documento})"
                })
                .ToListAsync();

            ViewData["IdCliente"] = activeClientes;


            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://www.datos.gov.co/resource/xdk5-pm3f.json");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var ciudades = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
                    ViewData["Ciudades"] = ciudades
                        .Select(ciudad => $"{ciudad["departamento"]}, {ciudad["municipio"]}")
                        .ToList();
                }
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdReferencia,Documento,Nombre,Correo,Direccion,Telefono,Ciudad,Estado,IdCliente")] Referencia referencium)
        {

            _context.Add(referencium);
            await _context.SaveChangesAsync();
            TempData["ToastrMessage"] = "La referencia se ha creado exitosamente";
            TempData["ToastrType"] = "success";

            return RedirectToAction("Index", "Clientes");
        }

        // GET: Referencias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Referencia == null)
            {
                return NotFound();
            }

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://www.datos.gov.co/resource/xdk5-pm3f.json");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var ciudades = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
                    ViewData["Ciudades"] = ciudades
                        .Select(ciudad => $"{ciudad["departamento"]}, {ciudad["municipio"]}")
                        .ToList();
                }
            }

            var referencium = await _context.Referencia.FindAsync(id);
            if (referencium == null)
            {
                return NotFound();
            }

            var activeClientes = await _context.Clientes
                .Where(c => c.Estado == true)
                .Select(c => new SelectListItem
                {
                    Value = c.IdCliente.ToString(),
                    Text = $"{c.Documento} - {c.Nombre}"
                })
                .ToListAsync();

            ViewData["IdCliente"] = activeClientes;

            var ciudades1 = ViewData["Ciudades"] as List<string>;
            ViewData["Ciudades"] = ciudades1;

            ViewData["SelectedCiudad"] = referencium.Ciudad;

            return View(referencium);
        }

        [HttpGet]
        public async Task<IActionResult> VerificarDocumentoExistente(int id, string documento)
        {
            var referenciaExistente = await _context.Referencia
                .Where(r => r.IdReferencia != id && r.Documento == documento)
                .FirstOrDefaultAsync();

            var clienteExistente = await _context.Clientes
                .Where(c => c.IdCliente != id && c.Documento == documento)
                .FirstOrDefaultAsync();

            if (referenciaExistente != null)
            {
                return Json("El documento ya existe en otra referencia.");
            }
            else if (clienteExistente != null)
            {
                return Json("El documento ya existe en un cliente.");
            }

            return Json(true);
        }

        [HttpGet]
        public async Task<IActionResult> VerificarCorreoExistente(int id, string correo)
        {
            var referenciaExistente = await _context.Referencia
                .Where(r => r.IdReferencia != id && r.Correo == correo)
                .FirstOrDefaultAsync();

            var clienteExistente = await _context.Clientes
                .Where(c => c.IdCliente != id && c.Correo == correo)
                .FirstOrDefaultAsync();

            if (referenciaExistente != null)
            {
                return Json("El correo electrónico ya existe en otra referencia.");
            }
            else if (clienteExistente != null)
            {
                return Json("El correo electrónico ya existe en un cliente.");
            }

            return Json(true);
        }


        // POST: Referencias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdReferencia,Documento,Nombre,Correo,Direccion,Telefono,Ciudad,Estado,IdCliente")] Referencia referencium)
        {
            if (id != referencium.IdReferencia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingReferencia = await _context.Referencia.FindAsync(id);
                    if (existingReferencia == null)
                    {
                        return NotFound();
                    }

                    existingReferencia.Documento = referencium.Documento;
                    existingReferencia.Nombre = referencium.Nombre;
                    existingReferencia.Correo = referencium.Correo;
                    existingReferencia.Direccion = referencium.Direccion;
                    existingReferencia.Telefono = referencium.Telefono;
                    existingReferencia.Ciudad = referencium.Ciudad;
                    existingReferencia.Estado = referencium.Estado;

                    _context.Update(existingReferencia);
                    await _context.SaveChangesAsync();

                    int idCliente = (int)existingReferencia.IdCliente;

                    TempData["ToastrMessage"] = "¡La referencia se ha editado correctamente!";
                    TempData["ToastrType"] = "success";

                    return RedirectToAction("Details", "Clientes", new { id = idCliente });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReferenciumExists(referencium.IdReferencia))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", referencium.IdCliente);
            return View(referencium);
        }

        // GET: Referencias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Referencia == null)
            {
                return NotFound();
            }

            var referencium = await _context.Referencia
                .Include(r => r.IdClienteNavigation)
                .FirstOrDefaultAsync(m => m.IdReferencia == id);
            if (referencium == null)
            {
                return NotFound();
            }

            return View(referencium);
        }

        // POST: Referencias/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var referencium = await _context.Referencia.FindAsync(id);

            if (referencium != null)
            {
                int idCliente = (int)referencium.IdCliente;

                // Verificar si hay carteras con saldo mayor a 0 y estado activo
                bool hayCarterasConSaldo = _context.Carteras
                    .Any(c => c.IdVentaNavigation != null &&
                              c.IdVentaNavigation.IdCliente == idCliente &&
                              c.Saldo > 0 &&
                              c.Estado == true); // Verificar que el saldo sea mayor a 0 y el estado sea activo

                if (hayCarterasConSaldo)
                {
                    TempData["ToastrMessage"] = "No se puede eliminar la referencia porque existen carteras asociadas al cliente con saldo pendiente o carteras activas.";
                    TempData["ToastrType"] = "danger";
                    return RedirectToAction("Details", "Clientes", new { id = idCliente });
                }

                _context.Referencia.Remove(referencium);
                await _context.SaveChangesAsync();

                TempData["ToastrMessage"] = "¡La referencia se ha eliminado correctamente!";
                TempData["ToastrType"] = "success";

                return RedirectToAction("Details", "Clientes", new { id = idCliente });
            }

            return View(referencium);
        }



        // POST: Referencia/Habilitar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Habilitar(int id)
        {
            var referencia = await _context.Referencia.FindAsync(id);
            if (referencia != null)
            {
                referencia.Estado = true;
                _context.Update(referencia);
                await _context.SaveChangesAsync();

                int idCliente = (int)referencia.IdCliente;

                TempData["ToastrMessage"] = "¡La referencia se ha habilitado correctamente!";
                TempData["ToastrType"] = "success";

                return RedirectToAction("Details", "Clientes", new { id = idCliente });
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Referencia/Deshabilitar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inhabilitar(int id)
        {
            var referencia = await _context.Referencia.FindAsync(id);
            if (referencia != null)
            {
                referencia.Estado = false;
                _context.Update(referencia);
                await _context.SaveChangesAsync();

                int idCliente = (int)referencia.IdCliente;

                TempData["ToastrMessage"] = "¡La referencia se ha inhabilitado correctamente!";
                TempData["ToastrType"] = "success";

                return RedirectToAction("Details", "Clientes", new { id = idCliente });

            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReferenciumExists(int id)
        {
            return (_context.Referencia?.Any(e => e.IdReferencia == id)).GetValueOrDefault();
        }
    }
}
