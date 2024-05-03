using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace APP.Components.Pages.Transferencia
{
	public partial class Transferencia : ComponentBase
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


		private bool spinnerVisible = false;
		private void EsconderSpinner() => spinnerVisible = false;
		private void MostrarSpinner() => spinnerVisible = true;

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
					await MostrarModalError();
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
			else
			{
				await MostrarModalError();
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
			else
			{
				await MostrarModalError();
			}
		}

		public async void Transferir()
		{
			MostrarSpinner();
			Movimiento transferencia = new Movimiento
			{
				Id = 0,
				CuentaOrigenIdentificador = Int64.Parse(modeloCuenta.CuentaIdentificador),
				CuentaReceptoraIdentificador = Int64.Parse(modeloMovimientos.CuentaReceptoraIdentificador),
				TipoMovimientoId = 1,
				Fecha = DateTime.UtcNow,
				Descripcion = modeloMovimientos.Descripcion,
				Monto = modeloMovimientos.Monto,

			};

			RespuestaConsumidor<RespuestaAPI<IEnumerable<TipoMovimiento>>> tipos = await movimientoServicio.ObtenerTipos();
			GestionarRespuesta<IEnumerable<TipoMovimiento>>(tipos);

			if (!OcurrioError)
			{
				tipos.Data.Datos.ToList().ForEach(x =>
				{
					if (x.Nombre == "Transferencia")
					{
						transferencia.TipoMovimientoId = x.Id;
					}
				});
			}
			else
			{
				await MostrarModalError();
			}


			RespuestaConsumidor<RespuestaAPI<Movimiento>> respuesta = await movimientoServicio.RealizarMovimiento(transferencia);
			GestionarRespuesta<Movimiento>(respuesta);
			EsconderSpinner();


			if (!OcurrioError)
			{
				ModalTitulo = "Éxito";
	            ModalMensaje = "Se hizo la transferencia exitosamente";

				var parametros = new Dictionary<string, object>
					{
						{ "OnclickCallback", EventCallback.Factory.Create<MouseEventArgs>(this, async () => {
						await modal.HideAsync();
						Navigation.NavigateTo("/resumen", forceLoad: true);
						})
						},
						{ "Mensaje", ModalMensaje }
					};

				await modal.ShowAsync<ContenidoModal>(ModalTitulo, parameters: parametros);
			}
			else
			{
				await MostrarModalError();
			}
		}

		public void Borrar()
		{
			modeloMovimientos.Monto = 0;
			modeloMovimientos.CuentaReceptoraIdentificador = "";
			modeloMovimientos.Descripcion = "";
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
	}
}