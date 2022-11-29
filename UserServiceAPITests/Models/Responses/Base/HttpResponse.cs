﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceAPITests.Models.Responses.Base
{
    public class HttpResponse<T>
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public T Body { get; set; }
    }
}
