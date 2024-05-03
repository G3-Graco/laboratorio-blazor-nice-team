using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace APP.Components.Pages.ConsultarPrestamoDetallado
{
    public partial class ConsultarPrestamoDetallado : ComponentBase
    {
        [Parameter]
		public int idprestamo { get; set; }

        private bool isConnected;
        private Modal modal = default!;
        public string ModalTitulo = "";
        public string ModalMensaje = "";
        public bool OcurrioError = false;

        [Inject]
        public ClienteServicio ClienteServicio { get; set; }
        public string NombreCliente = "";

        [Inject]
        public PagoServicio PagoServicio { get; set; }
        public List<Pago>? pagos = new List<Pago>();


        [Inject]
        public PrestamoServicio PrestamoServicio { get; set; }
        public double MontoPendiente = 0;
        public List<Prestamo>? prestamos = new List<Prestamo>();

        [Inject]
        public CuotaServicio CuotaServicio { get; set; }
        public List<Cuota>? cuotas = new List<Cuota>();

        [Inject]
        public DocumentoServicio documentoServicio { get; set; }
        public List<Documento> documentos = new List<Documento>();


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                isConnected = true;

                await ObntenerMontoPendienteTotal();
                //await ObtenerPrestamo();
                //await VerificarError();

                //await ObtenerPagos();
                //await VerificarError();

                //await ObtenerCuotas();
                //await VerificarError();

                await ObtenerNombreCliente();
                await VerificarError();
                await ObtenerDocumentos();

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

        private async Task ObtenerPrestamo()
        {
            prestamos = new List<Prestamo>();

            RespuestaConsumidor<RespuestaAPI<IEnumerable<Prestamo>>> respuesta = await PrestamoServicio.ConsultarPrestamos();

            GestionarRespuesta<IEnumerable<Prestamo>>(respuesta);

            if (!OcurrioError)
            {
                Prestamo prestamoconsultado = respuesta.Data.Datos.FirstOrDefault(p => p.Id == idprestamo);
                prestamos.Add(prestamoconsultado);
            }
            else
            {
                await MostrarModalError();
            }
        }

        private async Task<GridDataProviderResult<Prestamo>> PrestamoDataProvider(GridDataProviderRequest<Prestamo> request)
        {
            if (prestamos.Count == 0)
            {
                await ObtenerPrestamo();
				await VerificarError();
            }

            return await Task.FromResult(request.ApplyTo(prestamos));
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

        private async Task ObntenerMontoPendienteTotal()
        {
            RespuestaConsumidor<RespuestaAPI<double>> respuesta = await PrestamoServicio.ConsultarMontoPendiente(idprestamo);

            GestionarRespuesta<double>(respuesta);

            if (!OcurrioError)
            {
                MontoPendiente = respuesta.Data.Datos;
            }
            else
            {
                await MostrarModalError();
            }
        }

        private async Task ObtenerPagos()
        {
            pagos = new List<Pago>();

            RespuestaConsumidor<RespuestaAPI<IEnumerable<Pago>>> respuesta = await PagoServicio.ConsultarPagosDePrestamo(idprestamo);

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

        private async Task ObtenerCuotas()
        {
            cuotas = new List<Cuota>();

            RespuestaConsumidor<RespuestaAPI<IEnumerable<Cuota>>> respuesta = await CuotaServicio.ConsultarCuotas(idprestamo);

            GestionarRespuesta<IEnumerable<Cuota>>(respuesta);

            if (!OcurrioError)
            {
                cuotas = respuesta.Data.Datos.ToList();
            }
            else
            {
                await MostrarModalError();
            }
        }

        private async Task<GridDataProviderResult<Cuota>> CuotasDataProvider(GridDataProviderRequest<Cuota> request)
        {
            if (cuotas.Count == 0)
            {
				await ObtenerCuotas();
				await VerificarError();
            }

            return await Task.FromResult(request.ApplyTo(cuotas));
        }


        private async Task ObtenerDocumentos() {
            documentos = new List<Documento>();

            RespuestaConsumidor<RespuestaAPI<IEnumerable<Documento>>> respuesta = await documentoServicio.ObtenerDocumentos(idprestamo);

            GestionarRespuesta<IEnumerable<Documento>>(respuesta);

            if (!OcurrioError)
            {
                documentos = respuesta.Data.Datos.ToList();
            }
            else
            {
                await MostrarModalError();
            }
        }

    }
}
