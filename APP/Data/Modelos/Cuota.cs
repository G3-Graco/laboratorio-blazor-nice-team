namespace APP.Data.Modelos
{
    public class Cuota
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaPago { get; set; }
        public double Pago { get; set; }
        public int IdPrestamo { get; set; }
        public virtual Prestamo? prestamo { get; set; }
    }
}
