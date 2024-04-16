using APP.Data.Servicios;
using Microsoft.AspNetCore.Components;

namespace APP.Components.Pages.PaginaPrincipalLoggeada
{
	public partial class PagPrincipal : ComponentBase
	{
		[Inject]
		public NavigationManager Navigation { get; set; }

		[Inject]
		public ClienteServicio ClienteServicio { get; set; }
		public string NombreCliente { get; set; }
	}
}
