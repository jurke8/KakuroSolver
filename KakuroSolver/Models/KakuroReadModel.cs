using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KakuroSolver.Models
{
    public class KakuroReadModel
    {
        [Display(Name = "File", ResourceType = typeof(Localization))]
        public HttpPostedFileBase File { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Localization))]
        [Display(Name = "NumberOfRows", ResourceType = typeof(Localization))]
        [Range(3, 15, ErrorMessageResourceName = "ValueOutOfRange_3_15", ErrorMessageResourceType = typeof(Localization))]
        public int NumberOfRows { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Localization))]
        [Display(Name = "NumberOfColumns", ResourceType = typeof(Localization))]
        [Range(3, 15, ErrorMessageResourceName = "ValueOutOfRange_3_15", ErrorMessageResourceType = typeof(Localization))]
        public int NumberOfColumns { get; set; }
    }
}

