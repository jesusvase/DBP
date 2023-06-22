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
    public class AccesoController : Controller
    {

        static string cadena = "Data Source=(local);Initial Catalog=LABORATORIO3;Integrated Security=true";



        // GET: Acceso
        public ActionResult Login()
        {
            return View();
        }


        public ActionResult Registrar()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Registrar(Usuario oUsuario)
        {
            bool registrado;
            string mensaje;

            if (oUsuario.Clave == oUsuario.ConfirmarClave)
            {

                oUsuario.Clave = ConvertirSha256(oUsuario.Clave);
            }
            else {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            using (SqlConnection cn = new SqlConnection(cadena)) {

                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar,100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();


            }

            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("Login", "Acceso");
            }
            else {
                return View();
            }

        }

        public ActionResult Registrar2()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar2(Usuario oUsuario)
        {
            bool registrado;
            string mensaje;

            if (oUsuario.Clave == oUsuario.ConfirmarClave)
            {

                oUsuario.Clave = ConvertirSha256(oUsuario.Clave);
            }
            else
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            using (SqlConnection cn = new SqlConnection(cadena))
            {

                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario2", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();


            }

            ViewData["Mensaje"] = mensaje;


                return RedirectToAction("Index", "Home");
            
        }

        public ActionResult Registrar3()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar3(Usuario oUsuario)
        {
            bool registrado;
            string mensaje;

            if (oUsuario.Clave == oUsuario.ConfirmarClave)
            {

                oUsuario.Clave = ConvertirSha256(oUsuario.Clave);
            }
            else
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            using (SqlConnection cn = new SqlConnection(cadena))
            {

                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario3", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();


            }

            ViewData["Mensaje"] = mensaje;


            return RedirectToAction("Contact", "Home");

        }

        public List<Usuario> Get()
        {

            List<Usuario> Usuarios = new List<Usuario>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {

                cn.Open();
                SqlCommand command = new SqlCommand("ObtenerUsuarios", cn);

                SqlDataReader sqlDataReader = command.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    Usuarios.Add(new Usuario
                    {
                        IdUsuario = Convert.ToInt32(sqlDataReader["IdUsuario"]),
                        Correo = sqlDataReader["Correo"].ToString(),
                        Clave = sqlDataReader["Clave"].ToString(),
                        Tipo = Convert.ToInt32(sqlDataReader["Tipo"])
                    });

                }

                cn.Close();

                return Usuarios;
            }

        }

        public ActionResult GetUsuarios()
        {
            List<Usuario> usuarios = Get(); // Obtener los usuarios desde el controlador

            return Json(usuarios, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult Login(Usuario oUsuario)
        {
            oUsuario.Clave = ConvertirSha256(oUsuario.Clave);

            try
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                    cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                    cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            oUsuario.IdUsuario = Convert.ToInt32(reader["IdUsuario"]);
                            oUsuario.Tipo = Convert.ToInt32(reader["Tipo"]);

                            // Guardar el ID del usuario en la sesión
                            Session["IdUsuario"] = oUsuario.IdUsuario;
                        }
                        else
                        {
                            ViewData["Mensaje"] = "Usuario no encontrado";
                            return View();
                        }
                    }

                    string tipo2 = "";
                    if (oUsuario.Tipo == 1)
                    {
                        Session["usuario"] = oUsuario;
                        tipo2 = "Index";
                    }
                    else if (oUsuario.Tipo == 2)
                    {
                        Session["usuario"] = oUsuario;
                        tipo2 = "About";
                    }
                    else if (oUsuario.Tipo == 3)
                    {
                        Session["usuario"] = oUsuario;
                        tipo2 = "Contact";
                    }

                    if (!string.IsNullOrEmpty(tipo2))
                    {
                        return RedirectToAction(tipo2, "Home");
                    }
                    else
                    {
                        // Acción por defecto si tipo2 no tiene un valor válido
                        return RedirectToAction("DefaultAction", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo del error
                ViewData["Mensaje"] = "Ocurrió un error al iniciar sesión " ;
                return View();
            }
        }


        public static string ConvertirSha256(string texto)
        {

            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        //UPDATE 
        public int UpdateUsuario(Usuario usuario)
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
                            SqlCommand command = new SqlCommand("ActualizarUsuario", cn, transaction);
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            string contraseñaEncriptada = ConvertirSha256(usuario.Clave);
                            command.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
                            command.Parameters.AddWithValue("@Correo", usuario.Correo);
                            command.Parameters.AddWithValue("@Clave", contraseñaEncriptada);
                            command.Parameters.AddWithValue("@Tipo", usuario.Tipo);

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

        //DELETE 
        public int Delete(Usuario usuario)
        {

            int resultToReturn = 0;
            
            try
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {


                    cn.Open();
                    SqlCommand command = new SqlCommand("DeleteUsuario", cn);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);

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



    /////////////////////////////////////////////////CARRO////////////////
    public ActionResult RegistrarVehiculo(Vehiculo vehiculo)
        {

            using (SqlConnection cn = new SqlConnection(cadena))
            {

                SqlCommand cmd = new SqlCommand("InsertarVehiculo", cn);
                cmd.Parameters.AddWithValue("Nombre", vehiculo.Nombre);
                cmd.Parameters.AddWithValue("TipoVehiculoID", vehiculo.TipoVehiculo.ID);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();




            }
            return RedirectToAction("Registrar", "Carro");
        }

        public JsonResult GetTipo()
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

    }
}




