﻿using System;
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
                cmd.Parameters.AddWithValue("@Precio", vehiculo.Precio); // Agregar el parámetro Precio
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

        public JsonResult GetVehiculos()
        {
            List<Vehiculo> vehiculos = new List<Vehiculo>();

            using (SqlConnection connection = new SqlConnection(cadena))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GetVehiculo", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlDataReader sqlDataReader = command.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    vehiculos.Add(new Vehiculo
                    {
                        ID = Convert.ToInt32(sqlDataReader["ID"]),
                        Nombre = sqlDataReader["Nombre"].ToString()
                    });
                }

                connection.Close();
            }

            return Json(vehiculos, JsonRequestBehavior.AllowGet);
        }

        /////////////////////////////////////////////////INVENTARIO/////////////////////////////////////////
        ///
        public List<Inventario> ObtenerInventario()
        {
            List<Inventario> inventario = new List<Inventario>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand command = new SqlCommand("ObtenerInventario", cn);
                command.CommandType = CommandType.StoredProcedure;

                SqlDataReader sqlDataReader = command.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    inventario.Add(new Inventario
                    {
                        Vehiculo = new Vehiculo
                        {
                            Nombre = sqlDataReader["NombreVehiculo"].ToString(),
                            TipoVehiculo = new TipoVehiculo
                            {
                                Nombre = sqlDataReader["NombreTipoVehiculo"].ToString()
                            },
                            Precio = Convert.ToDecimal(sqlDataReader["Precio"]) // Agregar el campo Precio
                        },
                        CantidadDisponible = Convert.ToInt32(sqlDataReader["CantidadDisponible"])
                    });
                }

                cn.Close();
            }

            return inventario;
        }


        public ActionResult GetInventario()
        {
            List<Inventario> inventario = ObtenerInventario();

            return Json(inventario, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RegistrarInventario(Inventario inventario)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("AgregarVehiculosAInventario", cn);
                cmd.Parameters.AddWithValue("@VehiculoID", inventario.VehiculoID);
                cmd.Parameters.AddWithValue("@Cantidad", inventario.CantidadDisponible);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Contact", "Home");
        }

        /////////////////////////////////VENTAS/////////////////////////
        ///

        [HttpPost]
        public ActionResult AgregarVenta(Ventas venta)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("AgregarVenta", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClienteID", venta.ClienteID);
                cmd.Parameters.AddWithValue("@VehiculoID", venta.VehiculoID);
                cmd.Parameters.AddWithValue("@Cantidad", venta.Cantidad);
                cmd.Parameters.AddWithValue("@FechaVenta", venta.FechaVenta);

                try
                {
                    cmd.ExecuteNonQuery();
                    // Venta agregada correctamente
                    return RedirectToAction("Contact", "Home"); // Redirecciona a una página de éxito
                }
                catch (SqlException ex)
                {
                    // Ocurrió un error al agregar la venta
                    ViewBag.ErrorMessage = "Ocurrió un error al agregar la venta: " + ex.Message;
                    return RedirectToAction("Contact", "Home"); // Redirecciona a una página de éxito
                }
            }
        }

        public JsonResult ObtenerInventarioConCantidadMayorCero()
        {
            List<Inventario> inventario = new List<Inventario>();

            using (SqlConnection connection = new SqlConnection(cadena))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("ObtenerInventarioConCantidadMayorCero", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlDataReader sqlDataReader = command.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    inventario.Add(new Inventario
                    {
                        VehiculoID = Convert.ToInt32(sqlDataReader["VehiculoID"]),
                        Vehiculo = new Vehiculo
                        {
                            ID = Convert.ToInt32(sqlDataReader["VehiculoID"]),
                            Nombre = sqlDataReader["NombreVehiculo"].ToString(),
                            TipoVehiculo = new TipoVehiculo
                            {
                                Nombre = sqlDataReader["NombreTipoVehiculo"].ToString()
                            }
                        },
                        CantidadDisponible = Convert.ToInt32(sqlDataReader["CantidadDisponible"])
                    });
                }

                connection.Close();
            }

            return Json(inventario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerCorreoIdUsuario()
        {
            List<Usuario> usuarios = new List<Usuario>();

            using (SqlConnection connection = new SqlConnection(cadena))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("ObtenerCorreoIdUsuario", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlDataReader sqlDataReader = command.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    usuarios.Add(new Usuario
                    {
                        IdUsuario = Convert.ToInt32(sqlDataReader["IdUsuario"]),
                        Correo = sqlDataReader["Correo"].ToString()
                    });
                }

                connection.Close();
            }

            return Json(usuarios, JsonRequestBehavior.AllowGet);
        }


    }
}


