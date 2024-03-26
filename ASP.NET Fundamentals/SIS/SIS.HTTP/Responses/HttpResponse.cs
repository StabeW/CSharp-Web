using SIS.HTTP.Common;
using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Headers.Contracts;
using SIS.HTTP.Responses.Contracts;
using System.Text;

namespace SIS.HTTP.Responses
{
    public class HttpResponse : IHttpResponse
    {
        public HttpResponse()
        {
        }

        public HttpResponse(HttpResponseStatusCode statusCode) : this()
        {
            CoreValidator.ThrowIfNull(statusCode ,nameof(statusCode));
            StatusCode = statusCode;
        }

        public HttpResponseStatusCode StatusCode { get; set; }

        public IHttpHeaderCollection Headers { get; } = new HttpHeaderCollection();
        public byte[] Content { get; set; } = new byte[0];

        public void AddHeader(HttpHeader header)
        {
            CoreValidator.ThrowIfNull(header, nameof(header));
            Headers.AddHeader(header);
        }

        public byte[] GetBytes()
        {
            byte[] httpResponseBytesWithoutBody = Encoding.UTF8.GetBytes(this.ToString());

            byte[] httpResponseBytesWithBody = new byte[httpResponseBytesWithoutBody.Length + this.Content.Length];

            for (int i = 0; i < httpResponseBytesWithoutBody.Length; i++)
            {
                httpResponseBytesWithBody[i] = httpResponseBytesWithoutBody[i];
            }

            for (int i = 0; i < httpResponseBytesWithBody.Length - httpResponseBytesWithoutBody.Length; i++)
            {
                httpResponseBytesWithBody[i + httpResponseBytesWithoutBody.Length] = this.Content[i];
            }

            return httpResponseBytesWithBody;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result
                .Append($"{GlobalConstants.HttpOneProtocolFragment} {StatusCode} {StatusCode.ToString()}")
                .Append(GlobalConstants.HttpNewLine)
                .Append($"{Headers}").Append(GlobalConstants.HttpNewLine);

            result.Append(GlobalConstants.HttpNewLine);

            return result.ToString();
        }
    }
}
