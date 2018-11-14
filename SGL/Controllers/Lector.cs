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
using System.Collections;


namespace SGL.Controllers
{
    public class Lector
    {
        string servidor;
        string zip;
        List<factura> paquete = new List<factura>();

        public Lector(string servidor, string zip)
        {
            this.servidor = servidor;
            this.zip = zip;
        }


        public List<factura> paquete_facturas()
        {
            // Descomprime ZIP

            System.IO.Compression.ZipFile.ExtractToDirectory(zip, servidor);

            // Extrae informacion de Facturas en XML

            paquete = new List<factura>();
            string[] grupofac = Directory.GetFiles(servidor, "*.xml");

            foreach (string file in grupofac)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(file);

                string direccion = xmlDoc.DocumentElement.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[22].ChildNodes[1].InnerText;
                string planta = xmlDoc.DocumentElement.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[13].ChildNodes[1].InnerText;
                string destino = xmlDoc.DocumentElement.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[53].ChildNodes[1].InnerText;
                string fecha = xmlDoc.DocumentElement.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[10].ChildNodes[1].InnerText;
                string codfactura = xmlDoc.DocumentElement.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[00].ChildNodes[0].InnerText;
                string galones = xmlDoc.DocumentElement.ChildNodes[15].ChildNodes[1].InnerText;
                string octanaje = xmlDoc.DocumentElement.ChildNodes[15].ChildNodes[6].ChildNodes[0].InnerText;
                string monto = xmlDoc.DocumentElement.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[41].ChildNodes[1].InnerText;
                string scop = xmlDoc.DocumentElement.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[38].ChildNodes[1].InnerText;

                factura factura = new factura();

                factura.planta = Uti.mod_planta(planta);
                factura.unidad = Uti.mod_destino(direccion, planta, destino);
                factura.fecha = DateTime.Parse(Uti.mod_fecha(fecha));
                factura.codFactura = codfactura.Substring(15, 13);
                factura.octanaje = Uti.mod_octanaje(octanaje);
                factura.monto = Double.Parse(Uti.mod_monto(monto));
                factura.scop = scop;
                factura.galones = int.Parse(galones);

                paquete.Add(factura);
                
            }

            // Elimina todo el contenido en la carpeta UploadedFiles

            System.IO.DirectoryInfo di = new DirectoryInfo(servidor);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            // Devuelve Array de Facturas

            return paquete;

        }

        public void subir_facturas()
        {
            using (Entities obj = new Entities())
            {
                foreach (factura factura in paquete)
                {
                    obj.factura.Add(factura);
                }
                obj.SaveChanges();
            }
        }
    }
}