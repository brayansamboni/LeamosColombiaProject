namespace LeamosColombiaProject.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string? Nombre { get; set; }

    public string? Correo { get; set; }

    public string? Contraseña { get; set; }

    public bool? Estado { get; set; }

    public int? IdRol { get; set; }

    public string? RecoveryToken { get; set; }
    public DateTime? RecoveryTokenExpirationDate { get; set; }
    public virtual Rol? IdRolNavigation { get; set; }
}
