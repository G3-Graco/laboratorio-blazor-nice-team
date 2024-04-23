using APP.Data.Modelos;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace APP.Data.Servicios
{
    public class MovimientoServicio
    {
        public readonly ProtectedLocalStorage _protectedLocalStorage;
        public MovimientoServicio(ProtectedLocalStorage protectedLocalStorage)
        {
            _protectedLocalStorage = protectedLocalStorage;
        }

        public async Task<RespuestaConsumidor<RespuestaAPI<IEnumerable<Movimiento>>>> ConsultarMovimientos()
        {
            RespuestaConsumidor<RespuestaAPI<IEnumerable<Movimiento>>> respuesta = new();

            try
            {
                respuesta = await Consumidor.Execute<Movimiento, RespuestaAPI<IEnumerable<Movimiento>>>($"https://localhost:7181/api/Movimiento/movimientoscuenta", MethodHttp.GET, null, _protectedLocalStorage);
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

    }
}
