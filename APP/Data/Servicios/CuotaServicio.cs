using APP.Data.Modelos;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace APP.Data.Servicios
{
    public class CuotaServicio
    {
        public readonly ProtectedLocalStorage _protectedLocalStorage;
        public CuotaServicio(ProtectedLocalStorage protectedLocalStorage)
        {
            _protectedLocalStorage = protectedLocalStorage;
        }

        public async Task<RespuestaConsumidor<RespuestaAPI<IEnumerable<Cuota>>>> ConsultarCuotas(int idprestamo)
        {
            RespuestaConsumidor<RespuestaAPI<IEnumerable<Cuota>>> respuesta = new();

            try
            {
                respuesta = await Consumidor.Execute<Cuota, RespuestaAPI<IEnumerable<Cuota>>>($"https://localhost:7181/api/Cuota/cuotasprestamo?idPrestamo={idprestamo}", MethodHttp.GET, null, _protectedLocalStorage, true);
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }
    }
}
