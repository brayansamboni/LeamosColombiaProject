using LeamosColombiaProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class AutorizacionVistaAttribute : TypeFilterAttribute
{
    public AutorizacionVistaAttribute(string modulo) : base(typeof(AutorizacionVistaFilter))
    {
        Arguments = new object[] { modulo };
    }
}

public class AutorizacionVistaFilter : IAuthorizationFilter
{
    private readonly string _modulo;

    public AutorizacionVistaFilter(string modulo)
    {
        _modulo = modulo ?? throw new ArgumentNullException(nameof(modulo));
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Obtén el ID de usuario (ajusta según tu implementación de usuarios)
        var userId = context.HttpContext.User.FindFirst("IdUsuario")?.Value;
        System.Diagnostics.Debug.WriteLine($"UserID: {userId}");


        // Asumiendo que tienes acceso a tu DbContext (ajusta según tu implementación)
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<LeamosColombiaProjectContext>();

        // Verifica si userId no es nulo y es un número entero válido
        if (int.TryParse(userId, out var userIdInt))
        {
            // Obtén el usuario desde la base de datos
            var usuario = dbContext.Usuarios
                .Include(u => u.IdRolNavigation.PermisoRols)
                    .ThenInclude(pr => pr.IdPermisoNavigation)
                .FirstOrDefault(u => u.IdUsuario == userIdInt);

            // Mensaje de depuración


            // Verifica si el usuario tiene el permiso necesario para el módulo
            var tienePermiso = usuario?.IdRolNavigation?.PermisoRols
                .Any(pr => pr.IdPermisoNavigation.Modulo == _modulo) ?? false;

            // Mensaje de depuración
            System.Diagnostics.Debug.WriteLine($"Módulo: {_modulo}, TienePermiso: {tienePermiso}");

            if (!tienePermiso)
            {
                // Mensaje de depuración
                System.Diagnostics.Debug.WriteLine("Acceso denegado");

                // Redirige a la vista de acceso denegado
                context.Result = new ViewResult { ViewName = "AccessDenied" };
            }
        }
        else
        {
            // Mensaje de depuración
            System.Diagnostics.Debug.WriteLine("UserID no válido");

            // Manejar el caso en que el userId no se puede parsear
            context.Result = new ViewResult { ViewName = "AccessDenied" };
        }
    }
}

