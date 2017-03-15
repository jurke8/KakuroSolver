using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KakuroSolver.Models
{
    public class KakuroViewModel
    {
        [Required]
        [Display(Name = "File", ResourceType = typeof(Localization))]
        public HttpPostedFileBase File { get; set; }

        [Required]
        [Display(Name = "NumberOfRows", ResourceType = typeof(Localization))]
        [Range(3, 15, ErrorMessageResourceName = "ValueOutOfRange", ErrorMessageResourceType = typeof(Localization))]
        public int NumberOfRows { get; set; }

        [Required]
        [Display(Name = "NumberOfColumns", ResourceType = typeof(Localization))]
        [Range(3, 15, ErrorMessageResourceName = "ValueOutOfRange", ErrorMessageResourceType = typeof(Localization))]
        public int NumberOfColumns { get; set; }
    }
}