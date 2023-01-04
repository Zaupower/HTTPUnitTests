using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CommonLogic.Models.Responses.Base
{
    public class HttpResponse<T>
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public T Body { get; set; }
        public string Content { get; set; }
    }
}
