using System.Net;
using System.Net.Http;

namespace GradeSharp
{
    class HttpResponsePack
    {
        public HttpResponseMessage Response { get; set; }
        public CookieContainer Cookie { get; set; }
    }
}
