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
            return View();
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
        public ActionResult LoadKakuro(KakuroModel model)
        {
            @ViewBag.Position = "solver";
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            var ph = new PictureHelper();

            if (model.KakuroRead.File != null)
            {
                Bitmap originalImage = new Bitmap(model.KakuroRead.File.InputStream);

                var cells = ph.ReadFromImage(originalImage, model.KakuroRead.NumberOfRows, model.KakuroRead.NumberOfColumns);
                model.PictureCells = cells;
            }
            else
            {
                var cells = ph.GetBorder(model.KakuroRead.NumberOfRows, model.KakuroRead.NumberOfColumns);
                model.PictureCells = cells;
            }
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
            model.KakuroHelper.Combinations = new Algorithm().GetAllCombinations(new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, model.KakuroHelper.NumberOfFields, model.KakuroHelper.Sum);
            return View("Index", model);
        }
        public ActionResult ChangeLanguage(string language, string returnUrl)
        {
            Session["Culture"] = new CultureInfo(language);
            return Redirect(returnUrl);
        }
        [HttpPost]
        public ActionResult SolveKakuro(int columns, int rows, List<string> values)
        {
            @ViewBag.Position = "solver";
            var cells = new List<PictureCell>();
            var isBorder = false;
            var verticalSum = 0;
            var horizontalSum = 0;
            foreach (var value in values)
            {
                isBorder = false;
                verticalSum = 0;
                horizontalSum = 0;
                var backSlashPos = value.IndexOf("\\");
                if (backSlashPos != -1)
                {
                    isBorder = true;
                    string[] numbers = value.Split('\\');
                    verticalSum = (numbers[0] != "" && numbers[0] != " ") ? int.Parse(numbers[0]) : 0;//set vertical sum of border
                    horizontalSum = (numbers[1] != "" && numbers[1] != " ") ? int.Parse(numbers[1]) : 0;//set horizontal sum of border                   
                }
                var cell = new PictureCell() { IsBorder = isBorder, VerticalSum = verticalSum.ToString(), HorizontalSum = horizontalSum.ToString() };
                cells.Add(cell);
            }
            var algorithm = new Algorithm();

            var resultsCells = algorithm.GetResult(cells, rows, columns);
            var pictureCells = new List<PictureCell>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    pictureCells.Add(new PictureCell()
                    {
                        IsBorder = resultsCells[i][j].Border,
                        HorizontalSum = resultsCells[i][j].HorizontalSum == 0 ? "" : resultsCells[i][j].HorizontalSum.ToString(),
                        VerticalSum = resultsCells[i][j].VerticalSum == 0 ? "" : resultsCells[i][j].VerticalSum.ToString(),
                        Value = resultsCells[i][j].Value.ToString(),
                    });
                }
            }
            var model = new KakuroModel() { PictureCells = pictureCells, KakuroRead = new KakuroReadModel() { NumberOfColumns = columns, NumberOfRows = rows }, Solved = true };
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            //Bitmap originalImage = new Bitmap(model.KakuroRead.File.InputStream);

            //var ph = new PictureHelper();
            //var cells = ph.ReadFromImage(originalImage, model.KakuroRead.NumberOfRows, model.KakuroRead.NumberOfColumns);
            //model.PictureCells = cells;
            return View("Index", model);
        }
    }
}