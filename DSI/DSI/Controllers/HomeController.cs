using DSI.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSI.Controllers
{
    public class HomeController : Controller
    {
        DSIDataASPEntities db = new DSIDataASPEntities();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Employees()
        {
            ViewBag.Message = "Módulo de Exportación de datos.";

            return View(db.Employees.ToList());
        }

        public ActionResult Exports(string ReportType)
        {
            LocalReport localreport = new LocalReport
            {
                ReportPath = Server.MapPath("~/Reports/EmployeeListReport.rdlc")
            };

            ReportDataSource reportDataSource = new ReportDataSource
            {
                Name = "EmployeeDataSet",
                Value = db.Employees.ToList()
            };
            localreport.DataSources.Add(reportDataSource);

            string reportType = ReportType;
            string mimeType;
            string encoding;
            string fileNameExtension;

            if (reportType == "Excel")
            {
                fileNameExtension = "xls";
            }
            else if (reportType == "EXCELOPENXML")
            {
                fileNameExtension = "xlsx";
            }
            else if (reportType == "CSV")
            {
                fileNameExtension = "csv";
            }
            else
            {
                fileNameExtension = "pdf";
            }

            string[] streams;
            Warning[] warnings;
            byte[] renderedByte;
            renderedByte = localreport.Render(reportType, null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            Response.AddHeader("Content-Disposition", "attachment; filename = employee_report." + fileNameExtension);
            return File(renderedByte, fileNameExtension);
        }
        
    }
}