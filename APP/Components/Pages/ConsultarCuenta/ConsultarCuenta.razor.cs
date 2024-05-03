using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace APP.Components.Pages.ConsultarCuenta
{
	public partial class ConsultarCuenta : ComponentBase
	{
		private bool isConnected;
		private Modal modal = default!;
		public string ModalTitulo = "";
		public string ModalMensaje = "";
		public bool OcurrioError = false;

		[Inject]
		public ClienteServicio ClienteServicio { get; set; }
		public string NombreCliente = "";

		[Inject]
		public CuentaServicio CuentaServicio { get; set; }

		public List<Cuenta>? cuentas = new List<Cuenta>();

		[Inject]
		public MovimientoServicio MovimientoServicio { get; set; }

		public List<Movimiento>? movimientos = new List<Movimiento>();

		[Inject]
		public PagoServicio PagoServicio { get; set; }

		public List<Pago>? pagos = new List<Pago>();

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				isConnected = true;

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

		private async Task ObtenerCuentas()
		{
			cuentas = new List<Cuenta>();

			RespuestaConsumidor<RespuestaAPI<Cuenta>> respuesta = await CuentaServicio.ConsultarCuenta();

			GestionarRespuesta<Cuenta>(respuesta);

			if (!OcurrioError)
			{
				cuentas.Add(respuesta.Data.Datos);
			}
			else
			{
				await MostrarModalError();
			}
		}

		private async Task<GridDataProviderResult<Cuenta>> CuentasDataProvider(GridDataProviderRequest<Cuenta> request)
		{
			if (cuentas.Count == 0)
			{
				await ObtenerCuentas();
				await VerificarError();
			}

			return await Task.FromResult(request.ApplyTo(cuentas));
		}


		private async Task ObtenerMovimientos()
		{
			movimientos = new List<Movimiento>();

			RespuestaConsumidor<RespuestaAPI<IEnumerable<Movimiento>>> respuesta = await MovimientoServicio.ConsultarMovimientos();

			GestionarRespuesta<IEnumerable<Movimiento>>(respuesta);

			if (!OcurrioError)
			{
				movimientos = respuesta.Data.Datos.ToList();
			}
			else
			{
				await MostrarModalError();
			}
		}

		private async Task<GridDataProviderResult<Movimiento>> MovimientosDataProvider(GridDataProviderRequest<Movimiento> request)
		{
			if (cuentas.Count == 0)
			{
				await ObtenerMovimientos();
				await VerificarError();
			}

			return await Task.FromResult(request.ApplyTo(movimientos));
		}

		private async Task ObtenerPagos()
		{
			pagos = new List<Pago>();

			RespuestaConsumidor<RespuestaAPI<IEnumerable<Pago>>> respuesta = await PagoServicio.ConsultarPagos();

			GestionarRespuesta<IEnumerable<Pago>>(respuesta);

			if (!OcurrioError)
			{
				pagos = respuesta.Data.Datos.ToList();
			}
			else
			{
				await MostrarModalError();
			}
		}

		private async Task<GridDataProviderResult<Pago>> PagosDataProvider(GridDataProviderRequest<Pago> request)
		{
			if (pagos.Count == 0)
			{
				await ObtenerPagos();
				await VerificarError();
			}

			return await Task.FromResult(request.ApplyTo(pagos));
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
