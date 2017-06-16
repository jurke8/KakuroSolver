using KakuroSolver.Models;
using KakuroSolver.Models.DBModels;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KakuroSolver.Models
{
    public class KakuroModel
    {
        public KakuroHelperModel KakuroHelper { get; set; }
        public KakuroReadModel KakuroRead { get; set; }
        public List<PictureCell> PictureCells { get; set; }
        public KakuroStatistic KakuroStatistic { get; set; }
        public StatisticsModel StatisticsModel { get; set; }

        public bool Solved { get; set; }
    }
}