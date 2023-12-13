namespace LeamosColombiaProject.Models;

public partial class Rol
{
    public int IdRol { get; set; }

    public string? Nombre { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<PermisoRol> PermisoRols { get; set; } = new List<PermisoRol>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
