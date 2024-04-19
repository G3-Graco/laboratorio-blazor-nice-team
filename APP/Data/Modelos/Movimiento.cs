namespace APP.Data.Modelos
{
    public class Movimiento
    {
        public int Id { get; set; }
        public double Monto  { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string CuentaRecipiente { get; set; }
        public string CuentaIdentificador { get; set; }
        public int TipoMovimientoId { get; set; }
    }
}