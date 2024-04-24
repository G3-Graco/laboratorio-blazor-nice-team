using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace APP.Components.Pages.PagarPrestamoListar
{
    public partial class PagarPrestamoListar : ComponentBase
    {
        

        [Inject]
        public NavigationManager Navigation { get; set; }


        private bool isConnected;
        private Modal modal = default!;
        public string ModalTitulo = "";
        public string ModalMensaje = "";
        public bool OcurrioError = false;

        [Inject]
        public ClienteServicio ClienteServicio { get; set; }
        public string NombreCliente = "";


        [Inject]
        public CuotaServicio CuotaServicio { get; set; }
        public List<Cuota>? cuotas = new List<Cuota>();


        private HashSet<Cuota> cuotaSeleccionada = new();




        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                isConnected = true;

                //await ObtenerPrestamos();
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

        private async Task ObtenerCuotas()
        {
            cuotas = new List<Cuota>();

            RespuestaConsumidor<RespuestaAPI<IEnumerable<Cuota>>> respuesta = await CuotaServicio.ConsultarCuotasPagables();

            GestionarRespuesta<IEnumerable<Cuota>>(respuesta);

            if (!OcurrioError)
            {
                cuotas = respuesta.Data.Datos.ToList();
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
        private async Task ObtenerNombreCliente()
        {
            RespuestaConsumidor<RespuestaAPI<Cliente>> respuesta = await ClienteServicio.ConsultarCliente();

            GestionarRespuesta<Cliente>(respuesta);

            if (!OcurrioError)
            {
                NombreCliente = $"{respuesta.Data.Datos.Nombre} {respuesta.Data.Datos.Apellido}";
            }
        }

        private async Task PagarCuotaSeleccionada()
        {
            IEnumerable<Cuota> cuotaSeleccionado = cuotas is not null && cuotas.Any() ? cuotas : new();

            List<Cuota> cuotasListaSeleccionada = cuotaSeleccionado.ToList();

            if (cuotasListaSeleccionada.Count == 0)
            {
                ModalTitulo = "Error";
                ModalMensaje = "Primero debe consultar un préstamo";
                await modal.ShowAsync();
            }
            else
            {
                Navigation.NavigateTo($"/pagos/{cuotasListaSeleccionada[0].Id}", forceLoad: true);
            }
            
        }



    }
}
