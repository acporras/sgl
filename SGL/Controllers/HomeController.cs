using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SGL.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(){ return View(); }
        public ActionResult Login(){ return View(); }
        public ActionResult Resumen() { return View(); }
        public ActionResult Facturas_Listar() { return View(); }
        public ActionResult Facturas_Subir() { return View(); }
        public ActionResult OP_Listar() { return View(); }
        public ActionResult OP_Subir() { return View(); }
        public ActionResult OC_Listar() { return View(); }
        public ActionResult OC_Generar() { return View(); }



        [HttpPost]
        public ActionResult Login(string usuario, string password)
        {
            if (usuario.Equals("admin") && password.Equals("admin"))
            {
                return View("Resumen");
            }
            else
            {
                ViewBag.Response = "Usuario o password incorrectos.";
                return View();
            }
        }

        [HttpPost]
        public ActionResult Facturas_Subir(string usuario, string password)
        {
            return View("Facturas_Listar");
        }



    }
}