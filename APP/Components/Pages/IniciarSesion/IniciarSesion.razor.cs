using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

		public async Task MostrarModalError()
		{
			var parametros = new Dictionary<string, object>
			{
				{ "OnclickCallback", EventCallback.Factory.Create<MouseEventArgs>(this, CerrarModal) },
				{ "Mensaje", ModalMensaje }
			};

			await modal.ShowAsync<ContenidoModal>(ModalTitulo, parameters: parametros);
		}

		public async void InicioSesion()
		{

			RespuestaConsumidor<RespuestaAPI<RespuestaIniciarSesion>> respuesta = await UsuarioServicio.IniciarSesion(usuario);

			if (respuesta.Ok)
			{
				if (respuesta.Data.Ok)
				{
                    ModalTitulo = "Inicio sesión";
                    ModalMensaje = "Iniciaste sesión exitosamente";

					var parametros = new Dictionary<string, object>
					{
						{ "OnclickCallback", EventCallback.Factory.Create<MouseEventArgs>(this, async () => {
						await modal.HideAsync();
						Navigation.NavigateTo("/", forceLoad: true); 
						})
						},
						{ "Mensaje", ModalMensaje }
					};

					await modal.ShowAsync<ContenidoModal>(ModalTitulo, parameters: parametros);
	
				}
				else
				{
                    ModalTitulo = "Error";
                    ModalMensaje = respuesta.Data.Mensaje;
					await MostrarModalError();
				}
			}
			else
			{
                ModalTitulo = "Error";
                ModalMensaje = respuesta.Mensaje;

				await MostrarModalError();

			}
		}

		public async Task CerrarModal()
		{
			await modal.HideAsync();
		}

	}
}
