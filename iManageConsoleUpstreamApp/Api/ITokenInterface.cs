using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iManageConsoleUpstreamApp.Api
{
    public interface ITokenInterface
    {
        HttpResponseMessage GetToken(string endPoint, dynamic collection);
    }
}
