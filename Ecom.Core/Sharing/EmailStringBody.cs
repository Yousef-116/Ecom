using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Sharing
{
    public class EmailStringBody
    {
        public static string send(string email,string token , string component ,string message)
        {
            //string encodeToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
            string encodeToken = Uri.EscapeDataString(token);
            return $@"
                <html>
                <h1>{message}</h1>  
                <hr>
                <br>
                <p>Click the link below:</p>
                <a href=""http://localhost:4200/account/{component}?email={email}&code={encodeToken}"" >Active {message} Click Here</a>
                </html>
                ";

        }
    }
}
