using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

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

                //await ObtenerCuentas();
                //await VerificarError();

                //await ObtenerMovimientos();
                //await VerificarError();

                //await ObtenerPagos();
                //await VerificarError();

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

        private async Task ObtenerCuentas()
        {
            cuentas = new List<Cuenta>();

            RespuestaConsumidor<RespuestaAPI<Cuenta>> respuesta = await CuentaServicio.ConsultarCuenta();

            GestionarRespuesta<Cuenta>(respuesta);

            if (!OcurrioError)
            {
                cuentas.Add(respuesta.Data.Datos);
            }
        }

        private async Task<GridDataProviderResult<Cuenta>> CuentasDataProvider(GridDataProviderRequest<Cuenta> request)
        {
            if (cuentas.Count == 0)
            {
                ObtenerCuentas();
                VerificarError();
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
        }

        private async Task<GridDataProviderResult<Movimiento>> MovimientosDataProvider(GridDataProviderRequest<Movimiento> request)
        {
            if (cuentas.Count == 0)
            {
                ObtenerMovimientos();
                VerificarError();
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
        }

        private async Task<GridDataProviderResult<Pago>> PagosDataProvider(GridDataProviderRequest<Pago> request)
        {
            if (pagos.Count == 0)
            {
                ObtenerPagos();
                VerificarError();
            }

            return await Task.FromResult(request.ApplyTo(pagos));
        }

    }
}
