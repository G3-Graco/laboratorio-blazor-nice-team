using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace APP.Data.Servicios
{
    public class MovimientoServicio
    {
        public readonly ProtectedLocalStorage _protectedLocalStorage;
        public MovimientoServicio(ProtectedLocalStorage protectedLocalStorage)
        {
            _protectedLocalStorage = protectedLocalStorage;
        }
    }
}