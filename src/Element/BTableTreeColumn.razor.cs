﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Element
{
    public partial class BTableTreeColumn : BTableColumn
    {
        internal override bool IsTree { get; set; } = true;
    }
}
