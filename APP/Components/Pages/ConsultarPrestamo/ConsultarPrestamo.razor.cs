using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace APP.Components.Pages.ConsultarPrestamo
{
    public partial class ConsultarPrestamo : ComponentBase
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
        public PrestamoServicio PrestamoServicio { get; set; }
        public List<Prestamo>? prestamos = new List<Prestamo>();


        private HashSet<Prestamo> prestamoSeleccionado = new();




        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                isConnected = true;

                await ObtenerPrestamos();
                await VerificarError();

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

        private async Task ObtenerPrestamos()
        {
            prestamos = new List<Prestamo>();

            RespuestaConsumidor<RespuestaAPI<IEnumerable<Prestamo>>> respuesta = await PrestamoServicio.ConsultarPrestamos();

            GestionarRespuesta<IEnumerable<Prestamo>>(respuesta);

            if (!OcurrioError)
            {
                prestamos = respuesta.Data.Datos.ToList();
            }
        }

        private async Task<GridDataProviderResult<Prestamo>> PrestamoDataProvider(GridDataProviderRequest<Prestamo> request)
        {
            if (prestamos is null)
            {
                ObtenerPrestamos();
                VerificarError();
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
        }

        private async Task ConsultarPrestamoSeleccionado()
        {
            IEnumerable<Prestamo> prestamoSeleccionado = prestamos is not null && prestamos.Any() ? prestamos : new();

            List<Prestamo> prestamosListaSeleccionada = prestamoSeleccionado.ToList();

            if (prestamosListaSeleccionada.Count == 0)
            {
                ModalTitulo = "Error";
                ModalMensaje = "Primero debe consultar un préstamo";
                await modal.ShowAsync();
            }
            else
            {
                Navigation.NavigateTo($"/prestamo/{prestamosListaSeleccionada[0].Id}", forceLoad: true);
            }
            
        }



    }
}
