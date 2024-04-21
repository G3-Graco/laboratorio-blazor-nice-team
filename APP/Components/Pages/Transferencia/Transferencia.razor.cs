using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APP.Data.Modelos;
using APP.Data.Servicios;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace APP.Components.Pages.Transferencia
{
    public partial class Transferencia : ComponentBase
    {
        [Inject]
		public NavigationManager Navigation { get; set; }

		[Inject]
		public MovimientoServicio movimientoServicio { get; set; }

        [Inject]
        public CuentaServicio cuentaServicio { get; set; }

        private double Saldo { get; set; }
        
        private long CuentaNumero {get; set;}

		public Movimiento movimiento = new Movimiento();

		private Modal modal = default!;

        private string Cambio {get; set;}

		protected override async Task OnInitializedAsync()
        {
            var cuenta = await cuentaServicio.ConsultarCuenta();
            var saldo = cuenta.Data.Datos.Saldo; 
            Saldo = saldo;
            CuentaNumero = cuenta.Data.Datos.Identificador;
            movimiento.Fecha = DateTime.Now;
        }
        public async void Transferir()
        {
			movimiento.CuentaOrigenIdentificador = CuentaNumero;
            var tipos = await movimientoServicio.ObtenerTipos();
            tipos.Data.Datos.ToList().ForEach(x => {
                if (x.Nombre == "Transferencia") {
                    movimiento.TipoMovimientoId = x.Id;
                }
            });
            var respuesta = await movimientoServicio.RealizarMovimiento(movimiento);
            if (respuesta.Ok)
            {
                Cambio  = "¡Felicidades! se hizo una transferencia exitosa";
                var cuenta = await cuentaServicio.ConsultarCuenta();
                CuentaNumero = cuenta.Data.Datos.Identificador;
            } else {
                Cambio  = "¡Oh no! La transferencia no pudo ser realizada";
            }
		}

        public void Borrar() {
            movimiento.Monto = 0;
            movimiento.CuentaReceptoraIdentificador = 0;
            movimiento.Descripcion = "";
        }

		public async Task CerrarModal()
		{
			await modal.HideAsync();
		}
    }
}