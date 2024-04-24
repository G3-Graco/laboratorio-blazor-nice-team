using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace APP.Components.Pages.SolicitarPrestamo
{
    public partial class SolicitarPrestamo : ComponentBase
    {
        [Inject]
		public NavigationManager Navigation { get; set; }

		[Inject]
		public PrestamoServicio prestamoServicio { get; set; }

		[Inject]
		public CuentaServicio cuentaServicio { get; set; }

		[Inject]
		public ClienteServicio clienteServicio { get; set; }

		private double Sueldo { get; set; }
		
		private int Empleo { get; set; }

		public string problema { get; set; }

		private IBrowserFile Identidad { get; set; }

		private IBrowserFile Trabajo { get; set; }

		private IWebHostEnvironment Environment;

		public Prestamo prestamo = new Prestamo();

		private Modal modal = default!;

		protected override async Task OnInitializedAsync()
        {
            prestamo.Fecha = DateTime.Now;
			Sueldo = 0;
			Empleo = 0;
        }
        public async void Solicitar()
        {
			var cliente = await clienteServicio.ConsultarCliente();
			var plazos = await prestamoServicio.ConsultarPlazos();
			long.TryParse(cliente.Data.Datos.FechaNacimiento, out long numero);
			var edad = new DateTime(numero);
			if (Empleo < 3)
			{
				problema = "Se requiere tener un trabajo en el que se haya trabajado al menos 3 meses";
				return;
			}
			if (prestamo.MontoTotal < (Sueldo * 3)) {
				problema = "El monto no puede ser menor al triple del sueldo";
				return;
			}
			if ((DateTime.Now.Year - edad.Year) < 18)
			{
				problema = "No puede hacer préstamos si es menor de eddad";
				return;
			}
			plazos.Data.Datos.ToList().ForEach(x => {
				if (prestamo.NumeroCuotas <= x.MaximaCuotas && 
					prestamo.NumeroCuotas >= x.MinimoCuotas)
				{
					prestamo.IdPlazo = x.Id;
				}
			});
			var respuesta = await prestamoServicio.CrearPrestamo(prestamo);
			if (respuesta.Ok)
			{
				problema = respuesta.Mensaje;
			}
		}

		public async Task CargarIdentidad(InputFileChangeEventArgs e) {
			try
			{
				var archivo = e.File;
				var nombre = Path.GetRandomFileName();
				var camino = Path.Combine(
					Environment.ContentRootPath, 
					Environment.EnvironmentName, 
					"Carga_insegura", 
					nombre
				);
				await using FileStream fs = new(camino, FileMode.Create);
				await archivo.OpenReadStream().CopyToAsync(fs);
				Identidad = archivo;
			}
			catch (Exception ex)
			{
				problema = ex.Message;
			}
			
		}

		public async Task CargarTrabajo(InputFileChangeEventArgs e) {
			try
			{
				var archivo = e.File;
				var nombre = Path.GetRandomFileName();
				var camino = Path.Combine(
					Environment.ContentRootPath, 
					Environment.EnvironmentName, 
					"Carga_insegura", 
					nombre
				);
				await using FileStream fs = new(camino, FileMode.Create);
				await archivo.OpenReadStream().CopyToAsync(fs);
				Trabajo = archivo;
			}
			catch (Exception ex)
			{
				problema = ex.Message;
			}
			
        }

		public async Task CerrarModal()
		{
			await modal.HideAsync();
		}
    }
}