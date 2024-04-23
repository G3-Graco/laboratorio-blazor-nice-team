namespace APP.Data.Modelos
{
	public class ModeloPagarCuota
	{
		public string CuentaIdentificador { get; set; } //int64
		public string CuotaId { get; set; } //int
		public string MontoTotal { get; set; } //double
		public string PrestamoId { get; set; } //int
	}
}
