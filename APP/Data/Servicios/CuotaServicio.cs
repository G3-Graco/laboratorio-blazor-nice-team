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

        public async Task<RespuestaConsumidor<RespuestaAPI<IEnumerable<Cuota>>>> ConsultarCuotasPagables()
        {
            RespuestaConsumidor<RespuestaAPI<IEnumerable<Cuota>>> respuesta = new();

            try
            {
                respuesta = await Consumidor.Execute<Cuota, RespuestaAPI<IEnumerable<Cuota>>>($"https://localhost:7181/api/Cuota/cuotaspagables", MethodHttp.GET, null, _protectedLocalStorage);
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

		public async Task<RespuestaConsumidor<RespuestaAPI<Cuota>>> ConsultarCuota(int idcuota)
		{
			RespuestaConsumidor<RespuestaAPI<Cuota>> respuesta = new();

			try
			{
				respuesta = await Consumidor.Execute<Cuota, RespuestaAPI<Cuota>>($"https://localhost:7181/api/Cuota/{idcuota}", MethodHttp.GET, null);
			}
			catch (Exception ex)
			{

			}
			return respuesta;
		}
	}
}
