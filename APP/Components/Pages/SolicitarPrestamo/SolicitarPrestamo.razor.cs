using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Tewr.Blazor.FileReader;

namespace APP.Components.Pages.SolicitarPrestamo
{
    public partial class SolicitarPrestamo : ComponentBase
    {
        private bool isConnected;

        [Inject]
        public ClienteServicio ClienteServicio { get; set; }
        public string NombreCliente = "";

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public PrestamoServicio prestamoServicio { get; set; }

        [Inject]
        public CuentaServicio cuentaServicio { get; set; }

        [Inject]
        public ClienteServicio clienteServicio { get; set; }

        [Inject]
        public DocumentoServicio documentoServicio { get; set; }
        [Inject]
        public IFileReaderService fileReader { get; set; }

        private bool spinnerVisible = false;
        private void EsconderSpinner() => spinnerVisible = false;
        private void MostrarSpinner() => spinnerVisible = true;

        private double Sueldo { get; set; }

        private int Empleo { get; set; }

        public string problema { get; set; }

        private Stream Identidad { get; set; }

        private Stream Trabajo { get; set; }

        private ElementReference IdentidadFile { get; set; }
        private string IdentidadFileNombre { get; set; }

        private ElementReference TrabajoFile { get; set; }
        private string TrabajoFileNombre { get; set; }

        private IWebHostEnvironment Environment;

        public Prestamo prestamo = new Prestamo();

        public string ModalTitulo = "";
        public string ModalMensaje = "";
        public bool OcurrioError = false;
        private Modal modal = default!;

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

        protected override async Task OnInitializedAsync()
        {
            prestamo.Fecha = DateTime.Now;
            Sueldo = 0;
            Empleo = 0;
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

        public async void Solicitar()
        {
            try
            {
                MostrarSpinner();

                if (string.IsNullOrEmpty(TrabajoFileNombre) || string.IsNullOrEmpty(IdentidadFileNombre))
                {
                    ModalTitulo = "Error";
                    ModalMensaje = "Tienes que subir ambos archivos solicitados para solicitar el préstamo";
                    await MostrarModalError();
                    return;
                }

                var cliente = await clienteServicio.ConsultarCliente();
                GestionarRespuesta<Cliente>(cliente);

                if (OcurrioError)
                {
                    await MostrarModalError();
                    return;
                }



                var plazos = await prestamoServicio.ConsultarPlazos();
                GestionarRespuesta<IEnumerable<Plazo>>(plazos);

                if (OcurrioError)
                {
                    await MostrarModalError();
                    return;
                }



                long.TryParse(cliente.Data.Datos.FechaNacimiento, out long numero);
                var edad = new DateTime(numero);
                if (Empleo < 3)
                {
                    ModalTitulo = "No se pudo solicitar el préstamo";
                    ModalMensaje = "Se requiere tener un trabajo en el que se haya trabajado al menos 3 meses";
                    await MostrarModalError();
                    return;
                }
                if (prestamo.MontoTotal > (Sueldo * 3))
                {
                    ModalTitulo = "No se pudo solicitar el préstamo";
                    ModalMensaje = "El monto no puede ser menor al triple del sueldo";
                    await MostrarModalError();
                    return;
                }
                if ((DateTime.Now.Year - edad.Year) < 18)
                {
                    ModalTitulo = "No se pudo solicitar el préstamo";
                    ModalMensaje = "No puede hacer préstamos si es menor de eddad";
                    await MostrarModalError();
                    return;
                }
                plazos.Data.Datos.ToList().ForEach(x =>
                {
                    if (prestamo.NumeroCuotas <= x.MaximaCuotas &&
                        prestamo.NumeroCuotas >= x.MinimoCuotas)
                    {
                        prestamo.IdPlazo = x.Id;
                    }
                });
                problema = "después de plazos";
                var estados = await prestamoServicio.ObtenerEstados();
                estados.Data.Datos.ToList().ForEach(x =>
                {
                    if (x.Nombre == "En proceso") prestamo.IdEstado = x.Id;
                });
                var modeloPresta = new ModeloPrestamo()
                {
                    SueldoBasicoDelSolicitante = Sueldo,
                    NumeroCuotasDeseadas = prestamo.NumeroCuotas,
                    MontoTotalDeseado = prestamo.MontoTotal,
                    IdClienteSolicitante = prestamo.IdCliente
                };
                var respuesta = await prestamoServicio.CrearPrestamo(modeloPresta);
                if (respuesta.Ok)
                {
                    if (respuesta.Data.Ok)
                    {
                        string[] listaI = IdentidadFileNombre.Split('.');
                        listaI[^2] = listaI[^2] + $"-identidad-{respuesta.Data.Datos.Id}";
                        IdentidadFileNombre = "";
                        listaI.ToList().ForEach(x =>
                        {
                            if (listaI.ToList().IndexOf(x) != listaI.Length - 1) IdentidadFileNombre = IdentidadFileNombre + x + '.';
                            else IdentidadFileNombre += x;
                        });
                        var respuestaIdentidad = await documentoServicio.SubirIdentidad(Identidad, IdentidadFileNombre);
                        string[] listaT = TrabajoFileNombre.Split('.');
                        listaT[^2] = listaT[^2] + $"-trabajo-{respuesta.Data.Datos.Id}";
                        TrabajoFileNombre = "";
                        listaT.ToList().ForEach(x =>
                        {
                            if (listaT.ToList().IndexOf(x) != listaT.Length - 1) TrabajoFileNombre = TrabajoFileNombre + x + '.';
                            else TrabajoFileNombre += x;
                        });

                        var respuestaTrabajo = await documentoServicio.SubirTrabajo(Trabajo, TrabajoFileNombre);

                        EsconderSpinner();

                        ModalTitulo = "Solicitud de préstamo exitósa";
                        ModalMensaje = "Felicidades el préstamo fue solicitado exitósamente";
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
                        return;
                    }
                    else
                    {
                        ModalTitulo = "Error";
                        ModalMensaje = respuesta.Data.Mensaje;
                        await MostrarModalError();
                    }
                }
                else
                {
                    ModalTitulo = "Error";
                    ModalMensaje = respuesta.Mensaje;

                    await MostrarModalError();

                }
            }
            catch (Exception e)
            {
                ModalTitulo = "No se pudo solicitar el préstamo";
                ModalMensaje = e.Message;
                await MostrarModalError();
            }

        }

        public async Task CargarIdentidad()
        {
            try
            {
                var archivo = (await fileReader.CreateReference(IdentidadFile).EnumerateFilesAsync()).FirstOrDefault();

                if (archivo == null)
                {
                    problema = "No tiene un archivo";
                    return;
                }

                var informacion = await archivo.ReadFileInfoAsync();
                IdentidadFileNombre = informacion.Name;

                using (var memoryStream = await archivo.CreateMemoryStreamAsync((int)informacion.Size))
                {
                    Identidad = new MemoryStream(memoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                problema = ex.Message;
            }

        }

        public async Task CargarTrabajo()
        {
            try
            {
                var archivo = (await fileReader.CreateReference(TrabajoFile).EnumerateFilesAsync()).FirstOrDefault();

                if (archivo == null)
                {
                    problema = "No tiene un archivo";
                    return;
                }

                var informacion = await archivo.ReadFileInfoAsync();
                TrabajoFileNombre = informacion.Name;

                using (var memoryStream = await archivo.CreateMemoryStreamAsync((int)informacion.Size))
                {
                    Trabajo = new MemoryStream(memoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                problema = ex.Message;
            }

        }

        public async Task CerrarModal()
        {
            await modal.HideAsync();
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

    }


}