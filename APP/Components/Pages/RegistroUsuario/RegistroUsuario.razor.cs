using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

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
		public string modalMensaje = "";

		protected override async Task OnInitializedAsync()
        {
            modeloRegistrarUsuario.FechaNacimiento = DateOnly.FromDateTime(DateTime.Now);
        }
        public async void Registrar()
        {

			Respuesta<ModeloRegistrarUsuario> respuesta = await UsuarioServicio.RegistrarUsuario(modeloRegistrarUsuario);

			if (respuesta.Ok)
			{
				Navigation.NavigateTo("/", forceLoad: true);
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
