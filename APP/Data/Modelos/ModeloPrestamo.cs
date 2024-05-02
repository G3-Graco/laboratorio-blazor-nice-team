namespace APP.Data.Modelos
{
    public class ModeloPrestamo
    {
        public int NumeroCuotasDeseadas { get; set; }
		public double SueldoBasicoDelSolicitante { get; set; }
		public double MontoTotalDeseado { get; set; }
		public int IdClienteSolicitante { get; set; }
    }
}