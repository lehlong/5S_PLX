﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Filter.MD
{
    public class MappingCustomerPointFilter : BaseFilter
    {
        public string? CustomerCode { get; set; }
        public string? PointCode { get; set; }
    }
}
