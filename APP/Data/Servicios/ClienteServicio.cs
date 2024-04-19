using APP.Data.Modelos;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace APP.Data.Servicios
{
	public class ClienteServicio
	{
        public readonly ProtectedLocalStorage _protectedLocalStorage;
        public ClienteServicio(ProtectedLocalStorage protectedLocalStorage)
        {
            _protectedLocalStorage = protectedLocalStorage;
        }
        public async Task<RespuestaConsumidor<RespuestaAPI<Cliente>>> ConsultarCliente()
		{
			RespuestaConsumidor<RespuestaAPI<Cliente>> respuesta = new();

			try
			{
				respuesta = await Consumidor.Execute<Cliente, RespuestaAPI<Cliente>>($"https://localhost:7181/api/Cliente/consultarcliente", MethodHttp.GET, null, _protectedLocalStorage);
			}
			catch (Exception ex)
			{

			}
			return respuesta;
		}
	}
}
