using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace APP.Components.Pages.IniciarSesion
{
	public partial class IniciarSesion : ComponentBase
	{
		[Inject]
		public NavigationManager Navigation { get; set; }

		[Inject]
		public UsuarioServicio UsuarioServicio { get; set; }

		public Usuario usuario = new Usuario();

		private Modal modal = default!;
		public string modalMensaje = "";

		public async void InicioSesion()
		{

			RespuestaConsumidor<RespuestaAPI<RespuestaIniciarSesion>> respuesta = await UsuarioServicio.IniciarSesion(usuario);

			if (respuesta.Ok)
			{
				if (respuesta.Data.Ok)
				{
					Navigation.NavigateTo("/", forceLoad: true);
				}
				else
				{
					modalMensaje = respuesta.Data.Mensaje;
					await modal.ShowAsync();
				}
			}
			else
			{
				//modalMensaje = respuesta.Mensaje;
				modalMensaje = $"statuscode:{respuesta.StatusCode}, ok:{respuesta.Ok}, data:{respuesta.Data}, mensaje:{respuesta.Mensaje}";
				await modal.ShowAsync();
			}
		}

		public async Task CerrarModal()
		{
			await modal.HideAsync();
		}

	}
}
