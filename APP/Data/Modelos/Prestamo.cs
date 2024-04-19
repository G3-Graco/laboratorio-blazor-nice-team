namespace APP.Data.Modelos
{
	public class Prestamo
	{
		public int Id { get; set; }
		public int NumeroCuotas { get; set; }
		public double MontoTotal { get; set; }
		public double CuotaMensual { get; set; }
		public DateTime Fecha { get; set; }
		public int IdEstado { get; set; }
		public EstadoPrestamo? Estado { get; set; }
		public int IdCliente { get; set; }
		public Cliente cliente { get; set; }
		public int IdPlazo { get; set; }
		public Plazo? plazo { get; set; }
	}
}
