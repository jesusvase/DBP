using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PRUEBAS_LOGIN.Models
{
    public class Ventas
    {
        public int ID { get; set; }
        public int ClienteID { get; set; }
        public int VehiculoID { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Cantidad { get; set; }

        public int IdUsuario { get; set; }

        public Usuario Cliente { get; set; }

        public Usuario Cliente2 { get; set; }
        public Vehiculo Vehiculo { get; set; }
    }

}