﻿using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KakuroSolver.Models
{
    public class KakuroHelperModel
    {
        [Required]
        [Display(Name = "NumberOfFields", ResourceType = typeof(Localization))]
        [Range(2, 9, ErrorMessageResourceName = "ValueOutOfRange", ErrorMessageResourceType = typeof(Localization))]
        public int NumberOfFields { get; set; }

        [Required]
        [Display(Name = "Sum", ResourceType = typeof(Localization))]
        [Range(3, 45, ErrorMessageResourceName = "ValueOutOfRange", ErrorMessageResourceType = typeof(Localization))]
        public int Sum { get; set; }

        [Display(Name = "Combinations", ResourceType = typeof(Localization))]
        public List<List<int>> Combinations { get; set; }
    }
}