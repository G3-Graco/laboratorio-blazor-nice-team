using APP.Data.Modelos;

namespace APP.Data.Servicios
{
	public class ClienteServicio
	{
		public async Task<RespuestaConsumidor<RespuestaAPI<Cliente>>> ConsultarCliente()
		{
			RespuestaConsumidor<RespuestaAPI<Cliente>> respuesta = new();

			try
			{
				respuesta = await Consumidor.Execute<Cliente, RespuestaAPI<Cliente>>($"https://localhost:7181/api/Usuario/registrarse", MethodHttp.POST, null);
			}
			catch (Exception ex)
			{

			}
			return respuesta;
		}
	}
}
