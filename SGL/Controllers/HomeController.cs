using SGL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Xml;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;


namespace SGL.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Login(){return View();}

        public ActionResult OP_Subir()
        {
            // Validacion de ingreso
            usuario usuario = (usuario)Session["Usuario"];
            if (usuario == null) return RedirectToAction("Login");

            return View();
        }

        public ActionResult OC_Generar()
        {
            // Validacion de ingreso
            usuario usuario = (usuario)Session["Usuario"];
            if (usuario == null) return RedirectToAction("Login");

            return View();
        }

        public ActionResult Facturas_Subir()
        {
            // Validacion de ingreso
            usuario usuario = (usuario)Session["Usuario"];
            if (usuario == null) return RedirectToAction("Login");

            return View();
        }


        public ActionResult Eliminar_Factura(factura fac_pas)
        {
            // Validacion de ingreso
            usuario usuario = (usuario)Session["Usuario"];
            if (usuario == null) return RedirectToAction("Login");

            using (Entities obj = new Entities())
            {
                var x = from fac in obj.factura
                        where fac.codFactura.Equals(fac_pas.codFactura)
                        select fac;

                factura objetivo = x.ToList().ElementAt(0);

                edicion edit = new edicion();
                edit.numero = obj.edicion.ToList().Last().numero + 1;
                edit.codDoc = objetivo.codFactura;
                edit.codUsuario = usuario.codUsuario;
                edit.tipoDoc = "factura";
                edit.modificacion = "eliminacion";
                edit.fecha = DateTime.Today;
                edit.hora = DateTime.Now.TimeOfDay;
                obj.edicion.Add(edit);

                obj.factura.Remove(objetivo);
                obj.SaveChanges();
                return RedirectToAction("Facturas_Listar");
            }
        }

        [HttpPost]
        public ActionResult Login(string usuario, string password)
        {
            try
            {
                using (Entities obj = new Entities())
                {
                    var x = from usr in obj.usuario
                            where usr.codUsuario == usuario
                            select usr;

                    usuario user = x.ToList().ElementAt(0);
                    Session["Usuario"] = user;

                    ViewBag.Response = "Ploblema.";
                }
                    return RedirectToAction("Resumen");
            }
            catch (ArgumentOutOfRangeException e)
            {
                ViewBag.Response = "Usuario o password incorrectos.";
                return View();
            }
            catch (Exception e)
            {
                ViewBag.Message = "Tipo de error: " + e.GetType() + "Mensaje: " + e.Message;
                return View();
            }
        }


        public ActionResult Logout()
        {
            Session["Usuario"] = null;

            return RedirectToAction("Login");
        }

        public ActionResult Resumen() {

            // Validacion de ingreso
            usuario usuario = (usuario)Session["Usuario"];
            if (usuario == null) return RedirectToAction("Login");

            using (Entities obj = new Entities())
            {
                var x = obj.edicion;
                List<edicion> lista_ediciones = x.ToList();
                List<edicion> nueva_lista_ediciones = new List<edicion>();

                if (usuario.tipoUsuario.Equals("agente"))
                {
                    foreach (edicion edi in lista_ediciones)
                    {
                        if (edi.codUsuario.Equals(usuario.codUsuario)) nueva_lista_ediciones.Add(edi);
                    }
                }
                else nueva_lista_ediciones = lista_ediciones;

                ViewBag.usuario = usuario;
                ViewBag.lista_ediciones = nueva_lista_ediciones;
                return View();
            }
        }


        public ActionResult Facturas_Listar()
        {
            // Validacion de ingreso
            usuario usuario = (usuario)Session["Usuario"];
            if (usuario == null) return RedirectToAction("Login");

            using (Entities obj = new Entities())
            {
                var x = obj.factura;
                List<factura> lista_facturas = x.ToList();
                ViewBag.lista_facturas = lista_facturas;
                return View();
            }
        }

        public ActionResult OP_Listar()
        {
            // Validacion de ingreso
            usuario usuario = (usuario)Session["Usuario"];
            if (usuario == null) return RedirectToAction("Login");

            using (Entities obj = new Entities())
            {
                var x = obj.OP;
                List<OP> lista_OP = x.ToList();
                ViewBag.lista_OP = lista_OP;
                return View();
            }
        }

        public ActionResult OC_Listar()
        {
            // Validacion de ingreso
            usuario usuario = (usuario)Session["Usuario"];
            if (usuario == null) return RedirectToAction("Login");

            using (Entities obj = new Entities())
            {
                var x = obj.OC;
                List<OC> lista_OC = x.ToList();
                ViewBag.lista_OC = lista_OC;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Facturas_Subir(HttpPostedFileBase file)
        {
            // Validacion de ingreso
            usuario usuario = (usuario)Session["Usuario"];
            if (usuario==null) return RedirectToAction("Login");

            try
            {
                // Coloca archivo ZIP en directorio del servidor
                string servidor = Server.MapPath("~/UploadedFiles");
                string _FileName = Path.GetFileName(file.FileName);
                string _path = Path.Combine(servidor, _FileName);
                file.SaveAs(_path);

                // Leer archivo ZIP en UploadedFiles y extra información de los XML
                Lector lector = new Lector(servidor, _path);
                List<factura> lista_facturas = lector.paquete_facturas();
                ViewBag.lista_facturas = lista_facturas;

                //Sube las facturas a la BD
                lector.subir_facturas();
                ViewBag.message = "Facturas subidas exitosamente.";


                // Guarda edicion de cambios en BD y retorna vista
                using (Entities obj = new Entities())
                {
                    foreach (factura fac in lista_facturas)
                    {
                        edicion edit = new edicion();
                        edit.numero = obj.edicion.ToList().Last().numero + 1;
                        edit.codDoc = fac.codFactura;
                        edit.codUsuario = usuario.codUsuario;
                        edit.tipoDoc = "factura";
                        edit.modificacion = "creacion";
                        edit.fecha = DateTime.Today;
                        edit.hora = DateTime.Now.TimeOfDay;
                        obj.edicion.Add(edit);
                        obj.SaveChanges();
                    }
                    return View();
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException e)
            {
                ViewBag.Message = "Algunas de las facturas ya han sido subidas al servidor.";
                return View();
            }
            catch (Exception e)
            {
                ViewBag.Message = "Tipo de error: "+ e.GetType()+ "Mensaje: "+ e.Message;
                return View();
            }
        }
    }

    
}