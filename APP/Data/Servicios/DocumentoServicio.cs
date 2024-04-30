using APP.Data.Modelos;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json.Linq;

namespace APP.Data.Servicios
{
    public class DocumentoServicio
    {
        public readonly ProtectedLocalStorage _protectedLocalStorage;
        public DocumentoServicio(ProtectedLocalStorage protectedLocalStorage)
        {
            _protectedLocalStorage = protectedLocalStorage;
        }

        public async Task<RespuestaConsumidor<RespuestaAPI<Documento>>> SubirArchivo(Stream archivo, string nombre) {
            RespuestaConsumidor<RespuestaAPI<Documento>> respuesta = new();
            try
            {
                // respuesta = await Consumidor.Execute<FormFile, RespuestaAPI<Documento>>(
                //     "https://localhost:7181/api/Documento/CargarArchivo", 
                //     MethodHttp.POST, 
                //     archivo
                // );
                string url = "https://localhost:7181/api/Documento/CargarArchivo";

                var content = new MultipartFormDataContent();
                content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data");

                content.Add(new StreamContent(archivo, (int)archivo.Length), "image", nombre);
                HttpClient httpClient = new HttpClient();

                var response = httpClient.PostAsync(url, content);
                }
            catch (Exception e)
            {
                
            }
            return respuesta;
        }

        public async Task<RespuestaConsumidor<RespuestaAPI<Documento>>> AgregarArchivo(Documento documento) {
            RespuestaConsumidor<RespuestaAPI<Documento>> respuesta = new();
            try
            {
                respuesta = await Consumidor.Execute<Documento, RespuestaAPI<Documento>>(
                    "https://localhost:7181/api/Documento", 
                    MethodHttp.POST, 
                    documento
                );
            }
            catch (Exception e)
            {
            }
            return respuesta;
        }

        public async Task<RespuestaConsumidor<RespuestaAPI<IEnumerable<TipoDocumento>>>> ObtenerTipos() {
            RespuestaConsumidor<RespuestaAPI<IEnumerable<TipoDocumento>>> respuesta = new();
            try
            {
                respuesta = await Consumidor.Execute<TipoDocumento, RespuestaAPI<IEnumerable<TipoDocumento>>>(
                    "https://localhost:7181/api/TipoDocumento", 
                    MethodHttp.GET, 
                    null
                );
            }
            catch (Exception e)
            {
            }
            return respuesta;
        }
    }
}
