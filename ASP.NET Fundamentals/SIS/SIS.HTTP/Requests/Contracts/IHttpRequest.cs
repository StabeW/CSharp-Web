using SIS.HTTP.Enums;
using SIS.HTTP.Headers.Contracts;

namespace SIS.HTTP.Requests.Contracts
{
    public interface IHttpRequest
    {
        string Path { get; }

        string Url { get; }

        Dictionary<string, ISet<string>> FormData { get; }
        Dictionary<string, ISet<string>> QueryData { get; }

        IHttpHeaderCollection Headers { get; }

        HttpRequestMethod RequestMethod { get; }
    }
}
