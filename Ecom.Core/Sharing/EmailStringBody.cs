using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Sharing
{
    public class EmailStringBody
    {
        public static string send(string email, string token, string component, string message)
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

        //public static string send(string email, string token, string component, string message)
        //{
        //    //string encodeToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
        //    string encodeToken = Uri.EscapeDataString(token);
        //    return $@"
        //        <!DOCTYPE html>
        //        <html>
        //        <head>
        //        <meta charset='UTF-8'>
        //        <style>
        //        body {{
        //            margin:0;
        //            padding:0;
        //            background-color:#f4f6f9;
        //            font-family: Arial, Helvetica, sans-serif;
        //        }}

        //        .container {{
        //            max-width:600px;
        //            margin:40px auto;
        //            background:#ffffff;
        //            border-radius:10px;
        //            overflow:hidden;
        //            box-shadow:0 5px 20px rgba(0,0,0,0.1);
        //        }}

        //        .header {{
        //            background:#4f46e5;
        //            color:white;
        //            text-align:center;
        //            padding:30px;
        //            font-size:24px;
        //            font-weight:bold;
        //        }}

        //        .content {{
        //            padding:40px;
        //            text-align:center;
        //            color:#333;
        //        }}

        //        .content h1 {{
        //            margin-top:0;
        //            font-size:22px;
        //        }}

        //        .content p {{
        //            color:#666;
        //            font-size:15px;
        //        }}

        //        .button {{
        //            display:inline-block;
        //            margin-top:25px;
        //            padding:14px 30px;
        //            background:#4f46e5;
        //            color:#ffffff !important;
        //            text-decoration:none;
        //            border-radius:6px;
        //            font-weight:bold;
        //            font-size:16px;
        //        }}

        //        .button:hover {{
        //            background:#4338ca;
        //        }}

        //        .footer {{
        //            text-align:center;
        //            padding:20px;
        //            font-size:12px;
        //            color:#999;
        //            border-top:1px solid #eee;
        //        }}
        //        </style>
        //        </head>

        //        <body>

        //        <div class='container'>

        //            <div class='header'>
        //                Account Notification
        //            </div>

        //            <div class='content'>
        //                <h1>{message}</h1>
        //                <p>
        //                    Please confirm your request by clicking the button below.
        //                </p>

        //                <a class='button' 
        //                href='http://localhost:4200/account/{component}?email={email}&code={encodeToken}'>
        //                    Activate {message}
        //                </a>

        //                <p style='margin-top:25px;font-size:13px;color:#888;'>
        //                    If the button doesn't work, copy and paste the link into your browser.
        //                </p>
        //            </div>

        //            <div class='footer'>
        //                © 2026 Your Company. All rights reserved.
        //            </div>

        //        </div>

        //        </body>
        //        </html>
        //        ";
        //}
    }
}
