using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APP.Data.Modelos
{
    public class Documento
    {
        public int Id {get; set;}
        public byte[]? documento {get; set;}
        public string? Ubicacion {get; set;}
        public int IdTipo {get; set;}
        public virtual TipoDocumento? Tipo {get; set;}
        public int IdPrestamo {get; set;}
        public virtual Prestamo? prestamo {get; set;}
    }
}