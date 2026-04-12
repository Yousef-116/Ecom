using System;

namespace Ecom.Core.Sharing
{
    public class EmailStringBody
    {
        public static string send(string email, string token, string component, string message)
        {
            string encodeToken = Uri.EscapeDataString(token);
            string link = $"http://localhost:4200/account/{component}?email={email}&code={encodeToken}";

            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            margin: 0;
            padding: 0;
            background-color: #0c0e12;
            font-family: 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
            color: #ffffff;
        }}
        .wrapper {{
            width: 100%;
            table-layout: fixed;
            background-color: #0c0e12;
            padding: 40px 0;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #161a22;
            border-radius: 16px;
            border: 1px solid rgba(255, 255, 255, 0.08);
            overflow: hidden;
            box-shadow: 0 20px 40px rgba(0, 0, 0, 0.4);
        }}
        .header {{
            background: linear-gradient(135deg, #6366f1 0%, #55c1f7ff 100%);
            padding: 40px 20px;
            text-align: center;
        }}
        .logo {{
            font-size: 28px;
            font-weight: 800;
            color: #ffffff;
            letter-spacing: -1px;
            text-transform: uppercase;
            margin-bottom: 10px;
        }}
        .content {{
            padding: 40px 30px;
            text-align: center;
        }}
        .title {{
            font-size: 24px;
            font-weight: 700;
            color: #ffffff;
            margin-bottom: 20px;
        }}
        .text {{
            font-size: 16px;
            line-height: 1.6;
            color: #94a3b8;
            margin-bottom: 35px;
        }}
        .button-container {{
            margin: 30px 0;
        }}
        .button {{
            background: linear-gradient(135deg, #6366f1 0%, #559ef7ff 100%);
            color: #ffffff !important;
            text-decoration: none;
            padding: 16px 36px;
            border-radius: 12px;
            font-weight: 700;
            font-size: 16px;
            display: inline-block;
            transition: transform 0.2s ease;
            box-shadow: 0 10px 20px rgba(99, 102, 241, 0.3);
        }}
        .footer {{
            padding: 30px;
            text-align: center;
            font-size: 13px;
            color: #475569;
            border-top: 1px solid rgba(255, 255, 255, 0.05);
        }}
        .divider {{
            height: 1px;
            background: rgba(255, 255, 255, 0.05);
            margin: 20px 0;
        }}
    </style>
</head>
<body>
    <div class=""wrapper"">
        <div class=""container"">
            <div class=""header"">
                <div class=""logo"">E-COMMERCE</div>
                <div style=""font-size: 14px; opacity: 0.8; font-weight: 500;"">PREMIUM SHOPPING EXPERIENCE</div>
            </div>
            
            <div class=""content"">
                <h1 class=""title"">{message}</h1>
                <p class=""text"">
                    Hello,<br><br>
                    You have requested a secure action for your account. Please click the button below to proceed with <strong>{message}</strong>.
                </p>
                
                <div class=""button-container"">
                    <a href=""{link}"" class=""button"">Confirm {message}</a>
                </div>
                
                <p class=""text"" style=""font-size: 14px; margin-top: 40px;"">
                    If the button doesn't work, you can also copy and paste this link into your browser:
                    <br>
                    <span style=""color: #28a6dcff; word-break: break-all;"">{link}</span>
                </p>
            </div>
            
            <div class=""footer"">
                <p>&copy; 2026 E-Commerce. All rights reserved.</p>
                <p>This is an automated message, please do not reply to this email.</p>
            </div>
        </div>
    </div>
</body>
</html>
";
        }
    }
}