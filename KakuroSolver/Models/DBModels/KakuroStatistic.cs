using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KakuroSolver.Models.DBModels
{
    public class KakuroStatistic : BaseEntity
    {
        [Required]
        public int Rows { get; set; }
        [Required]
        public int Columns { get; set; }
        //[Display(Name = "NumberOfFields", ResourceType = typeof(Localization))]
        [NotMapped]
        public int NumberOfFields
        {
            get
            {
                return Rows * Columns;
            }
            set { }
        }
        //[Display(Name = "ReadTime", ResourceType = typeof(Localization))]
        public long LoadTime { get; set; }
        //[Display(Name = "SolveTime", ResourceType = typeof(Localization))]
        public long SolveTime { get; set; }
        public bool Loaded { get; set; }
        public bool Solved { get; set; }
        public KakuroStatistic()
        {
            Id = Guid.NewGuid();
        }
    }
}