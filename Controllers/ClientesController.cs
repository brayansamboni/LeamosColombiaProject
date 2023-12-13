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
    public class ClientesController : Controller
    {
        private readonly LeamosColombiaProjectContext _context;

        public ClientesController(LeamosColombiaProjectContext context)
        {
            _context = context;
        }

        // GET: Clientes
        [AutorizacionVista("Clientes")]
        public async Task<IActionResult> Index()
        {
            var leamosColombiaProjectContext = _context.Clientes.Include(c => c.IdTipoDocumentoNavigation);
            return View(await leamosColombiaProjectContext.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.IdTipoDocumentoNavigation)
                .Include(c => c.Referencia)
                .FirstOrDefaultAsync(m => m.IdCliente == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        public async Task<IActionResult> DetailsVentas(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.IdTipoDocumentoNavigation)
                .Include(c => c.Venta)
                .FirstOrDefaultAsync(m => m.IdCliente == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create.
        public async Task<IActionResult> Create()
        {
            ViewData["IdTipoDocumento"] = new SelectList(_context.TipoDocumentos, "IdTipoDocumento", "Documento");

            using (var httpClient = new HttpClient())
            {
                var responseCiudades = await httpClient.GetAsync("https://www.datos.gov.co/resource/xdk5-pm3f.json");
                if (responseCiudades.IsSuccessStatusCode)
                {
                    var jsonCiudades = await responseCiudades.Content.ReadAsStringAsync();
                    var ciudades = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonCiudades);

                    ViewData["Ciudades"] = ciudades
                        .Select(ciudad => $"{ciudad["departamento"]}, {ciudad["municipio"]}")
                        .ToList();
                }
            }

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://www.datos.gov.co/resource/muyy-6yw9.json");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var universidades = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

                    ViewData["Universidades"] = universidades
                        .GroupBy(universidad => universidad.ContainsKey("nombreinstitucion") ? universidad["nombreinstitucion"] : "Nombre Desconocido")
                        .Select(group => group.First())
                        .Select(universidad =>
                        {
                            string nombre = universidad.ContainsKey("nombreinstitucion") ? universidad["nombreinstitucion"] : "Nombre Desconocido";
                            return nombre;
                        })
                        .ToList();
                }
            }

            var cliente = new Cliente
            {
                Ciudad = "Selecciona la ciudad *",
            };

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Documento,Nombre,Correo,Direccion,Telefono,Ciudad,Universidad,Facultad,Estado,IdTipoDocumento")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                TempData["ToastrMessage"] = "¡El cliente se ha creado exitosamente!";
                TempData["ToastrType"] = "success";

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdTipoDocumento"] = new SelectList(_context.TipoDocumentos, "IdTipoDocumento", "Documento", cliente.IdTipoDocumento);
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }


            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }


            using (var httpClient = new HttpClient())
            {
                var responseCiudades = await httpClient.GetAsync("https://www.datos.gov.co/resource/xdk5-pm3f.json");
                if (responseCiudades.IsSuccessStatusCode)
                {
                    var jsonCiudades = await responseCiudades.Content.ReadAsStringAsync();
                    var ciudades = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonCiudades);

                    ViewData["Ciudades"] = ciudades
                        .Select(ciudad => $"{ciudad["departamento"]}, {ciudad["municipio"]}")
                        .ToList();
                }
            }


            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://www.datos.gov.co/resource/muyy-6yw9.json");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var universidades = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

                    ViewData["Universidades"] = universidades
                        .GroupBy(universidad => universidad.ContainsKey("nombreinstitucion") ? universidad["nombreinstitucion"] : "Nombre Desconocido")
                        .Select(group => group.First())
                        .Select(universidad =>
                        {
                            string nombre = universidad.ContainsKey("nombreinstitucion") ? universidad["nombreinstitucion"] : "Nombre Desconocido";
                            return nombre;
                        })
                        .ToList();
                }
            }

            ViewData["CiudadSeleccionada"] = cliente.Ciudad;

            ViewData["IdTipoDocumento"] = new SelectList(_context.TipoDocumentos, "IdTipoDocumento", "Documento", cliente.IdTipoDocumento);

            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCliente,Documento,Nombre,Correo,Direccion,Telefono,Ciudad,Universidad,Facultad,Estado,IdTipoDocumento")] Cliente cliente)
        {
            if (id != cliente.IdCliente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.IdCliente))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["ToastrMessage"] = "¡El cliente se ha editado correctamente!";
                TempData["ToastrType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTipoDocumento"] = new SelectList(_context.TipoDocumentos, "IdTipoDocumento", "IdTipoDocumento", cliente.IdTipoDocumento);
            return View(cliente);
        }


        // POST: Clientes/Habilitar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Habilitar(int id)
        {
            var cliente = await _context.Clientes.Include(c => c.Referencia).FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente != null)
            {
                cliente.Estado = true;

                foreach (var referencia in cliente.Referencia)
                {
                    referencia.Estado = true;
                }

                await _context.SaveChangesAsync();
                TempData["ToastrMessage"] = "¡El cliente y sus referencias se han habilitado correctamente!";
                TempData["ToastrType"] = "success";

            }

            return RedirectToAction(nameof(Index));
        }


        // POST: Clientes/Deshabilitar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inhabilitar(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Referencia)
                .Include(c => c.Venta)
                    .ThenInclude(v => v.Carteras)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente != null)
            {
                if (cliente.Venta.Any(v => v.Carteras.Any(ca => ca.Estado == true && ca.Saldo > 0)))
                {
                    TempData["ToastrMessage"] = "No se puede inhabilitar el cliente. Tiene carteras activas con saldo.";
                    TempData["ToastrType"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                cliente.Estado = false;

                foreach (var referencia in cliente.Referencia)
                {
                    referencia.Estado = false;
                }

                await _context.SaveChangesAsync();
                TempData["ToastrMessage"] = "¡El cliente y sus referencias se han inhabilitado correctamente!";
                TempData["ToastrType"] = "success";
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Clientes/VerificarDocumentoExistente/
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
                return Json("El documento ya existe en una referencia.");
            }
            else if (clienteExistente != null)
            {
                return Json("El documento ya existe en otro cliente.");
            }

            return Json(true);
        }

        // GET: Referencias/VerificarCorreoExistente/
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
                return Json("El correo electrónico ya existe en una referencia.");
            }
            else if (clienteExistente != null)
            {
                return Json("El correo electrónico ya existe en otro cliente.");
            }

            return Json(true);
        }

        private bool ClienteExists(int id)
        {
            return (_context.Clientes?.Any(e => e.IdCliente == id)).GetValueOrDefault();
        }
    }
}
