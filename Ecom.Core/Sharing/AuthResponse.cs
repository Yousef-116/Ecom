using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Sharing
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; }

        public static AuthResponse Fail(string message)
            => new AuthResponse { Success = false, Message = message };

        public static AuthResponse Ok(string message, string? token = null)
            => new AuthResponse { Success = true, Message = message, Token = token };
    }

}
