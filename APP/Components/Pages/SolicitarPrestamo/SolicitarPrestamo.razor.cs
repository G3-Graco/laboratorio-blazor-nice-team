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

		private double Sueldo { get; set; }
		
		private int Empleo { get; set; }

		private IBrowserFile Identidad { get; set; }

		private IBrowserFile Trabajo { get; set; }

		private IWebHostEnvironment Environment;

		public Prestamo prestamo = new Prestamo();

		private Modal modal = default!;

		protected override async Task OnInitializedAsync()
        {
            prestamo.Fecha = DateTime.Now;
			Sueldo = 0;
        }
        public async void Solicitar()
        {
			
		}

		public async Task CargarIdentidad(InputFileChangeEventArgs e) {
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
			// foreach (var file in e.GetMultipleFiles(1))
			// {
			// 	try
			// 	{
			// 		var trustedFileName = Path.GetRandomFileName();
			// 		var path = Path.Combine(Environment.ContentRootPath,
			// 			Environment.EnvironmentName, "unsafe_uploads",
			// 			trustedFileName);

			// 		await using FileStream fs = new(path, FileMode.Create);
			// 		await file.OpenReadStream(maxFileSize).CopyToAsync(fs);

			// 		loadedFiles.Add(file);

			// 		Logger.LogInformation(
			// 			"Unsafe Filename: {UnsafeFilename} File saved: {Filename}",
			// 			file.Name, trustedFileName);
			// 	}
			// 	catch (Exception ex)
			// 	{
			// 		Logger.LogError("File: {Filename} Error: {Error}", 
			// 			file.Name, ex.Message);
			// 	}
			// }
		}

		public async Task CargarTrabajo(InputFileChangeEventArgs e) {
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

		public async Task CerrarModal()
		{
			await modal.HideAsync();
		}
    }
}