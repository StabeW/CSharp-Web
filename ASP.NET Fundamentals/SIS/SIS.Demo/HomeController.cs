﻿using SIS.HTTP.Enums;
using SIS.HTTP.Requests.Contracts;
using SIS.HTTP.Responses.Contracts;
using SIS.WebServer.Results;

namespace SIS.Demo
{
    public class HomeController
    {
        public IHttpResponse Index(IHttpRequest request)
        {
            string content = "<h1>Hello, world!</h1>";

            return new HtmlResult(content, HttpResponseStatusCode.Ok);
        }

    }
}
