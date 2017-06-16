using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KakuroSolver.Models.DBModels
{
    public class ApplicationDbContext : DbContext

    {
        public DbSet<KakuroStatistic> KakuroStatistics { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {


        }
    }
}