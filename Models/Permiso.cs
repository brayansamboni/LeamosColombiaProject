namespace LeamosColombiaProject.Models;

public partial class Permiso
{
    public int IdPermiso { get; set; }

    public string? Modulo { get; set; }

    public virtual ICollection<PermisoRol> PermisoRols { get; set; } = new List<PermisoRol>();
}
