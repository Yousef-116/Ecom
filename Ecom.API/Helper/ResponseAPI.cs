using System;

namespace Ecom.API.Helper;

public class ResponseAPI
{
    public int StatusCode { get; set; }
    public string Message { get; set; }

    public ResponseAPI(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message ?? GetMessageFromStatusCode(StatusCode);
    }

    private string? GetMessageFromStatusCode(int statusCode)
    {
        return statusCode switch
        {
            200 => "Done",
            400 => "Bad Request",
            401 => "Un Authorized",
            500 => "server Error",
            _ => null,
        };

    }


}
