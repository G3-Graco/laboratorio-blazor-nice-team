using APP.Data.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;

namespace APP.Components.Pages.CerrarSesion
{
	public partial class CerrarSesion
	{
		[CascadingParameter]
		public HttpContext? HttpContext { get; set; }
		[Inject]
		public NavigationManager Navigation { get; set; }
        [Inject]
        public UsuarioServicio UsuarioServicio { get; set; }


        protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();

			if (HttpContext.User.Identity.IsAuthenticated)
			{
				await HttpContext.SignOutAsync();

                await UsuarioServicio.CerrarSesion();

                Navigation.NavigateTo("/cerrarsesion", forceLoad: true);
			}
		}
	}
}
