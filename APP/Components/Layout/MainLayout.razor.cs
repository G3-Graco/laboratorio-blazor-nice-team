using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace APP.Components.Layout
{
    public partial class MainLayout
    {
        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public UsuarioServicio UsuarioServicio { get; set; }
        private bool Iniciado;
        public async void CerrarSesion()
        {
            UsuarioServicio.CerrarSesion();
            Iniciado = false;
            Navigation.NavigateTo("/", forceLoad: true);
            
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var jwt = await UsuarioServicio._protectedLocalStorage.GetAsync<string>("jwt");
                Iniciado = jwt.Success;
            }
            catch (System.Exception)
            {
                Iniciado = false;
            }
            
        }

        public void Cambiar() {
            Iniciado = true;
        }

    }
}
