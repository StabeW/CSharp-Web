using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Responses;
using System.Text;

namespace SIS.WebServer.Results
{
    public class HtmlResult : HttpResponse
    {
        public HtmlResult(string content, HttpResponseStatusCode responseStatusCode)
            :base(responseStatusCode)
        {
            Headers.AddHeader(new HttpHeader(
                HttpHeader.ContentType, "text/html; charset=utf-8"));
            Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
