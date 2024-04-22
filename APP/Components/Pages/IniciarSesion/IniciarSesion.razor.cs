using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace APP.Components.Pages.IniciarSesion
{
	public partial class IniciarSesion : ComponentBase
	{
		[CascadingParameter]
		public HttpContext? HttpContext { get; set; }
        [Inject]
        public ProtectedLocalStorage? protectedLocalStorage { get; set; }

        [Inject]
		public NavigationManager Navigation { get; set; }

		[Inject]
		public UsuarioServicio UsuarioServicio { get; set; }

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


        public async Task Autenticar()
		{
            var jwt = await protectedLocalStorage.GetAsync<string>("jwt");
            string token = jwt.Success ? jwt.Value : "";

            if(token == "")
            {
                ModalTitulo = "Error";
                ModalMensaje = "Grave error al iniciar sesión";
                await modal.ShowAsync();
                return;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, ""),
                new Claim("id", ""),
                new Claim(ClaimTypes.Role, "")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);


            ModalTitulo = "Inicio sesión";
            ModalMensaje = "Iniciaste sesión exitosamente";
            await modal.ShowAsync();
        }
		public async void InicioSesion()
		{

			RespuestaConsumidor<RespuestaAPI<RespuestaIniciarSesion>> respuesta = await UsuarioServicio.IniciarSesion(usuario);
            GestionarRespuesta<RespuestaIniciarSesion>(respuesta);

            if (!OcurrioError)
            {

                await Autenticar();
                
                Navigation.NavigateTo("/", forceLoad: true);
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
