using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PRUEBAS_LOGIN.Models
{
    public class Inventario
    {
        public int ID { get; set; }
        public int VehiculoID { get; set; }
        public int CantidadDisponible { get; set; }

        public Vehiculo Vehiculo { get; set; }
        
    }
}