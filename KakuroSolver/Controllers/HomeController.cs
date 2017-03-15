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
            var model = new MyViewModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(MyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Bitmap originalImage = new Bitmap(model.File.InputStream);

            var ph = new PictureHelper();
            var cells = ph.ReadFromImage(originalImage, 5, 5);

            // now you could pass the byte array to your model and store wherever 
            // you intended to store it

            return View(model);
        }
        public ActionResult ChangeLanguage(string language, string returnUrl)
        {
            Session["Culture"] = new CultureInfo(language);
            return Redirect(returnUrl);
        }
    }
}