using System.Net;
using System.Net.Mail;

namespace ETC
{
    public class EmailSender : IEmailSender
    {

        public Task SendEmailAsync(string ToEmail, string subject, string body)
        {
            var fromAddress = "webadmin@[INSERTDOMAIN].com";
            // var pw = "";

          


            var client = new SmtpClient("smtp.[INSERTDOMAIN].com")
            {
                 //EnableSsl = true,
                //Credentials = new NetworkCredential(mail, pw)
            };

            MailMessage mailMessage = new MailMessage(fromAddress, ToEmail, subject, body)
            {
                // Set IsBodyHtml to true to indicate the body is HTML
                IsBodyHtml = true
            };


            return client.SendMailAsync(mailMessage);
        }

      


    }
}
