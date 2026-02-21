using Ecom.Core.DTO;
using Ecom.Core.Services;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MimeKit;
namespace Ecom.infrastructure.Repositories.Service
{
    internal class EmailService : IEmailService
    {
        private readonly IConfiguration Configuration;
        //SMTP
        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public async Task SendEmail(EmailDTO emailDTO)
        {
            var smtp = new SmtpClient();

            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress("My Ecom", Configuration["EmailSettings:From"]));
                message.To.Add(MailboxAddress.Parse(emailDTO.To));
                message.Subject = emailDTO.Subject;

                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = emailDTO.Content
                };

                await smtp.ConnectAsync(
                    Configuration["EmailSettings:SmtpServer"],
                    int.Parse(Configuration["EmailSettings:Port"]),
                    //MailKit.Security.SecureSocketOptions.StartTls
                    true
                );

                await smtp.AuthenticateAsync(
                    Configuration["EmailSettings:From"],
                    Configuration["EmailSettings:Password"]
                );

                await smtp.SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email sending failed:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                // Optional: rethrow if you want upper layers to handle it
                // throw;
            }
            finally
            {
                if (smtp.IsConnected)
                {
                    await smtp.DisconnectAsync(true);
                }

                smtp.Dispose();
            }
        }
    }
}
