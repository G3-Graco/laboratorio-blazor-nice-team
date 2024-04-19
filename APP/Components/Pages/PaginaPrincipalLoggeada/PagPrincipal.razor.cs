using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace APP.Components.Pages.PaginaPrincipalLoggeada
{
    public partial class PagPrincipal : ComponentBase
    {
        private bool isConnected;

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public CuentaServicio CuentaServicio { get; set; }

        private Modal modal = default!;
        public string ModalTitulo = "";
        public string ModalMensaje = "";
        public bool OcurrioError = false;
        public string NombreCliente = "";//bienvenido async

        public List<Cuenta>? cuentas;




        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                isConnected = true;

                await ObtenerCuentas();
                await VerificarError();

                StateHasChanged();
            }
        }


        private async Task<GridDataProviderResult<Cuenta>> CuentasDataProvider(GridDataProviderRequest<Cuenta> request)
            {
                //if (cuentas is null) // pull employees only one time for client-side filtering, sorting, and paging
                //    cuentas = ObtenerCuentas(); // call a service or an API to pull the employees

                return await Task.FromResult(request.ApplyTo(cuentas));
            }

        private async Task ObtenerCuentas()
        {
            cuentas = new List<Cuenta>();

            RespuestaConsumidor<RespuestaAPI<Cuenta>> respuesta = await CuentaServicio.ConsultarCuenta();

            if (respuesta.Ok)
            {
                if (respuesta.Data.Ok)
                {
                    cuentas.Add(respuesta.Data.Datos);
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

    }
}
