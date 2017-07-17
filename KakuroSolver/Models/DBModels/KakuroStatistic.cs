using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KakuroSolver.Models.DBModels
{
    public class KakuroStatistic : BaseEntity
    {
        [Required]
        public int Rows { get; set; }
        [Required]
        public int Columns { get; set; }
        [NotMapped]
        public int NumberOfFields
        {
            get
            {
                return Rows * Columns;
            }
            set { }
        }
        public long LoadTime { get; set; }
        public long SolveTime { get; set; }
        public bool Loaded { get; set; }
        public bool Solved { get; set; }
        public KakuroStatistic()
        {
            Id = Guid.NewGuid();
        }
    }
}

