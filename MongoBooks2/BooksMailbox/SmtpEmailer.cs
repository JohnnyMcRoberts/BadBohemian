namespace BooksMailbox
{
    using System;

    using System.Net.Mail;

    using System.Net;
    
    public class SmtpEmailer
    {
        private static SmtpClient SetupSmtpClient(StmpConnection connection)
        {
            SmtpClient client = new SmtpClient(connection.Host, connection.Port)
            {
                Credentials = new NetworkCredential(connection.FromEmail, connection.Password),
                EnableSsl = true
            };
            return client;
        }

        public static void SendTestEmail(StmpConnection connection, string toEmail)
        {
            SmtpClient client = SetupSmtpClient(connection);

            client.Send(connection.FromEmail, toEmail, "test", "testbody");
            Console.WriteLine("Sent");
        }

        public static void SendHtmlEmail(StmpConnection connection, HtmlEmailDefinition emailDefinition)
        {
            SmtpClient client = SetupSmtpClient(connection);

            MailMessage message = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress(connection.FromEmail, emailDefinition.FromEmailDisplayName),
                Subject = emailDefinition.Subject,
                Body = emailDefinition.BodyHtml
            };

            message.To.Add(new MailAddress(emailDefinition.ToEmail, emailDefinition.ToEmailDisplayName));

            client.Send(message);
        }
    }
}