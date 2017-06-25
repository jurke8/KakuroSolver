using KakuroSolver.Helpers;
using KakuroSolver.Models;
using KakuroSolver.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            var model = new KakuroModel();

            using (var db = new ApplicationDbContext())
            {
                model.StatisticsModel = new StatisticsModel()
                {
                    KakuroStatistics = db.KakuroStatistics.ToList()
                };
            };
            @ViewBag.Position = "navigation";
            return View("Index", model);
        }
        public ActionResult Solver()
        {
            var model = new KakuroModel();

            using (var db = new ApplicationDbContext())
            {
                model.StatisticsModel = new StatisticsModel()
                {
                    KakuroStatistics = db.KakuroStatistics.ToList()
                };
            };
            @ViewBag.Position = "solver";
            return View("Index", model);
        }
        public ActionResult Helper()
        {
            var model = new KakuroModel();

            using (var db = new ApplicationDbContext())
            {
                model.StatisticsModel = new StatisticsModel()
                {
                    KakuroStatistics = db.KakuroStatistics.ToList()
                };
            };
            @ViewBag.Position = "helper";
            return View("Index", model);
        }
        [HttpPost]
        public ActionResult LoadKakuro(KakuroModel model)
        {
            @ViewBag.Position = "solver";
            if (!ModelState.IsValid)
            {
                using (var db = new ApplicationDbContext())
                {
                    model.StatisticsModel = new StatisticsModel()
                    {
                        KakuroStatistics = db.KakuroStatistics.ToList()
                    };
                };
                return View("Index", model);
            }
            var ph = new PictureHelper();

            if (model.KakuroRead.File != null)
            {
                Bitmap originalImage = new Bitmap(model.KakuroRead.File.InputStream);
                var watch = System.Diagnostics.Stopwatch.StartNew();

                var cells = ph.ReadFromImage(originalImage, model.KakuroRead.NumberOfRows, model.KakuroRead.NumberOfColumns);
                var elapsedMs = watch.ElapsedMilliseconds;
                var loaded = true;
                //Check is load correctly
                foreach (var cell in cells)
                {
                    if ("?".Equals(cell.VerticalSum) || "?".Equals(cell.HorizontalSum))
                    {
                        loaded = false;
                        break;
                    }
                }
                var kakuroStatistic = new KakuroStatistic()
                {
                    Rows = model.KakuroRead.NumberOfRows,
                    Columns = model.KakuroRead.NumberOfColumns,
                    LoadTime = elapsedMs,
                    Loaded = loaded
                };
                using (var db = new ApplicationDbContext())
                {
                    db.KakuroStatistics.Add(kakuroStatistic);
                    db.SaveChanges();
                    model.StatisticsModel = new StatisticsModel()
                    {
                        KakuroStatistics = db.KakuroStatistics.ToList()
                    };
                }
                model.KakuroStatistic = kakuroStatistic;
                if (!loaded)
                {

                    ModelState.AddModelError(string.Empty, Resources.Localization.LoadError);
                }
                model.PictureCells = cells;
            }
            else
            {
                var cells = ph.GetBorder(model.KakuroRead.NumberOfRows, model.KakuroRead.NumberOfColumns);
                model.PictureCells = cells;
                using (var db = new ApplicationDbContext())
                {
                    model.StatisticsModel = new StatisticsModel()
                    {
                        KakuroStatistics = db.KakuroStatistics.ToList()
                    };
                }
            }
            return View("Index", model);
        }
        [HttpPost]
        public ActionResult Helper(KakuroModel model)
        {
            @ViewBag.Position = "helper";
            using (var db = new ApplicationDbContext())
            {
                model.StatisticsModel = new StatisticsModel()
                {
                    KakuroStatistics = db.KakuroStatistics.ToList()
                };
            };
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            model.KakuroHelper.Combinations = Algorithm.GetAllCombinations(new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, model.KakuroHelper.NumberOfFields, model.KakuroHelper.Sum);
            return View("Index", model);
        }
        public ActionResult ChangeLanguage(string language, string returnUrl)
        {
            Session["Culture"] = new CultureInfo(language);
            return Redirect(returnUrl);
        }
        [HttpPost]
        public ActionResult SolveKakuro(int columns, int rows, List<string> values, Guid? kakuroid)
        {
            @ViewBag.Position = "solver";
            var cells = new List<PictureCell>();
            var isBorder = false;
            var verticalSum = 0;
            var horizontalSum = 0;
            var totalVerticalSum = 0;
            var totalHorizontalSum = 0;

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
                    totalVerticalSum += verticalSum;
                    totalHorizontalSum += horizontalSum;
                }
                var cell = new PictureCell()
                {
                    IsBorder = isBorder,
                    VerticalSum = verticalSum == 0 ? "" : verticalSum.ToString(),
                    HorizontalSum = horizontalSum == 0 ? "" : horizontalSum.ToString()
                };
                cells.Add(cell);
            }
            var statistic = new StatisticsModel();

            if (totalVerticalSum != totalHorizontalSum)
            {
                ModelState.AddModelError(string.Empty, Resources.Localization.WrongEntry);
                if (kakuroid == null)
                {
                    using (var db = new ApplicationDbContext())
                    {
                        statistic = new StatisticsModel()
                        {
                            KakuroStatistics = db.KakuroStatistics.ToList()
                        };
                        var model2 = new KakuroModel() { PictureCells = cells, KakuroRead = new KakuroReadModel() { NumberOfColumns = columns, NumberOfRows = rows }, Solved = true, StatisticsModel = statistic };
                        return View("Index", model2);
                    }
                }
                else
                {
                    using (var db = new ApplicationDbContext())
                    {
                        var kakuroStat = db.KakuroStatistics.Where(ks => ks.Id == kakuroid).FirstOrDefault();

                        var model2 = new KakuroModel() { PictureCells = cells, KakuroRead = new KakuroReadModel() { NumberOfColumns = columns, NumberOfRows = rows }, Solved = true, KakuroStatistic = kakuroStat, StatisticsModel = statistic };
                        return View("Index", model2);
                    }
                }
            }
            //DO WORK
            var algorithm = new Algorithm();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var resultsCells = algorithm.GetResult(cells, rows, columns);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            var solved = true;

            //Check is solved

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (!resultsCells[i][j].Border && resultsCells[i][j].Value == -1)
                    {
                        solved = false;
                        break;
                    }
                }
                if (!solved)
                {
                    break;
                }
            }
            var kakuro = new KakuroStatistic();
            using (var db = new ApplicationDbContext())
            {
                if (kakuroid != null)
                {
                    kakuro = db.KakuroStatistics.Where(ks => ks.Id == kakuroid).FirstOrDefault();
                    if (kakuro != null && kakuro.SolveTime == 0)
                    {
                        kakuro.SolveTime = elapsedMs;
                        kakuro.Solved = solved;
                        db.Entry(kakuro).State = EntityState.Modified;
                        db.SaveChanges();
                        statistic = new StatisticsModel()
                        {
                            KakuroStatistics = db.KakuroStatistics.ToList()
                        };
                    }
                    else
                    {
                        kakuro = new KakuroStatistic();
                        kakuro.SolveTime = elapsedMs;
                        kakuro.Solved = solved;
                        kakuro.Rows = rows;
                        kakuro.Columns = columns;

                        db.KakuroStatistics.Add(kakuro);
                        db.SaveChanges();
                        statistic = new StatisticsModel()
                        {
                            KakuroStatistics = db.KakuroStatistics.ToList()
                        };
                    }
                }
                else
                {
                    kakuro.SolveTime = elapsedMs;
                    kakuro.Solved = solved;
                    kakuro.Rows = rows;
                    kakuro.Columns = columns;

                    db.KakuroStatistics.Add(kakuro);
                    db.SaveChanges();
                    statistic = new StatisticsModel()
                    {
                        KakuroStatistics = db.KakuroStatistics.ToList()
                    };
                }
            }

            var pictureCells = new List<PictureCell>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    pictureCells.Add(new PictureCell()
                    {
                        IsBorder = resultsCells[i][j].Border,
                        //HorizontalSum = resultsCells[i][j].HorizontalSum.ToString(),
                        //VerticalSum = resultsCells[i][j].VerticalSum.ToString(),
                        HorizontalSum = resultsCells[i][j].HorizontalSum == 0 ? "" : resultsCells[i][j].HorizontalSum.ToString(),
                        VerticalSum = resultsCells[i][j].VerticalSum == 0 ? "" : resultsCells[i][j].VerticalSum.ToString(),
                        Value = resultsCells[i][j].Value.ToString(),
                    });
                }
            }
            var model = new KakuroModel() { PictureCells = pictureCells, KakuroRead = new KakuroReadModel() { NumberOfColumns = columns, NumberOfRows = rows }, Solved = true, KakuroStatistic = kakuro, StatisticsModel = statistic };
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