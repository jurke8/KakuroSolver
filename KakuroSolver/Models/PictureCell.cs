﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KakuroReader
{
    public class PictureCell
    {
        public bool IsBorder { get; set; }
        public string VerticalSum { get; set; }
        public string HorizontalSum { get; set; }
    }
}