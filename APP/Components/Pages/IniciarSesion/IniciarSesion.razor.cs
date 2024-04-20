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
		public string ModalTitulo = "";
		public string ModalMensaje = "";

		public async void InicioSesion()
		{

			RespuestaConsumidor<RespuestaAPI<RespuestaIniciarSesion>> respuesta = await UsuarioServicio.IniciarSesion(usuario);

			if (respuesta.Ok)
			{
				if (respuesta.Data.Ok)
				{
                    ModalTitulo = "Inicio sesión";
                    ModalMensaje = "Iniciaste sesión exitosamente";
                    await modal.ShowAsync();
      
                    Navigation.NavigateTo("/", forceLoad: true);
					//nice
				}
				else
				{
                    ModalTitulo = "Error";
                    ModalMensaje = respuesta.Data.Mensaje;
					await modal.ShowAsync();

				}
			}
			else
			{
                ModalTitulo = "Error";
                ModalMensaje = respuesta.Mensaje;
				await modal.ShowAsync();

			}
		}

		public async Task CerrarModal()
		{
			await modal.HideAsync();
		}

	}
}
