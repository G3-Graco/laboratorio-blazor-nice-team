using APP.Data.Modelos;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace APP.Data.Servicios
{
	public class CuentaServicio
	{
        public readonly ProtectedLocalStorage _protectedLocalStorage;
        public CuentaServicio(ProtectedLocalStorage protectedLocalStorage)
        {
            _protectedLocalStorage = protectedLocalStorage;
        }

        public async Task<RespuestaConsumidor<RespuestaAPI<Cuenta>>> ConsultarCuenta()
		{
			RespuestaConsumidor<RespuestaAPI<Cuenta>> respuesta = new();

			try
			{
				respuesta = await Consumidor.Execute<Cuenta, RespuestaAPI<Cuenta>>($"https://localhost:7181/api/Cuenta/cuentacliente", MethodHttp.GET, null, _protectedLocalStorage);
			}
			catch (Exception ex)
			{

			}
			return respuesta;
		}
	}
}
