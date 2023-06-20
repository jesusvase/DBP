using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PRUEBAS_LOGIN.Models
{
    public class Vehiculo
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public int TipoVehiculoID { get; set; }

        public TipoVehiculo TipoVehiculo { get; set; }

    }
}