using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iManageConsoleUpstreamApp.Api
{
    public interface ICustomerDiscoveryInterface
    {
        HttpResponseMessage GetCustomerID(string endPoint);
    }
}
