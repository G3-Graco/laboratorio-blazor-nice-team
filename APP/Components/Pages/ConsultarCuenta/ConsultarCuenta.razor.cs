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

        public List<Cuenta>? cuentas;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                isConnected = true;

                await ObtenerNombreCliente();
                await VerificarError();

                await ObtenerCuentas();
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



    }
}
