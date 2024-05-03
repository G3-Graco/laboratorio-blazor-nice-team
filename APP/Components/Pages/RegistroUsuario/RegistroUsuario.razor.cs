using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace APP.Components.Pages.RegistroUsuario
{
    public partial class RegistroUsuario : ComponentBase
    {
		[Inject]
		public NavigationManager Navigation { get; set; }

		[Inject]
		public UsuarioServicio UsuarioServicio { get; set; }

		public ModeloRegistrarUsuario modeloRegistrarUsuario = new ModeloRegistrarUsuario();

        private Modal modal = default!;
        public string ModalTitulo = "";
        public string ModalMensaje = "";
        public bool OcurrioError = false;

        public string Contrasena = "password";
		public IconName Icono = IconName.EyeSlash;

		private bool spinnerVisible = false;
		private void EsconderSpinner() => spinnerVisible = false;
		private void MostrarSpinner() => spinnerVisible = true;


		public void VerContrasena()
        {
            if (this.Contrasena == "password")
            {
                this.Contrasena = "text";
				this.Icono = IconName.Eye;

			}
            else
            {
                this.Contrasena = "password";
				this.Icono = IconName.EyeSlash;

			}
        }

        protected override async Task OnInitializedAsync()
        {
            modeloRegistrarUsuario.FechaNacimiento = DateOnly.FromDateTime(DateTime.Now);
        }
        public async Task MostrarModalError()
        {
            var parametros = new Dictionary<string, object>
            {
                { "OnclickCallback", EventCallback.Factory.Create<MouseEventArgs>(this, CerrarModal) },
                { "Mensaje", ModalMensaje }
            };

            await modal.ShowAsync<ContenidoModal>(ModalTitulo, parameters: parametros);
        }
        public async Task VerificarError()
        {
            if (OcurrioError)
            {
                await MostrarModalError();
            }
        }
        public async Task CerrarModal()
        {
            await modal.HideAsync();
        }

        public async void Registrar()
        {
			MostrarSpinner();
			RespuestaConsumidor<RespuestaAPI<ModeloRegistrarUsuario>> respuesta = await UsuarioServicio.RegistrarUsuario(modeloRegistrarUsuario);
			EsconderSpinner();

			if (respuesta.Ok)
			{
				if (respuesta.Data.Ok)
				{
                    ModalTitulo = "Registrado";
                    ModalMensaje = "Usuario registrado exitosamente";

                    var parametros = new Dictionary<string, object>
                    {
                        { "OnclickCallback", EventCallback.Factory.Create<MouseEventArgs>(this, async () => {
                        await modal.HideAsync();
                        Navigation.NavigateTo("/iniciarsesion", forceLoad: true);
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
				ModalMensaje = respuesta.Mensaje;
				ModalTitulo = "Error";
                await MostrarModalError();
             
            }
		}
    }
}
