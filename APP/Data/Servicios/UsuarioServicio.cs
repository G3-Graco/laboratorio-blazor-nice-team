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

		public async Task<Respuesta<ModeloRegistrarUsuario>> RegistrarUsuario(ModeloRegistrarUsuario modeloRegistrarUsuario)
		{
			Respuesta<ModeloRegistrarUsuario> respuesta = new();

			try
			{
				respuesta = await Consumidor.Execute<ModeloRegistrarUsuario, ModeloRegistrarUsuario>($"https://localhost:7181/api/Usuario/registrarse", MethodHttp.POST, modeloRegistrarUsuario);
			}
			catch (Exception ex)
			{

			}
			return respuesta;
		}

	}
}
