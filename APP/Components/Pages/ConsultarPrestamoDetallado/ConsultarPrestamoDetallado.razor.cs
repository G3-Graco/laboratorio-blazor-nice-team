using Microsoft.AspNetCore.Components;

namespace APP.Components.Pages.ConsultarPrestamoDetallado
{
    public partial class ConsultarPrestamoDetallado : ComponentBase
    {
        [Parameter]
		public int idprestamo { get; set; }
	}
}
