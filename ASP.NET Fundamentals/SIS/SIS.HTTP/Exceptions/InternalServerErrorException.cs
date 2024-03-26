namespace SIS.HTTP.Exceptions
{
    public class InternalServerErrorException : Exception
    {
        private const string InternalServerErrorMessage = "The Server has encountered an error.";

        public InternalServerErrorException() : this(InternalServerErrorMessage)
        {

        }

        public InternalServerErrorException(string name) : base(name)
        {

        }
    }
}
