using MN_WEB.Entities;
using MN_WEB.Models;
using System.Web.Mvc;

namespace MN_WEB.Controllers
{
    public class HomeController : Controller
    {
        UsuarioModel model = new UsuarioModel();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IniciarSesion(UsuarioEnt entidad)
        {
            var resp = model.IniciarSesion(entidad);

            if (resp != null)
                return RedirectToAction("Principal", "Home");
            else
                return View("Index");
        }


        [HttpGet]
        public ActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrarse(UsuarioEnt entidad)
        {
            entidad.IdRol = 2;
            entidad.Estado = true;

            var resp = model.Registrarse(entidad);

            if (resp > 0)
                return RedirectToAction("Index", "Home");
            else
                return View("Registro");
        }


        [HttpGet]
        public ActionResult Recuperar()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Principal()
        {
            return View();
        }

    }
}