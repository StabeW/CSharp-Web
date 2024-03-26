using SIS.HTTP.Common;

namespace SIS.HTTP.Headers
{
    public class HttpHeader
    {
        public const string Cookie = "Cookie";

        public const string ContentType = "Content-Type";

        public const string ContentLength = "Content-Length";

        public const string ContentDisposition = "Content-Disposition";

        public HttpHeader(string key, string value)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));
            CoreValidator.ThrowIfNullOrEmpty(value, nameof(value));

            Key = key;
            Value = value;
        }

        public string Key { get; }

        public string Value { get; }

        public override string ToString()
        {
            return $"{Key}: {Value}";
        }
    }
}
