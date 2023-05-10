using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iManageConsoleUpstreamApp.Api
{
    public interface IDocumentApiInterface
    {
        HttpResponseMessage HttpPostAttachedFile(string endPoint,string payload,string filePath);
    }
}
