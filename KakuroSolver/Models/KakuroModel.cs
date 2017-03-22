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
    }
}