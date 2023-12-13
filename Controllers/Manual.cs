using Microsoft.AspNetCore.Mvc;

namespace LeamosColombiaProject.Controllers
{
    public class Manual : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult VerIniciarSesion()
        {
            return View("ManualIngresar"); 
        }

        public IActionResult VerManualUsuarios()
        {
            return View("ManualUsuarios"); 
        }
        public IActionResult VerProveedores()
        {
            return View("ManualIngresar");
        }

        public IActionResult VerCompras()
        {
            return View("ManualCompras");
        }
        public IActionResult VerProductos()
        {
            return View("ManualProductos");
        }
        public IActionResult VerClientes()
        {
            return View("ManualClientes");
        }
        public IActionResult VerVentas()
        {
            return View("ManualVentas");
        }
        public IActionResult VerCartera()
        {
            return View("ManualCartera");
        }
    }
}
