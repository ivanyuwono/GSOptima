using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

namespace GSOptima.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        //public Task SendEmailAsync(string email, string subject, string message)
        //{
        //    // Plug in your email service here to send an email.
        //    return Task.FromResult(0);
        //}

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }

        public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
            //Options.SendGridKey = "-BVRLClMTEyeMK3saj8mMA.92pdwdbe1mnU5hq8Gxkmg_aQpDs5G6N1LwyqDBtM67k";
            //Options.SendGridKey = "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";
            Options.SendGridKey = "SG.2nBI7_QzTY6_UNt - WrZMkg.yqDf8RoEs3pKLvaYc3YNS4bhl88T8DTjONqoJln0_co";
            Options.SendGridUser = "ivanyuwono2@yahoo.com";

            //SG.QsEL3MvoQCu99oTa3BwwtQ.sWZ68cdieEqIFqF8sJbsUKwU7FwXAyQ - rI_C_hZCjTY
            //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
        }

        public AuthMessageSenderOptions Options { get; set; } //set only via Secret Manager
        public Task SendEmailAsync(string email, string subject, string message, string environment)
        {
            // Plug in your email service here to send an email.
            if (environment.ToUpper() == "DEVELOPMENT")
                Execute(Options.SendGridKey, subject, message, email).Wait(); //development
            else
                SendMail(email, subject, message).Wait();  //production
            
            return Task.FromResult(0);
        }
        public async Task SendMail(string email, string subject, string message)
        {

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("GSOptima Admin", "mail@gsoptima.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            //emailMessage.Body = new TextPart("plain") { Text = message };


            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;
            emailMessage.Body = bodyBuilder.ToMessageBody();


            using (var client = new SmtpClient())
            {
                client.LocalDomain = "mail.gsoptima.com";
                

                await client.ConnectAsync("mail.gsoptima.com", 587, SecureSocketOptions.None).ConfigureAwait(false);
                
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("mail@gsoptima.com", "masterclass#1");
                await client.SendAsync(emailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }

        }
        public async Task Execute(string apiKey, string subject, string message, string email)
        {
            //var client = new SendGridClient(@"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");
            var client = new SendGridClient(@"SG.2nBI7_QzTY6_UNt - WrZMkg.yqDf8RoEs3pKLvaYc3YNS4bhl88T8DTjONqoJln0_co");  //ini key baru untuk ivanyuwono2@yahoo.com
            
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("ivanyuwono2@yahoo.com", "GSOptima Admin"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            var response = await client.SendEmailAsync(msg);

        }

    }
}
