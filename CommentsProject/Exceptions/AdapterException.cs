using System;

namespace CommentsProject.Exceptions
{
    public class AdapterException : Exception
    {
        public int StatusCode { get; private set; }

        public AdapterException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
