using System;

namespace Ecom.API.Helper;

public class ApiExceptions : ResponseAPI
{
    public string Details { get; set; }

    public ApiExceptions(int statusCode, string message, string details = null) : base(statusCode, message)
    {
        Details = details;
    }
}
