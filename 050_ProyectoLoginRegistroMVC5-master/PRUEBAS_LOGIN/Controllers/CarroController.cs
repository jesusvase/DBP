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
                        ID= Convert.ToInt32(sqlDataReader["ID"]),
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


        // Reporte2 // 

        public ActionResult GenerarReportePDF2()
        {
            List<Ventas> ventas = ObtenerVentas();

            // Crear el documento PDF
            Document document = new Document();
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            // Crear una tabla en el documento PDF
            PdfPTable table = new PdfPTable(5); // 5 columnas en tu tabla
            table.WidthPercentage = 100; // Ancho de la tabla al 100% del documento

            // Agregar las cabeceras de las columnas
            table.AddCell("Cliente");
            table.AddCell("Vehículo");
            table.AddCell("Fecha de Venta");
            table.AddCell("Precio de Venta");
            table.AddCell("Cantidad");

            // Agregar los datos de la tabla
            foreach (var venta in ventas)
            {
                table.AddCell(venta.Cliente.Correo);
                table.AddCell(venta.Vehiculo.Nombre);
                table.AddCell(venta.FechaVenta.ToString("dd/MM/yyyy"));
                table.AddCell(venta.Vehiculo.Precio.ToString());
                table.AddCell(venta.Cantidad.ToString());
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
                        ID = Convert.ToInt32(sqlDataReader["ID"]),
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

        //////////////////////////////////MANTENIMIENTO//////////////////
        ///
        public List<ServicioMantenimiento> GetServiciosMantenimiento()
        {
            List<ServicioMantenimiento> servicios = new List<ServicioMantenimiento>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand command = new SqlCommand("MostrarServicioMantenimiento", cn);
                command.CommandType = CommandType.StoredProcedure;

                SqlDataReader sqlDataReader = command.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    servicios.Add(new ServicioMantenimiento
                    {
                        Id = Convert.ToInt32(sqlDataReader["Id"]),
                        Precio = sqlDataReader.GetDecimal(sqlDataReader.GetOrdinal("Precio")),
                        Descripcion = sqlDataReader["Descripcion"].ToString(),

                        // Obtener el nombre de usuario
                        Usuario = new Usuario
                        {
                            Correo = sqlDataReader["Usuario"].ToString() // Utilizar el nombre de columna correcto: "Usuario"
                        },

                        // Obtener el nombre del vehículo
                        Vehiculo = new Vehiculo
                        {
                            Nombre = sqlDataReader["Vehiculo"].ToString() // Utilizar el nombre de columna correcto: "Vehiculo"
                        },

                        // Obtener el nombre del tipo de mantenimiento
                        TipoMantenimiento = new TipoMantenimiento
                        {
                            Nombre = sqlDataReader["TipoMantenimiento"].ToString() // Utilizar el nombre de columna correcto: "TipoMantenimiento"
                        }
                    });
                }

                cn.Close();
            }

            return servicios;
        }


        public ActionResult GetServicioMantenimiento()
        {
            List<ServicioMantenimiento> servicios = GetServiciosMantenimiento();

            return Json(servicios, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AgregarServicioMantenimiento(ServicioMantenimiento servicio)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                int idUsuario = (int)Session["IdUsuario"];
                SqlCommand cmd = new SqlCommand("AgregarServicioMantenimiento", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@IdVehiculo", servicio.IdVehiculo);
                cmd.Parameters.AddWithValue("@IdTipoMantenimiento", servicio.IdTipoMantenimiento);
                cmd.Parameters.AddWithValue("@Descripcion", servicio.Descripcion);
                try
                {
                    cmd.ExecuteNonQuery();
                    // Servicio de mantenimiento agregado correctamente
                    return RedirectToAction("Contact", "Home"); // Redirecciona a una página de éxito
                }
                catch (SqlException ex)
                {
                    // Ocurrió un error al agregar el servicio de mantenimiento
                    ViewBag.ErrorMessage = "Ocurrió un error al agregar el servicio de mantenimiento: " + ex.Message;
                    return RedirectToAction("Contact", "Home"); // Redirecciona a una página de éxito
                }
            }
        }

        public JsonResult GetTiposMantenimiento()
        {
            List<TipoMantenimiento> tiposMantenimiento = new List<TipoMantenimiento>();

            using (SqlConnection connection = new SqlConnection(cadena))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("ObtenerTiposMantenimiento", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlDataReader sqlDataReader = command.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    tiposMantenimiento.Add(new TipoMantenimiento
                    {
                        Id = Convert.ToInt32(sqlDataReader["Id"]),
                        Nombre = sqlDataReader["Nombre"].ToString()
                    });
                }

                connection.Close();
            }

            return Json(tiposMantenimiento, JsonRequestBehavior.AllowGet);
        }

        //////// Servivio Repuesto ///////////

        // Get // 


        public List<ServicioRespuesto> GetServiciosRepuesto()
        {
            List<ServicioRespuesto> servicios = new List<ServicioRespuesto>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand command = new SqlCommand("ObtenerServicioRepuestos", cn);
                command.CommandType = CommandType.StoredProcedure;

                SqlDataReader sqlDataReader = command.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    ServicioRespuesto servicio = new ServicioRespuesto();
                    servicio.Id = Convert.ToInt32(sqlDataReader["Id"]);
                    servicio.Precio = sqlDataReader.GetDecimal(sqlDataReader.GetOrdinal("Precio"));
                    servicio.Descripcion = sqlDataReader["Descripcion"].ToString();

                    Usuario usuario = new Usuario();
                    usuario.Correo = sqlDataReader["NombreUsuario"].ToString();
                    servicio.Usuario = usuario;

                    usuario.IdUsuario = Convert.ToInt32(sqlDataReader["IdUsuario"]);
                    servicio.Usuario = usuario;

                    Vehiculo vehiculo = new Vehiculo();
                    vehiculo.Nombre = sqlDataReader["NombreVehiculo"].ToString();
                    servicio.Vehiculo = vehiculo;

                    TipoRepuesto tipoRepuesto = new TipoRepuesto();
                    tipoRepuesto.Nombre = sqlDataReader["TipoRepuesto"].ToString();
                    servicio.TipoRepuesto = tipoRepuesto;

                    servicios.Add(servicio);
                }

                cn.Close();
            }

            return servicios;
        }




        public ActionResult GetServicioRepuesto()
        {
            List<ServicioRespuesto> servicios = GetServiciosRepuesto();

            return Json(servicios, JsonRequestBehavior.AllowGet);
        }




        // Insert //
        [HttpPost]
        public ActionResult AgregarServicioRepuesto(ServicioRespuesto servicio)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                int idUsuario = (int)Session["IdUsuario"];
                SqlCommand cmd = new SqlCommand("AgregarServicioRespuesto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@IdVehiculo", servicio.IdVehiculo);
                cmd.Parameters.AddWithValue("@IdTipoRepuesto", servicio.IdTipoRepuesto);
                cmd.Parameters.AddWithValue("@Descripcion", servicio.Descripcion);
                try
                {
                    cmd.ExecuteNonQuery();
                    // Servicio de repuesto agregado correctamente
                    return RedirectToAction("Contact", "Home"); // Redirecciona a una página de éxito
                }
                catch (SqlException ex)
                {
                    // Ocurrió un error al agregar el servicio de repuesto
                    ViewBag.ErrorMessage = "Ocurrió un error al agregar el servicio de repuesto: " + ex.Message;
                    return RedirectToAction("Contact", "Home"); // Redirecciona a una página de éxito
                }
            }
        }


        // Get Tipos Repuesto //

        public JsonResult GetTiposRepuesto()
        {
            List<TipoRepuesto> tiposRepuesto = new List<TipoRepuesto>();

            using (SqlConnection connection = new SqlConnection(cadena))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("ObtenerTiposRepuesto", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlDataReader sqlDataReader = command.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    tiposRepuesto.Add(new TipoRepuesto
                    {
                        Id = Convert.ToInt32(sqlDataReader["Id"]),
                        Nombre = sqlDataReader["Nombre"].ToString()
                    });
                }

                connection.Close();
            }

            return Json(tiposRepuesto, JsonRequestBehavior.AllowGet);
        }

        public int DeleteInventario(Inventario inventario)
        {

            int resultToReturn = 0;

            try
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    cn.Open();
                    SqlCommand command = new SqlCommand("DeleteInventario", cn);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", inventario.ID);

                    resultToReturn = command.ExecuteNonQuery();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultToReturn;
        }

        public int DeleteVentas(Ventas ventas)
        {
            int resultToReturn = 0;

            try
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    cn.Open();
                    SqlCommand command = new SqlCommand("DeleteVentas", cn);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", ventas.ID);

                    resultToReturn = command.ExecuteNonQuery();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultToReturn;
        }

        public int DeleteServicioMantenimiento(ServicioMantenimiento servicioMantenimiento)
        {
            int resultToReturn = 0;

            try
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    cn.Open();
                    SqlCommand command = new SqlCommand("DeleteServicioMantenimiento", cn);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", servicioMantenimiento.Id);

                    resultToReturn = command.ExecuteNonQuery();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultToReturn;
        }

        public int DeleteServicioRespuesto(ServicioRespuesto servicioRespuesto)
        {
            int resultToReturn = 0;

            try
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    cn.Open();
                    SqlCommand command = new SqlCommand("DeleteServicioRespuesto", cn);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", servicioRespuesto.Id);

                    resultToReturn = command.ExecuteNonQuery();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultToReturn;
        }


        // Update Repuesto // 
        public int UpdateServicioRepuesto(ServicioRespuesto servicioRepuesto)
        {
            int resultToReturn = 0;

            try
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    cn.Open();
                    using (SqlTransaction transaction = cn.BeginTransaction())
                    {
                        try
                        {
                            SqlCommand command = new SqlCommand("ActualizarServicioRepuesto", cn, transaction);
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Id", servicioRepuesto.Id);
                            command.Parameters.AddWithValue("@IdVehiculo", servicioRepuesto.IdVehiculo);
                            command.Parameters.AddWithValue("@Descripcion", servicioRepuesto.Descripcion);
                            command.Parameters.AddWithValue("@IdTipoRepuesto", servicioRepuesto.IdTipoRepuesto);

                            resultToReturn = command.ExecuteNonQuery();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultToReturn;
        }

        //Update Mantenimiento // 

        public int UpdateServicioMantenimiento(ServicioMantenimiento servicioMantenimiento)
        {
            int resultToReturn = 0;

            try
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    cn.Open();
                    using (SqlTransaction transaction = cn.BeginTransaction())
                    {
                        try
                        {
                            SqlCommand command = new SqlCommand("ActualizarServicioMantenimiento", cn, transaction);
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Id", servicioMantenimiento.Id);
                            command.Parameters.AddWithValue("@IdVehiculo", servicioMantenimiento.IdVehiculo);
                            command.Parameters.AddWithValue("@Descripcion", servicioMantenimiento.Descripcion);
                            command.Parameters.AddWithValue("@IdTipoMantenimiento", servicioMantenimiento.IdTipoMantenimiento);

                            resultToReturn = command.ExecuteNonQuery();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultToReturn;
        }


    }
}



