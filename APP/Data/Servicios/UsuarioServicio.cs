using APP.Data.Modelos;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace APP.Data.Servicios
{
	public class UsuarioServicio
	{
		public readonly ProtectedLocalStorage _protectedLocalStorage;
		public UsuarioServicio(ProtectedLocalStorage protectedLocalStorage)
		{
			_protectedLocalStorage = protectedLocalStorage;
		}

		public async Task<RespuestaConsumidor<RespuestaAPI<ModeloRegistrarUsuario>>> RegistrarUsuario(ModeloRegistrarUsuario modeloRegistrarUsuario)
		{
			RespuestaConsumidor< RespuestaAPI<ModeloRegistrarUsuario>> respuesta = new();

			try
			{
				respuesta = await Consumidor.Execute<ModeloRegistrarUsuario, RespuestaAPI<ModeloRegistrarUsuario>>($"https://localhost:7181/api/Usuario/registrarse", MethodHttp.POST, modeloRegistrarUsuario);
			}
			catch (Exception ex)
			{

			}
			return respuesta;
		}

		public async Task<RespuestaConsumidor<RespuestaAPI<RespuestaIniciarSesion>>> IniciarSesion(Usuario usuario)
		{
			RespuestaConsumidor<RespuestaAPI<RespuestaIniciarSesion>> respuesta = new();

			try
			{
				respuesta = await Consumidor.Execute<Usuario, RespuestaAPI<RespuestaIniciarSesion>>($"https://localhost:7181/api/Usuario/iniciosesion", MethodHttp.POST, usuario);

				if (respuesta.Ok)
				{
					await _protectedLocalStorage.SetAsync("jwt", respuesta.Data.Datos.jwt);
					await _protectedLocalStorage.SetAsync("idusuariosesion", respuesta.Data.Datos.idusuariosesion);
				}

			}
			catch (Exception ex)
			{

			}
			return respuesta;
		}

		public async Task CerrarSesion()
		{
			await _protectedLocalStorage.DeleteAsync("jwt");
			await _protectedLocalStorage.DeleteAsync("idusuariosesion");
		}
	}
}
