using APP.Data.Modelos;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace APP.Data.Servicios
{
	public class PrestamoServicio
	{
		public async Task<RespuestaConsumidor<RespuestaAPI<IEnumerable<Prestamo>>>> ConsultarPrestamos()
		{
			RespuestaConsumidor<RespuestaAPI<IEnumerable<Prestamo>>> respuesta = new();
            try
            {
				respuesta = await Consumidor.Execute<Prestamo, RespuestaAPI<IEnumerable<Prestamo>>>($"https://localhost:7181/api/Prestamo", MethodHttp.GET, null, _protectedLocalStorage);
			}
			catch (Exception ex)
			{

			}
			return respuesta;
		}
	}
}
