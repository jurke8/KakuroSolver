using KakuroSolver.Helpers;
using KakuroSolver.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KakuroSolver.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            @ViewBag.Position = "navigation";
            return View("Index");
        }
        public ActionResult Solver()
        {
            @ViewBag.Position = "solver";
            return View("Index");
        }
        public ActionResult Helper()
        {
            @ViewBag.Position = "helper";
            return View("Index");
        }
        [HttpPost]
        public ActionResult Solver(KakuroReadModel model)
        {
            @ViewBag.Position = "solver";
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            Bitmap originalImage = new Bitmap(model.File.InputStream);

            var ph = new PictureHelper();
            var cells = ph.ReadFromImage(originalImage, model.NumberOfRows, model.NumberOfColumns);

            // now you could pass the byte array to your model and store wherever 
            // you intended to store it

            return View("Index");
        }
        [HttpPost]
        public ActionResult Helper(KakuroHelperModel model)
        {
            @ViewBag.Position = "helper";
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            // now you could pass the byte array to your model and store wherever 
            // you intended to store it

            return View("Index");
        }
        public ActionResult ChangeLanguage(string language, string returnUrl)
        {
            Session["Culture"] = new CultureInfo(language);
            return Redirect(returnUrl);
        }
    }
}