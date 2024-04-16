namespace APP.Data.Modelos
{
	public class Usuario
	{
		public int Id { get; set; } //codigo autogenerado
		public string NombreUsuario { get; set; }
		public string Contrasena { get; set; }
		public int ClienteId { get; set; }

	}
}
