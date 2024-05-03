using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Reflection;

namespace APP.Components.Pages.PaginaPrincipalLoggeada
{
    public partial class PagPrincipal : ComponentBase
    {
        private bool isConnected;

		[Inject]
		public ClienteServicio ClienteServicio { get; set; }
		[Inject]
        public CuentaServicio CuentaServicio { get; set; }
		[Inject]
		public PrestamoServicio PrestamoServicio { get; set; }

		private Modal modal = default!;
        public string ModalTitulo = "";
        public string ModalMensaje = "";

        public bool OcurrioError = false;

        public string NombreCliente = "";//bienvenido async

        public List<Cuenta>? cuentas = new List<Cuenta>();

        public List<Prestamo>? prestamos = new List<Prestamo>();

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

		private async Task ObtenerPrestamosActivos()
		{
			prestamos = new List<Prestamo>();
			//
			RespuestaConsumidor<RespuestaAPI<IEnumerable<Prestamo>>> respuesta = await PrestamoServicio.ConsultarPrestamos();

			GestionarRespuesta<IEnumerable<Prestamo>>(respuesta);

			if (!OcurrioError)
			{
				prestamos = respuesta.Data.Datos.ToList();
                prestamos = prestamos.Where(p => p.IdEstado != 3).ToList();
			}
            else
            {
                await MostrarModalError();
            }
        }

		private async Task<GridDataProviderResult<Prestamo>> PrestamosDataProvider(GridDataProviderRequest<Prestamo> request)
		{
			if (cuentas.Count == 0)
			{
				await ObtenerPrestamosActivos();
				await VerificarError();
			}

			return await Task.FromResult(request.ApplyTo(prestamos));
		}


	}
}
