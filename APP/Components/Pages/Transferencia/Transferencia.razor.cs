using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace APP.Components.Pages.Transferencia
{
    public partial class Transferencia : ComponentBase
    {
        [Inject]
		public NavigationManager Navigation { get; set; }

		[Inject]
		public MovimientoServicio movimientoServicio { get; set; }

        public Movimiento transferencia = new Movimiento();

        private Modal modal = default!;

        protected override async Task OnInitializedAsync()
        {
            transferencia.Fecha = DateTime.Now;
        }

        public async void Transferir() {

        }

        public async Task CerrarModal()
		{
			await modal.HideAsync();
		}
    }
}