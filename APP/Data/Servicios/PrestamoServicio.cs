using APP.Data.Modelos;

namespace APP.Data.Servicios
{
	public class PrestamoServicio
	{
		public async Task<RespuestaConsumidor<RespuestaAPI<IEnumerable<Prestamo>>>> ConsultarPrestamos()
		{
			RespuestaConsumidor<RespuestaAPI<IEnumerable<Prestamo>>> respuesta = new();

			try
			{
				//no es el controlador, editar cuando este disponible el controlador de préstamos en la api
				respuesta = await Consumidor.Execute<Cuenta, RespuestaAPI<IEnumerable<Prestamo>>>($"https://localhost:7181/api/Cuenta/cuentacliente", MethodHttp.POST, null);
			}
			catch (Exception ex)
			{

			}
			return respuesta;
		}
	}
}
