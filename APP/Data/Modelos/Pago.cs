namespace APP.Data.Modelos
{
    public class Pago
    {
        public int Id { get; set; }
        public Int64 CuentaIdentificador { get; set; }
        public Cuenta? CuentaOrigen { get; set; }
        public int CuotaId { get; set; }
        public Cuota? CuotaPagada { get; set; }
    }
}
