﻿

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class DropDownOption : PopupOption
    {
        public Action Refresh { get; set; }
        public object Select { get; set; }
        public RenderFragment OptionContent { get; set; }
        public bool IsTree { get; set; }
        internal float Width { get; set; }
    }
}
