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

        public async Task<RespuestaConsumidor<RespuestaAPI<Documento>>> SubirArchivo(FormFile archivo) {
            RespuestaConsumidor<RespuestaAPI<Documento>> respuesta = new();
            try
            {
                respuesta = await Consumidor.Execute<FormFile, RespuestaAPI<Documento>>(
                    "https://localhost:7181/api/Documento/CargarArchivo", 
                    MethodHttp.POST, 
                    archivo
                );
            }
            catch (Exception e)
            {
                
            }
            return respuesta;
        }
    }
}
