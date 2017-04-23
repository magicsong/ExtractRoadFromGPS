using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MapBox.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult GetGPS()
        {
            string filepath = @"D:\My University\数据挖掘\程序\项目用数据\GPSTraces.json";
            string contentType = MimeMapping.GetMimeMapping(filepath);
            return File(filepath, contentType);
        }
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
    }
}