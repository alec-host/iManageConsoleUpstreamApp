﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iManageConsoleUpstreamApp.Api.Payload
{
    public class DocProfile
    {
        public string? comment { get; set; }
        public string? database { get; set; }
        public string? default_security { get; set; }
        public string? name { get; set; }
        public string? type { get; set; }
        public string? wstype { get; set; }
        public string? file_create_date { get; set; }
        public string? file_edit_date { get; set; }

    }
}
