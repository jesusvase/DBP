using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MN_WEB.Entities
{
    public class UsuarioEnt
    {
        public string CorreoElectronico { get; set; }
        public string Contrasenna { get; set; }
        public string Identificacion { get; set; }
        public string Nombre { get; set; }
        public string ConfirmarContrasenna { get; set; }
        public bool Estado { get; set; }
        public byte IdRol { get; set; }
    }
}