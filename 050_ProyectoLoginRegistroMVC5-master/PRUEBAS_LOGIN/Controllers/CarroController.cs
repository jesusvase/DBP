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
using iTextSharp.text;
using iTextSharp.text.pdf;

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


        // Reporte //



        public ActionResult GenerarReportePDF()
        {
            List<Inventario> inventario = ObtenerInventario();

            // Crear el documento PDF
            Document document = new Document();
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            // Crear una tabla en el documento PDF
            PdfPTable table = new PdfPTable(4); // 4 columnas en tu tabla
            table.WidthPercentage = 100; // Ancho de la tabla al 100% del documento

            // Agregar las cabeceras de las columnas
            table.AddCell("Nombre del Vehículo");
            table.AddCell("Tipo de Vehículo");
            table.AddCell("Precio");
            table.AddCell("Cantidad Disponible");

            // Agregar los datos de la tabla
            foreach (var item in inventario)
            {
                table.AddCell(item.Vehiculo.Nombre);
                table.AddCell(item.Vehiculo.TipoVehiculo.Nombre);
                table.AddCell(item.Vehiculo.Precio.ToString());
                table.AddCell(item.CantidadDisponible.ToString());
            }

            // Agregar la tabla al documento
            document.Add(table);

            // Cerrar el documento
            document.Close();

            // Descargar el archivo PDF generado
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Reporte.pdf");
            Response.BinaryWrite(memoryStream.ToArray());
            Response.End();

            return null;
        }

        /////////////////////////////////VENTAS/////////////////////////

        [HttpPost]
        public ActionResult AgregarVenta(Ventas venta)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                int idUsuario = (int)Session["IdUsuario"];
                SqlCommand cmd = new SqlCommand("AgregarVenta", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClienteID", venta.ClienteID);
                cmd.Parameters.AddWithValue("@VehiculoID", venta.VehiculoID);
                cmd.Parameters.AddWithValue("@Cantidad", venta.Cantidad);
                cmd.Parameters.AddWithValue("@FechaVenta", venta.FechaVenta);
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
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


        public List<Ventas> ObtenerVentas()
        {
            List<Ventas> ventas = new List<Ventas>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand command = new SqlCommand("ObtenerVentas", cn);
                command.CommandType = CommandType.StoredProcedure;

                SqlDataReader sqlDataReader = command.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    ventas.Add(new Ventas
                    {
                        Cliente2 = new Usuario
                        {
                            Correo = sqlDataReader["NombreUsuario"].ToString()
                        },
                        Cliente = new Usuario
                        {
                            Correo = sqlDataReader["NombreCliente"].ToString()
                        },
                        Vehiculo = new Vehiculo
                        {
                            Nombre = sqlDataReader["NombreVehiculo"].ToString(),
                            Precio = sqlDataReader.GetDecimal(sqlDataReader.GetOrdinal("PrecioVenta"))
                        },

                        //FechaVenta = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("FechaVenta")),


                        //FechaVenta = Convert.ToDateTime(sqlDataReader["FechaVenta"]).ToString("dd/MM/yyyy"),

                        FechaVenta = Convert.ToDateTime(sqlDataReader["FechaVenta"]),
                        Cantidad = Convert.ToInt32(sqlDataReader["Cantidad"])

                        
                    });
                }

                cn.Close();
            }

            return ventas;
        }


        public ActionResult GetVentas()
        {
            List<Ventas> ventas = ObtenerVentas();

            return Json(ventas, JsonRequestBehavior.AllowGet);
        }

        public List<Ventas> GetVentasPorCliente()
        {
            List<Ventas> ventas = new List<Ventas>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                int idUsuario = (int)Session["IdUsuario"];
                SqlCommand command = new SqlCommand("ObtenerComprasPorCliente", cn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ClienteID", idUsuario);

                SqlDataReader sqlDataReader = command.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    ventas.Add(new Ventas
                    {
                        Cliente = new Usuario
                        {
                            Correo = sqlDataReader["NombreCliente"].ToString()
                        },
                        Vehiculo = new Vehiculo
                        {
                            Nombre = sqlDataReader["NombreVehiculo"].ToString(),
                            Precio = sqlDataReader.GetDecimal(sqlDataReader.GetOrdinal("PrecioVenta"))
                        },

                        FechaVenta = Convert.ToDateTime(sqlDataReader["FechaVenta"]),
                        Cantidad = Convert.ToInt32(sqlDataReader["Cantidad"])


                    });
                }

                cn.Close();
            }

            return ventas;
        }

        public ActionResult GetVentasCliente()
        {
            List<Ventas> ventas = GetVentasPorCliente();

            return Json(ventas, JsonRequestBehavior.AllowGet);
        }

    }
}



