namespace SIS.HTTP.Exceptions
{
    public class BadRequestException : Exception
    {
        private const string BadRequestMessage = "The Request was malformed or contains unsupported elements.";

        public BadRequestException() : this(BadRequestMessage)
        {
            
        }

        public BadRequestException(string name) : base(name)
        {
            
        }
    }
}
