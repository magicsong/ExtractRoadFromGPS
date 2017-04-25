using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MapBox.Controllers
{
    public class GetDataController : Controller
    {
        // GET: GetData
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetStartPoints()
        {
            string filepath = Server.MapPath("~/App_Data/OrigPoints.json");
            string contentType = MimeMapping.GetMimeMapping(filepath);
            return File(filepath, contentType);
        }
        public ActionResult GetEndPoints()
        {
            string filepath = Server.MapPath("~/App_Data/DesPoints.json");
            string contentType = MimeMapping.GetMimeMapping(filepath);
            return File(filepath, contentType);
        }
    }
}