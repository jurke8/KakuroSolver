using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KakuroSolver.Models
{
    public class MyViewModel
    {
        [Required]
        public HttpPostedFileBase File { get; set; }
    }
}