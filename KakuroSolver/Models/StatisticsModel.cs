using KakuroSolver.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KakuroSolver.Models
{
    public class StatisticsModel
    {
        public List<KakuroStatistic> KakuroStatistics { get; set; }

        public int KakuroNumber
        {
            get
            {
                return KakuroStatistics.Count();
            }
            set { }
        }
        public int SolvedKakuros
        {
            get
            {
                var returnValue = 0;
                foreach (var item in KakuroStatistics)
                {
                    if (item.Solved)
                    {
                        returnValue++;
                    }
                }
                return returnValue;
            }
            set { }
        }
    }
}

