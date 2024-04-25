using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace APP.Data.Servicios
{
    public class DocumentoServicio
    {
        public readonly ProtectedLocalStorage _protectedLocalStorage;
        public DocumentoServicio(ProtectedLocalStorage protectedLocalStorage)
        {
            _protectedLocalStorage = protectedLocalStorage;
        }
    }
}
