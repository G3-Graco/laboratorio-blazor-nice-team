namespace APP.Data.Modelos
{
	public class ModeloRegistrarUsuario
	{
		public string? NombreUsuario { get; set; }
		public string? Contrasena { get; set; }
		public string? Nombre { get; set; }
		public string? Apellido { get; set; }
		public string? Cedula { get; set; }
		public string? Correo { get; set; }
		public DateOnly FechaNacimiento { get; set; }
		public string? Telefono { get; set; }
		public string? Direccion { get; set; }
	}
}
