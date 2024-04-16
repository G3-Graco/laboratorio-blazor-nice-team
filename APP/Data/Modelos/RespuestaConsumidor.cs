namespace APP.Data.Modelos
{
	public class RespuestaConsumidor<Entidad>
	{
		public string? StatusCode { get; set; }
		public bool Ok { get; set; }
		public string? Mensaje { get; set; }
		public Entidad? Data { get; set; }
	}
}
