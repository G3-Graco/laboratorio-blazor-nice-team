using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using System.Text.Json;

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

        public byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
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

            //
            var payload = token.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, keyValuePairs["unique_name"].ToString()),
                new Claim("id", keyValuePairs["id"].ToString()),
                new Claim(ClaimTypes.Role, "user") //bueno
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
                
                Navigation.NavigateTo("/inicio", forceLoad: true);
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
