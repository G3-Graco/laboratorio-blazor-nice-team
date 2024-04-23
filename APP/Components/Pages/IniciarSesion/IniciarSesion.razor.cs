using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using BlazorServerAuthenticationAndAuthorization.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

using System.Text.Json;

namespace APP.Components.Pages.IniciarSesion
{
	public partial class IniciarSesion : ComponentBase
	{
		[Inject]
		public CustomAuthenticationStateProvider authStateProvider { get; set; }

		[Inject]
        public ProtectedLocalStorage? protectedLocalStorage { get; set; }

        [Inject]
		public NavigationManager Navigation { get; set; }

		[Inject]
		public UsuarioServicio UsuarioServicio { get; set; }
		[CascadingParameter]
		private Task<AuthenticationState> authenticationState { get; set; }
        string autstate = "";

		public Usuario usuario = new Usuario();

        private Modal modal = default!;
        public string ModalTitulo = "";
        public string ModalMensaje = "";
        public bool OcurrioError = false;

			

		public bool GestionarRespuesta<Entidad>(RespuestaConsumidor<RespuestaAPI<Entidad>> respuesta)
        {
            if (respuesta.Ok)
            {
                if (respuesta.Data.Ok)
                {
                    OcurrioError = false;
                }
                else
                {
                    ModalTitulo = "Error";
                    ModalMensaje = respuesta.Data.Mensaje;
                    OcurrioError = true;
                }
            }
            else
            {
                ModalTitulo = $"Error: \"{respuesta.StatusCode}\"";
                ModalMensaje = respuesta.Mensaje;
                OcurrioError = true;

            }

            return OcurrioError;
        }

		public async void InicioSesion()
		{

			RespuestaConsumidor<RespuestaAPI<RespuestaIniciarSesion>> respuesta = await UsuarioServicio.IniciarSesion(usuario);
            GestionarRespuesta<RespuestaIniciarSesion>(respuesta);

            if (!OcurrioError)
            {

				//CustomAuthenticationStateProvider customAuthStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
                /*await */
                authStateProvider.NotifyAuthenticationStateChanged();

				var authState = await authenticationState;
				var autstate = $"Hello {authState.User.Identity.Name}";

				//Navigation.NavigateTo("/inicio", forceLoad: true);
            }
            else
            {
                await modal.ShowAsync();
            }
		}

		public async Task CerrarModal()
		{
			await modal.HideAsync();
		}

	}
}
