using iManageConsoleUpstreamApp.Api.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iManageConsoleUpstreamApp.Api.DocumentPoco
{
    public class Profile
    {
        public Document? document { get; set; }
        public bool warnings_for_required_and_disabled_fields { get; set; } = true;
    }
    
}
