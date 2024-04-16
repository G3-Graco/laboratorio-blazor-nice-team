using APP.Data.Modelos;

namespace APP.Data.Servicios
{
	public class CuentaServicio
	{
		public async Task<RespuestaConsumidor<RespuestaAPI<Cuenta>>> ConsultarCuenta()
		{
			RespuestaConsumidor<RespuestaAPI<Cuenta>> respuesta = new();

			try
			{
				respuesta = await Consumidor.Execute<Cuenta, RespuestaAPI<Cuenta>>($"https://localhost:7181/api/Cuenta/cuentacliente", MethodHttp.POST, null);
			}
			catch (Exception ex)
			{

			}
			return respuesta;
		}
	}
}
