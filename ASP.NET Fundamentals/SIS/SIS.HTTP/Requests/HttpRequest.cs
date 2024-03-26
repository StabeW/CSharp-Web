using SIS.HTTP.Common;
using SIS.HTTP.Enums;
using SIS.HTTP.Exceptions;
using SIS.HTTP.Headers;
using SIS.HTTP.Headers.Contracts;
using SIS.HTTP.Requests.Contracts;
using System.Net;
using System.Web;

namespace SIS.HTTP.Requests
{
    public class HttpRequest : IHttpRequest
    {
        public HttpRequest(string requestString)
        {
            CoreValidator.ThrowIfNullOrEmpty(requestString, nameof(requestString));
        }
        public string Path { get; private set; }

        public string Url { get; private set; }

        public Dictionary<string, ISet<string>> FormData { get; } = new Dictionary<string, ISet<string>>();

        public Dictionary<string, ISet<string>> QueryData { get; } = new Dictionary<string, ISet<string>>();

        public IHttpHeaderCollection Headers { get; } = new HttpHeaderCollection();

        public HttpRequestMethod RequestMethod { get; private set; }

        private bool IsValidRequestLine(string[] requestLineParams)
        {
            if (requestLineParams.Length != 3
                || requestLineParams[2] != GlobalConstants.HttpOneProtocolFragment)
            {
                return false;
            }

            return true;
        }

        private bool IsValidRequestQueryString(string queryString, string[] queryParameters)
        {
            CoreValidator.ThrowIfNullOrEmpty(queryString, nameof(queryString));

            return true;
        }

        private bool HasQueryString()
        {
            return Url.Split('?').Length > 1;
        }

        private IEnumerable<string> ParsePlainRequestHeaders(string[] requestLines)
        {
            for (int i = 1; i < requestLines.Length - 1; i++)
            {
                if (!string.IsNullOrEmpty(requestLines[i]))
                {
                    yield return requestLines[i];
                }
            }
        }

        private void ParseRequestMethod(string[] requestLine)
        {
            bool parseResult = HttpRequestMethod.TryParse(requestLine[0], true,
                out HttpRequestMethod method);

            if (!parseResult)
            {
                throw new BadRequestException(
                    string.Format(GlobalConstants.UnsupportedHttpMethodExceptionMessage,
                        requestLine[0]));
            }

            RequestMethod = method;
        }

        private void ParseRequestUrl(string[] requestLine)
        {
            Url = HttpUtility.UrlDecode(requestLine[1]);
        }

        private void ParseRequestPath()
        {
            Path = Url.Split('?')[0];
        }

        private void ParseRequestHeaders(string[] requestContent)
        {
            requestContent.Select(content => content.Split(new[] { ": " }
                    , StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(headerKeyValuePair => Headers.AddHeader(new HttpHeader(headerKeyValuePair[0], headerKeyValuePair[1])));
        }

        private void ParseQueryParameters()
        {
            if (HasQueryString())
            {
                List<string[]> parameters = Url.Split('?', '#')[1]
                    .Split('&')
                    .Select(plainQueryParameter => plainQueryParameter.Split('='))
                    .ToList();

                foreach (var parameter in parameters)
                {
                    if (!QueryData.ContainsKey(parameter[0]))
                    {
                        QueryData.Add(parameter[0], new HashSet<string>());
                    }

                    QueryData[parameter[0]].Add(WebUtility.UrlDecode(parameter[1]));
                }
            }
        }

        private void ParseRequestFormDataParameters(string formData)
        {
            if (string.IsNullOrEmpty(formData) == false)
            {
                List<string[]> data = formData
                   .Split('&')
                   .Select(formDataParameter => formDataParameter.Split('='))
                   .ToList();

                foreach (var formDataKvp in data)
                {
                    string key = formDataKvp[0];
                    string value = formDataKvp[1];

                    if (FormData.ContainsKey(key) == false)
                    {
                        FormData.Add(key, new HashSet<string>());
                    }

                    FormData[key].Add(WebUtility.UrlDecode(value));
                }
            }
        }

        private void ParseRequestParameters(string formData)
        {
            ParseQueryParameters();
            ParseRequestFormDataParameters(formData);
        }

        private void ParseRequest(string requestString)
        {
            string[] splitRequestContent = requestString
                .Split(new[] { GlobalConstants.HttpNewLine }, StringSplitOptions.None);

            string[] requestLine = splitRequestContent[0]
                .Trim()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (!IsValidRequestLine(requestLine))
            {
                throw new BadRequestException();
            }

            ParseRequestMethod(requestLine);
            ParseRequestUrl(requestLine);
            ParseRequestPath();

            ParseRequestHeaders(ParsePlainRequestHeaders(splitRequestContent).ToArray());

            ParseRequestParameters(splitRequestContent[splitRequestContent.Length - 1]);
        }
    }
}


