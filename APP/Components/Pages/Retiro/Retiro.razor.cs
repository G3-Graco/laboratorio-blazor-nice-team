using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace APP.Components.Pages.Retiro
{
	public partial class Retiro : ComponentBase
	{
		private bool isConnected;

		[Inject]
		public ClienteServicio ClienteServicio { get; set; }
		public string NombreCliente = "";

		[Inject]
		public NavigationManager Navigation { get; set; }

		[Inject]
		public MovimientoServicio movimientoServicio { get; set; }

		[Inject]
		public CuentaServicio CuentaServicio { get; set; }

		public ModeloMovimientos modeloMovimientos = new ModeloMovimientos();

		public ModeloCuenta modeloCuenta = new ModeloCuenta();

		private Modal modal = default!;
		public string ModalTitulo = "";
		public string ModalMensaje = "";
		public bool OcurrioError = false;

		private string Cambio { get; set; }

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				isConnected = true;

				await ObtenerCuentaDatos();
				await ObtenerNombreCliente();
				await VerificarError();

				StateHasChanged();
			}
		}

		public async Task VerificarError()
		{
			if (OcurrioError)
			{
				await modal.ShowAsync();
			}
		}

		public async Task CerrarModal()
		{
			await modal.HideAsync();
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
		private async Task ObtenerNombreCliente()
		{
			RespuestaConsumidor<RespuestaAPI<Cliente>> respuesta = await ClienteServicio.ConsultarCliente();

			GestionarRespuesta<Cliente>(respuesta);

			if (!OcurrioError)
			{
				NombreCliente = $"{respuesta.Data.Datos.Nombre} {respuesta.Data.Datos.Apellido}";
			}
		}

		private async Task ObtenerCuentaDatos()
		{
			RespuestaConsumidor<RespuestaAPI<Cuenta>> respuesta = await CuentaServicio.ConsultarCuenta();
			GestionarRespuesta<Cuenta>(respuesta);


			if (!OcurrioError)
			{
				modeloCuenta.CuentaIdentificador = respuesta.Data.Datos.Identificador.ToString();
				modeloCuenta.Saldo = respuesta.Data.Datos.Saldo.ToString();
			}
		}

		public async void Retirar()
		{
			Movimiento retiro = new Movimiento
			{
				Id = 0,
				CuentaOrigenIdentificador = Int64.Parse(modeloCuenta.CuentaIdentificador),
				//CuentaReceptoraIdentificador = Int64.Parse(modeloMovimientos.CuentaReceptoraIdentificador),
				TipoMovimientoId = 2,
				Fecha = DateTime.UtcNow,
				Descripcion = modeloMovimientos.Descripcion,
				Monto = double.Parse(modeloMovimientos.Monto),

			};

			RespuestaConsumidor<RespuestaAPI<IEnumerable<TipoMovimiento>>> tipos = await movimientoServicio.ObtenerTipos();
			GestionarRespuesta<IEnumerable<TipoMovimiento>>(tipos);

			if (!OcurrioError)
			{
				tipos.Data.Datos.ToList().ForEach(x =>
				{
					if (x.Nombre == "Retiro")
					{
						retiro.TipoMovimientoId = x.Id;
					}
				});
			}

			RespuestaConsumidor<RespuestaAPI<Movimiento>> respuesta = await movimientoServicio.RealizarMovimiento(retiro);
			GestionarRespuesta<Movimiento>(respuesta);


			if (!OcurrioError)
			{
				ModalTitulo = "Éxito";
	            ModalMensaje = "Se hizo el depósito exitósamente";

				await modal.ShowAsync();

				Navigation.NavigateTo("/", forceLoad: true);
			}
			else
			{
				VerificarError();
			}
		}

		public void Borrar()
		{
			modeloMovimientos.Monto = "";
			//modeloMovimientos.CuentaReceptoraIdentificador = "";
			modeloMovimientos.Descripcion = "";
		}

	}
}