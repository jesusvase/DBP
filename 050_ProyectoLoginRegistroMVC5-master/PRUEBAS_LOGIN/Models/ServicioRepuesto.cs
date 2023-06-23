using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PRUEBAS_LOGIN.Models
{
    public class ServicioRespuesto
    {
        public int Id { get; set; }
        public decimal Precio { get; set; }
        public int IdUsuario { get; set; }
        public int IdVehiculo { get; set; }
        public string Descripcion { get; set; }
        public int IdTipoRepuesto { get; set; }

        public  Usuario Usuario { get; set; }
        public  Vehiculo Vehiculo { get; set; }
        public  TipoRepuesto TipoRepuesto { get; set; }
    }



}