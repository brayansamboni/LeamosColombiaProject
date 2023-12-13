namespace LeamosColombiaProject.Models.ViewModels
{
    public class RolViewModel
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; }
        public bool Estado { get; set; }
        public List<string> Modulos { get; set; }
    }

}
