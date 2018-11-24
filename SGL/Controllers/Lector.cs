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
using System.Collections;
using ExcelDataReader;
using System.Data;

namespace SGL.Controllers
{
    public class Lector
    {
        string servidor;
        string archivo;
        List<factura> paquete_factura = new List<factura>();
        List<OP> paquete_OP = new List<OP>();

        public Lector(string servidor, string archivo)
        {
            this.servidor = servidor;
            this.archivo = archivo;
        }


        public List<factura> paquete_facturas()
        {
            // Descomprime ZIP

            System.IO.Compression.ZipFile.ExtractToDirectory(archivo, servidor);

            // Extrae informacion de Facturas en XML

            paquete_factura = new List<factura>();
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
                factura.fecha = Uti.mod_fecha(fecha);
                factura.codFactura = codfactura.Substring(15, 13);
                factura.octanaje = Uti.mod_octanaje(octanaje);
                factura.monto = Double.Parse(Uti.mod_monto(monto));
                factura.scop = scop;
                factura.galones = int.Parse(galones);

                paquete_factura.Add(factura);
                
            }

            eliminar_archivos_servidor();

            // Devuelve Array de Facturas

            return paquete_factura;

        }

        public void eliminar_archivos_servidor()
        {
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
        }
        

        public void subir_facturas()
                {
                    using (Entities obj = new Entities())
                    {
                        foreach (factura factura in paquete_factura)
                        {
                            obj.factura.Add(factura);
                        }
                        obj.SaveChanges();
                    }
                }

        public List<OP> excel()
        {
            DataSet dataSet;
            DataTable table;

            using (var stream = File.Open(archivo, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    dataSet = reader.AsDataSet();
                    table = dataSet.Tables[0];
                }
            }

            int row = 11;
            var current_cell = table.Rows[row][9];
            int total_rows = table.Rows.Count;

            while (row < total_rows - 1)
            {
                OP op = new OP();
                op.codOP = (string)table.Rows[row][9];
                op.fecha = (DateTime)table.Rows[row][11];
                op.unidad = (string)table.Rows[row][2];
                op.planta = (string)table.Rows[row][7];
                op.scop = ((double)table.Rows[row][8]).ToString();
                op.estacion = (string)table.Rows[row][4];
                op.galones = (int)((double)table.Rows[row][6]);
                op.octanaje = (string)table.Rows[row][5];
                paquete_OP.Add(op);
                Console.WriteLine(current_cell);
                row++;
                if (row == 50) { break; }
                current_cell = table.Rows[row][8];
            }

            eliminar_archivos_servidor();

            return paquete_OP;
        }

        public void subir_OP()
        {
            using (Entities obj = new Entities())
            {
                foreach (OP op in paquete_OP)
                {
                    obj.OP.Add(op);
                }
                obj.SaveChanges();
            }
        }
    }
}