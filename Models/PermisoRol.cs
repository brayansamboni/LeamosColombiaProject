namespace LeamosColombiaProject.Models;

public partial class PermisoRol
{
    public int IdPermisoRol { get; set; }

    public int? IdPermiso { get; set; }

    public int? IdRol { get; set; }

    public virtual Permiso? IdPermisoNavigation { get; set; }

    public virtual Rol? IdRolNavigation { get; set; }
}
