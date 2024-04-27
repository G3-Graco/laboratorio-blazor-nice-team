using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

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

		[Inject]
		public DocumentoServicio documentoServicio { get; set; }

		private double Sueldo { get; set; }
		
		private int Empleo { get; set; }

		public string problema { get; set; }

		private FormFile Identidad { get; set; }

		private FormFile Trabajo { get; set; }

		private IWebHostEnvironment Environment;

		public Prestamo prestamo = new Prestamo();

        public string ModalTitulo = "";
        public string ModalMensaje = "";
        public bool OcurrioError = false;
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
			var estados = await prestamoServicio.ObtenerEstados();
			estados.Data.Datos.ToList().ForEach(x => {
				if (x.Nombre == "En proceso") prestamo.IdEstado = x.Id;
			});
			var respuestaIdentidad = await documentoServicio.SubirArchivo(Identidad);
			var respuestaTrabajo = await documentoServicio.SubirArchivo(Trabajo);
			if (respuestaIdentidad.Data.Datos.Ubicacion == "" || 
				respuestaTrabajo.Data.Datos.Ubicacion == "")
			{
				problema = "No se pudo guardar todos los documentos";
				return;
			}
			var respuesta = await prestamoServicio.CrearPrestamo(prestamo);
			if (respuesta.Ok)
			{
				problema = respuesta.Mensaje;
			}
			var documentoIdentidad = respuestaIdentidad.Data.Datos;
			var documentoTrabajo = respuestaTrabajo.Data.Datos;
			var tipos = await documentoServicio.ObtenerTipos();
			tipos.Data.Datos.ToList().ForEach(x => {
				if (x.Nombre == "Identificación") documentoIdentidad.IdTipo = x.Id;
				else if (x.Nombre == "Recibo") documentoTrabajo.IdTipo = x.Id;
			});
			documentoIdentidad.IdPrestamo = respuesta.Data.Datos.Id;
			documentoTrabajo.IdPrestamo = respuesta.Data.Datos.Id;
			await documentoServicio.AgregarArchivo(documentoIdentidad);
			await documentoServicio.AgregarArchivo(documentoTrabajo);
		}

		public async Task CargarIdentidad(InputFileChangeEventArgs e) {
			try
			{
				// var archivo = e.File;
				// var informacion = new FormFile(Empleo, )
				// var archivo = await documentoServicio.AgregarArchivo(e);
				// MemoryStream memoria = new MemoryStream();
				// await e.File.OpenReadStream().CopyToAsync(memoria);
				// Identidad = memoria.ToArray();
				
				var archivo = e.File;
				var nombre = Path.GetRandomFileName();
				var camino = Path.Combine(
					Environment.ContentRootPath, 
					Environment.EnvironmentName, 
					"Carga_insegura", 
					nombre
				);
				MemoryStream memoria = new MemoryStream();
				await archivo.OpenReadStream().CopyToAsync(memoria); 
				Identidad = new FormFile(memoria, 0, archivo.Size, "Identidad", archivo.Name);
				/*
				await using FileStream fs = new(camino, FileMode.Create);
				await archivo.OpenReadStream().CopyToAsync(fs);
				Identidad = archivo;
				*/
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
				MemoryStream memoria = new MemoryStream();
				await archivo.OpenReadStream().CopyToAsync(memoria); 
				Trabajo = new FormFile(memoria, 0, archivo.Size, "Trabajo", $"{archivo.Name}.{archivo.ContentType}");
				// MemoryStream memoria = new MemoryStream();
				// await e.File.OpenReadStream().CopyToAsync(memoria); 
				// Trabajo = memoria.ToArray();
				// Stream todo = new MemoryStream(Trabajo);
				// IFormFile informacion = new FormFile(todo, 0, Trabajo.Length, e.File.Name, e.File.Name);
				/*
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
				*/
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

        public async Task MostrarModalError()
        {
            var parametros = new Dictionary<string, object>
            {
                { "OnclickCallback", EventCallback.Factory.Create<MouseEventArgs>(this, CerrarModal) },
                { "Mensaje", ModalMensaje }
            };

            await modal.ShowAsync<ContenidoModal>(ModalTitulo, parameters: parametros);
        }
        public async Task VerificarError()
        {
            if (OcurrioError)
            {
                await MostrarModalError();
            }
        }
        public bool GestionarRespuesta<Entidad>(RespuestaConsumidor<RespuestaAPI<Entidad>> respuesta)
        {
            if (respuesta.Ok)
            {
                if (respuesta.Data.Ok)
                {
                    OcurrioError = false;
                }
                else
                {
                    ModalTitulo = "Error";
                    ModalMensaje = respuesta.Data.Mensaje;
                    OcurrioError = true;
                }
            }
            else
            {
                ModalTitulo = $"Error: \"{respuesta.StatusCode}\"";
                ModalMensaje = respuesta.Mensaje;
                OcurrioError = true;

            }

            return OcurrioError;
        }
    }
}