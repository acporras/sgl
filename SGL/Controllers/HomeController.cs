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

        public ActionResult OC_Quitar_Facturas(factura fac_pas)
        {
            List<factura> facturas_oc = (List<factura>)Session["FacturasOC"];
            List<factura> nuevas_facturas_oc = new List<factura>();
            foreach (factura factu in facturas_oc)
            {
                if (!factu.codFactura.Equals(fac_pas.codFactura)) nuevas_facturas_oc.Add(factu);
            }
            Session["FacturasOC"] = nuevas_facturas_oc;

            return RedirectToAction("OC_Generar");
        }

        public ActionResult OC_Agregar_Facturas(factura fac_pas)
        {
            List<factura> facturas_oc = (List<factura>)Session["FacturasOC"];
            facturas_oc.Add(fac_pas);
            Session["FacturasOC"] = facturas_oc;

            OC oc = (OC)Session["OC"];
            oc.unidad = fac_pas.unidad;
            oc.planta = fac_pas.planta;
            Session["OC"] = oc;

            return RedirectToAction("OC_Generar");
        }

        [HttpPost]
        public ActionResult OC_Generar(string fecha, string codigo, string unidad, string planta, string programa, string monto_total)
        {
            // Validacion de ingreso
            usuario usuario = (usuario)Session["Usuario"];
            if (usuario == null) return RedirectToAction("Login");

            OC oc = (OC)Session["OC"];
            oc.programa = programa;
            List<factura> facturas_oc = (List<factura>)Session["FacturasOC"];
            List<detalleOC> lista_detalle = new List<detalleOC>();

            foreach (factura fac in facturas_oc)
            {
                detalleOC detalle = new detalleOC();
                detalle.codFactura = fac.codFactura;
                detalle.codOC = oc.codOC;
            }

            using (Entities obj = new Entities())
            {
                obj.OC.Add(oc);

                var objetivo = oc;
                edicion edit = new edicion();
                edit.numero = obj.edicion.ToList().Last().numero + 1;
                edit.codDoc = objetivo.codOC;
                edit.codUsuario = usuario.codUsuario;
                edit.tipoDoc = "OC";
                edit.modificacion = "creacion";
                edit.fecha = DateTime.Today;
                edit.hora = DateTime.Now.TimeOfDay;
                obj.edicion.Add(edit);

                obj.SaveChanges();
            }

            using (Entities obj2 = new Entities())
            {
                foreach (detalleOC detallito in lista_detalle)
                {
                    obj2.detalleOC.Add(detallito);

                    var objetivo = detallito;
                    edicion edit = new edicion();
                    edit.numero = obj2.edicion.ToList().Last().numero + 1;
                    edit.codDoc = objetivo.codFactura;
                    edit.codUsuario = usuario.codUsuario;
                    edit.tipoDoc = "detalle OC";
                    edit.modificacion = "creacion";
                    edit.fecha = DateTime.Today;
                    edit.hora = DateTime.Now.TimeOfDay;
                    obj2.edicion.Add(edit);

                    obj2.SaveChanges();
                }
            }

            string last_codOC = "";
            string next_codOC = "";
            using (Entities obj3 = new Entities())
            {
                last_codOC = obj3.OC.ToList().Last().codOC;
                next_codOC = (int.Parse(last_codOC.Substring(0, 3)) + 1).ToString() + "-C";
            }
            
            Session["RespuestaGeneracionOC"] = "OC generada correctamente.";
            List<factura> facturas_vacio = new List<factura>();
            Session["FacturasOC"] = facturas_vacio;
            OC oc_vacio = new OC();
            oc_vacio.codOC = next_codOC;
            oc_vacio.planta = "";
            oc_vacio.unidad = "";
            oc_vacio.fecha = DateTime.Today;
            Session["OC"] = oc;

            return RedirectToAction("OC_Generar");
        }

        public ActionResult OC_Generar()
        {
            // Validacion de ingreso
            usuario usuario = (usuario)Session["Usuario"];
            if (usuario == null) return RedirectToAction("Login");

            OC oc = (OC)Session["OC"];
            ViewBag.OC = oc;
            List<factura> facturas_oc = (List<factura>)Session["FacturasOC"];
            ViewBag.facturasOC = facturas_oc;
            ViewBag.Response = (string)Session["RespuestaGeneracionOC"];

            using (Entities obj = new Entities())
            {
                var x = obj.factura;
                List<factura> lista_facturas = x.ToList();
                ViewBag.lista_facturas = lista_facturas;
                return View();
            }
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

        public ActionResult Eliminar_OC(OC OC_pas)
        {
            // Validacion de ingreso
            usuario usuario = (usuario)Session["Usuario"];
            if (usuario == null) return RedirectToAction("Login");

            using (Entities obj = new Entities())
            {
                var x = from oc in obj.OC
                        where oc.codOC.Equals(OC_pas.codOC)
                        select oc;

                OC objetivo = x.ToList().ElementAt(0);

                edicion edit = new edicion();
                edit.numero = obj.edicion.ToList().Last().numero + 1;
                edit.codDoc = objetivo.codOC;
                edit.codUsuario = usuario.codUsuario;
                edit.tipoDoc = "OC";
                edit.modificacion = "eliminacion";
                edit.fecha = DateTime.Today;
                edit.hora = DateTime.Now.TimeOfDay;
                obj.edicion.Add(edit);

                obj.OC.Remove(objetivo);
                obj.SaveChanges();
            }

            string last_codOC = "";
            string next_codOC = "";
            using (Entities obj = new Entities())
            {
                last_codOC = obj.OC.ToList().Last().codOC;
                next_codOC = (int.Parse(last_codOC.Substring(0, 3)) + 1).ToString() + "-C";
            }
            OC orden = (OC)Session["OC"];
            orden.codOC = next_codOC;
            Session["OC"] = orden;

            return RedirectToAction("OC_Listar");
        }

        public ActionResult Eliminar_OP(OP OP_pas)
        {
            // Validacion de ingreso
            usuario usuario = (usuario)Session["Usuario"];
            if (usuario == null) return RedirectToAction("Login");

            using (Entities obj = new Entities())
            {
                var x = from op in obj.OP
                        where op.codOP.Equals(OP_pas.codOP)
                        select op;

                OP objetivo = x.ToList().ElementAt(0);

                edicion edit = new edicion();
                edit.numero = obj.edicion.ToList().Last().numero + 1;
                edit.codDoc = objetivo.codOP;
                edit.codUsuario = usuario.codUsuario;
                edit.tipoDoc = "OP";
                edit.modificacion = "eliminacion";
                edit.fecha = DateTime.Today;
                edit.hora = DateTime.Now.TimeOfDay;
                obj.edicion.Add(edit);

                obj.OP.Remove(objetivo);
                obj.SaveChanges();
                return RedirectToAction("OP_Listar");
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
                    List<factura> facturas_oc = new List<factura>();

                    // Creacion de valores para Orden de Compra
                    Session["FacturasOC"] = facturas_oc;
                    string last_codOC = obj.OC.ToList().Last().codOC;
                    string next_codOC = (int.Parse(last_codOC.Substring(0, 3))+1).ToString()+"-C";
                    OC oc = new OC();
                    oc.codOC = next_codOC;
                    oc.planta = "";
                    oc.unidad = "";
                    oc.fecha = DateTime.Today;
                    Session["OC"] = oc;
                    Session["RespuestaGeneracionOC"] = "";

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

                

                //Valida si las facturas tienen OP registrado
                List<OP> listado_OP = new List<OP>();
                List<string> listado_scops = new List<string>();
                using (Entities obj = new Entities())
                {
                    listado_OP = obj.OP.ToList();
                }
                foreach(OP opi in listado_OP)
                {
                    listado_scops.Add(opi.scop);
                }
                string mensaje = "";
                foreach(factura factu in lista_facturas)
                {
                    if (!listado_scops.Contains(factu.scop))
                    {
                        mensaje += " ("+factu.codFactura+","+factu.scop+") ";
                    } 
                }
                if (mensaje.Length > 1)
                {
                    ViewBag.message = "Las siguientes facturas no tienen OP registrado: " + mensaje;
                    return View();
                }

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

        [HttpPost]
        public ActionResult OP_Subir(HttpPostedFileBase file)
        {
            // Validacion de ingreso
            usuario usuario = (usuario)Session["Usuario"];
            if (usuario == null) return RedirectToAction("Login");

            try
            {
                // Coloca archivo ZIP en directorio del servidor
                string servidor = Server.MapPath("~/UploadedFiles");
                string _FileName = Path.GetFileName(file.FileName);
                string _path = Path.Combine(servidor, _FileName);
                file.SaveAs(_path);

                // Leer archivo ZIP en UploadedFiles y extra información de los XML
                Lector lector = new Lector(servidor, _path);
                List<OP> lista_OP = lector.excel();
                ViewBag.lista_OP = lista_OP;

                //Sube las facturas a la BD
                lector.subir_OP();
                ViewBag.message = "Órdenes de pedido subidas exitosamente.";


                // Guarda edicion de cambios en BD y retorna vista
                using (Entities obj = new Entities())
                {
                    foreach (OP op in lista_OP)
                    {
                        edicion edit = new edicion();
                        edit.numero = obj.edicion.ToList().Last().numero + 1;
                        edit.codDoc = op.codOP;
                        edit.codUsuario = usuario.codUsuario;
                        edit.tipoDoc = "OP";
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
                ViewBag.Message = "Tipo de error: " + e.GetType() + "Mensaje: " + e.Message;
                return View();
            }
        }

    }



    
}