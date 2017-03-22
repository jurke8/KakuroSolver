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
        public ActionResult Solver(KakuroModel model)
        {
            @ViewBag.Position = "solver";
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            Bitmap originalImage = new Bitmap(model.KakuroRead.File.InputStream);

            var ph = new PictureHelper();
            var cells = ph.ReadFromImage(originalImage, model.KakuroRead.NumberOfRows, model.KakuroRead.NumberOfColumns);

            return View("Index", model);
        }
        [HttpPost]
        public ActionResult Helper(KakuroModel model)
        {
            @ViewBag.Position = "helper";
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            // do stuff - call findall combinations
            model.KakuroHelper.Combinations = new Algorithm().GetAllCombinations(new List<int>() {1,2,3,4,5,6,7,8,9 }, model.KakuroHelper.NumberOfFields,model.KakuroHelper.Sum);
            return View("Index",model);
        }
        public ActionResult ChangeLanguage(string language, string returnUrl)
        {
            Session["Culture"] = new CultureInfo(language);
            return Redirect(returnUrl);
        }
    }
}