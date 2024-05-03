using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace APP.Components.Pages.PagarPrestamo
{
    public partial class PagarPrestamo : ComponentBase
    {
        [Parameter]
		public int idcuota { get; set; }

        private bool isConnected;
        private Modal modal = default!;
        public string ModalTitulo = "";
        public string ModalMensaje = "";
        public bool OcurrioError = false;

        [Inject]
        public ClienteServicio ClienteServicio { get; set; }
        public string NombreCliente = "";
		[Inject]
		public NavigationManager Navigation { get; set; }


		[Inject]
        public PagoServicio PagoServicio { get; set; }
		public ModeloPagarCuota ModeloPagarCuota = new ModeloPagarCuota();

		[Inject]
		public CuentaServicio CuentaServicio { get; set; }


		[Inject]
		public CuotaServicio CuotaServicio { get; set; }


		protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                isConnected = true;

                await ObtenerPagoDatos();

                await ObtenerNombreCliente();
                await VerificarError();

                StateHasChanged();
            }
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
		private async Task ObtenerPagoDatos()
		{
			RespuestaConsumidor<RespuestaAPI<Cuota>> respuestaCuota = await CuotaServicio.ConsultarCuota(idcuota);
			GestionarRespuesta<Cuota>(respuestaCuota);

			if (OcurrioError)
			{
				await MostrarModalError();
			}

			RespuestaConsumidor<RespuestaAPI<Cuenta>> respuestaCuenta = await CuentaServicio.ConsultarCuenta();
			GestionarRespuesta<Cuenta>(respuestaCuenta);


			if (!OcurrioError)
			{
				ModeloPagarCuota.CuentaIdentificador = respuestaCuenta.Data.Datos.Identificador.ToString();
				ModeloPagarCuota.CuotaId = respuestaCuota.Data.Datos.Id.ToString();
                ModeloPagarCuota.MontoTotal = respuestaCuota.Data.Datos.Pago.ToString();
                ModeloPagarCuota.PrestamoId = respuestaCuota.Data.Datos.IdPrestamo.ToString();
			}
			else
			{
				await MostrarModalError();
			}

		}
		private async Task Pagar()
		{
            Pago pago = new Pago
            {
                Id = 0,
                CuentaIdentificador = Int64.Parse(ModeloPagarCuota.CuentaIdentificador),
                CuotaId = int.Parse(ModeloPagarCuota.CuotaId)
			};

			RespuestaConsumidor<RespuestaAPI<Pago>> respuesta = await PagoServicio.PagarCuota(pago);

			GestionarRespuesta<Pago>(respuesta);

			if (!OcurrioError)
			{
				ModalTitulo = "Éxito";
				ModalMensaje = "Pago realizado exitosamente";

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



	}
}
