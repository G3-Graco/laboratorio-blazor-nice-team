using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace APP.Components.Pages.PaginaPrincipalLoggeada
{
    public partial class PagPrincipal : ComponentBase
    {
        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public CuentaServicio CuentaServicio { get; set; }

        private Modal modal = default!;
        public string mensajeerror = "";
        public string NombreCliente { get; set; } //bienvenido async

        public List<Cuenta>? cuentas;


        protected override async Task OnInitializedAsync()
        {
            cuentas = new List<Cuenta>();


            RespuestaConsumidor<RespuestaAPI<Cuenta>> respuesta = await CuentaServicio.ConsultarCuenta();

            if (respuesta.Ok)
            {
                cuentas.Add(respuesta.Data.Datos);
            }
            else
            {
                mensajeerror = respuesta.Mensaje;
             
            }
        }


            private async Task<GridDataProviderResult<Cuenta>> CuentasDataProvider(GridDataProviderRequest<Cuenta> request)
            {
                //if (cuentas is null) // pull employees only one time for client-side filtering, sorting, and paging
                //    cuentas = ObtenerCuentas(); // call a service or an API to pull the employees

                return await Task.FromResult(request.ApplyTo(cuentas));
            }

        //private async IEnumerable<Cuenta> ObtenerCuentas()
        //{
        //    RespuestaConsumidor<RespuestaAPI<Cuenta>> respuesta = await CuentaServicio.ConsultarCuenta();

        //    if (respuesta.Ok)
        //    {
        //        if (respuesta.Data.Ok)
        //        {
        //            return respuesta.Data;
        //        }
        //        else
        //        {

        //            await modal.ShowAsync<string>("Error", respuesta.Data.Mensaje);
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        await modal.ShowAsync<string>("Error", respuesta.Mensaje);
        //        return null;
        //    }
        //}

        public async Task CerrarModal()
        {
            await modal.HideAsync();
        }

    }
}
