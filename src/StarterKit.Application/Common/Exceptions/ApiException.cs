using System.Globalization;

namespace StarterKit.Application.Common.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; }

    public ApiException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public ApiException(int statusCode, string message, params object[] args) 
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
        StatusCode = statusCode;
    }
}