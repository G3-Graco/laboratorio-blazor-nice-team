using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace APP.Components.Pages.SolicitarPrestamo
{
    public partial class SolicitarPrestamo : ComponentBase
    {
        [Inject]
		public NavigationManager Navigation { get; set; }

		[Inject]
		public PrestamoServicio prestamoServicio { get; set; }

		public Prestamo prestamo = new Prestamo();

		private Modal modal = default!;

		protected override async Task OnInitializedAsync()
        {
            prestamo.Fecha = DateTime.Now;
        }
        public async void Solicitar()
        {
			
		}

		public async Task CerrarModal()
		{
			await modal.HideAsync();
		}
    }
}