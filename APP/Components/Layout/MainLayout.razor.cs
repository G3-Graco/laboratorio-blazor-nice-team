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

            Navigation.NavigateTo("/inicio", forceLoad: true);
            
        }

        protected override async Task OnInitializedAsync()
        {
            var jwt = await UsuarioServicio._protectedLocalStorage.GetAsync<string>("jwt");
            Iniciado = jwt.Success;
        }

    }
}
