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
		public string modalTitulo = "";
		public string modalMensaje = "";

		protected override async Task OnInitializedAsync()
        {
            modeloRegistrarUsuario.FechaNacimiento = DateOnly.FromDateTime(DateTime.Now);
        }
        public async void Registrar()
        {

			RespuestaConsumidor<RespuestaAPI<ModeloRegistrarUsuario>> respuesta = await UsuarioServicio.RegistrarUsuario(modeloRegistrarUsuario);

			if (respuesta.Ok)
			{
				if (respuesta.Data.Ok)
				{
                    await modal.ShowAsync<string>("Registrado", "Usuario registrado exitosamente");
                    Navigation.NavigateTo("/iniciarsesion", forceLoad: true);
					//nice
				}
				else
				{
					modalTitulo = "Error";
					modalMensaje = respuesta.Data.Mensaje;
					await modal.ShowAsync();
					// modal.ShowAsync<string>("Error", respuesta.Data.Mensaje);
				}
				
			}
			else
			{
				modalMensaje = respuesta.Mensaje;
				modalTitulo = "Error";
				//modalMensaje = $"statuscode:{respuesta.StatusCode}, ok:{respuesta.Ok}, data:{respuesta.Data}, mensaje:{respuesta.Mensaje}";
				await modal.ShowAsync();
				//await modal.ShowAsync<string>($"Error: {respuesta.StatusCode}", respuesta.Mensaje);
			}
		}

		public async Task CerrarModal()
		{
			await modal.HideAsync();
		}
    }
}
