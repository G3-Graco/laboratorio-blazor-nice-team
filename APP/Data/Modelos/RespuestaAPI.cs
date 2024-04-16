namespace APP.Data.Modelos
{
	public class RespuestaAPI<Entidad>
	{
		public bool Ok { get; set; }
		public string? Mensaje { get; set; }
		public Entidad? Datos { get; set; }
	}
}
