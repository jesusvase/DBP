using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

using PRUEBAS_LOGIN.Models;

using System.Data.SqlClient;
using System.Data;
using System.IO;

using System.Drawing;
using System.Drawing.Imaging;

namespace PRUEBAS_LOGIN.Controllers
{
    public class CarroController : Controller
    {

        static string cadena = "Data Source=(local);Initial Catalog=LABORATORIO3;Integrated Security=true";

        public ActionResult Registrar()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Registrar(Vehiculo vehiculo)
        {

            using (SqlConnection cn = new SqlConnection(cadena))
            {

                SqlCommand cmd = new SqlCommand("InsertarVehiculo", cn);
                cmd.Parameters.AddWithValue("@Nombre", vehiculo.Nombre);
                cmd.Parameters.AddWithValue("@TipoVehiculoID", vehiculo.TipoVehiculoID);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();




            }
            return RedirectToAction("Contact", "Home");
        }

        public JsonResult Get()
        {
            List<TipoVehiculo> tipoVehiculos = new List<TipoVehiculo>();

            using (SqlConnection connection = new SqlConnection(cadena))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GetTipoVehiculo", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlDataReader sqlDataReader = command.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    tipoVehiculos.Add(new TipoVehiculo
                    {
                        ID = Convert.ToInt32(sqlDataReader["ID"]),
                        Nombre = sqlDataReader["Nombre"].ToString()
                    });
                }

                connection.Close();
            }

            return Json(tipoVehiculos, JsonRequestBehavior.AllowGet);
        }

        public List<Vehiculo> ObtenerVehiculosConTipos()
        {
            
            List<Vehiculo> vehiculos = new List<Vehiculo>();

            using (SqlConnection connection = new SqlConnection(cadena))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("ObtenerVehiculosConTipos", connection);
                command.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Vehiculo vehiculo = new Vehiculo();
                    vehiculo.ID = (int)reader["ID"];
                    vehiculo.Nombre = (string)reader["Nombre"];
                    vehiculo.TipoVehiculoID = (int)reader["TipoVehiculoID"];

                    vehiculo.TipoVehiculo = new TipoVehiculo();
                    vehiculo.TipoVehiculo.ID = (int)reader["TipoVehiculoID"];
                    vehiculo.TipoVehiculo.Nombre = (string)reader["TipoVehiculoNombre"];

                    vehiculos.Add(vehiculo);
                }

                reader.Close();
            }

            return vehiculos;
        }
    }
}



