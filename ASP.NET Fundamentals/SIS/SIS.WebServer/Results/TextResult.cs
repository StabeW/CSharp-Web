using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Responses;
using System.Text;

namespace SIS.WebServer.Results
{
    public class TextResult : HttpResponse
    {
        public TextResult(string content, HttpResponseStatusCode responseStatusCode,
            string contentType = "text/plain; charset=utf-8")
            : base(responseStatusCode)
        {
            Headers.AddHeader(new HttpHeader(HttpHeader.ContentType, contentType));
            Content = Encoding.UTF8.GetBytes(content);
        }

        public TextResult(byte[] content, HttpResponseStatusCode responseStatusCode,
            string contentType = "text/plain; charset=utf-8")
            : base(responseStatusCode)
        {
            Content = content;
            Headers.AddHeader(new HttpHeader(HttpHeader.ContentType, contentType));
        }
    }
}
