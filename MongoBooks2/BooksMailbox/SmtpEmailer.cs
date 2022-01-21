namespace BooksMailbox
{
    using System;
    using System.IO;

    using System.Net;
    using System.Net.Mail;

    public class SmtpEmailer
    {
        private static SmtpClient SetupSmtpClient(StmpConnection connection)
        {
            SmtpClient client = new SmtpClient(connection.Host, connection.Port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(connection.FromEmail, connection.Password),
                EnableSsl = true,
                Timeout = 300000,
                DeliveryMethod = SmtpDeliveryMethod.Network
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

        public static void SendHtmlEmailWithAttachment(
            StmpConnection connection,
            HtmlEmailDefinition emailDefinition,
            string attachmentText)
        {
            using (MemoryStream stream = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(stream))    // using UTF-8 encoding by default
            using (SmtpClient mailClient = new SmtpClient(connection.Host, connection.Port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(connection.FromEmail, connection.Password),
                EnableSsl = true,
                Timeout = 300000,
                DeliveryMethod = SmtpDeliveryMethod.Network
            })
            using (MailMessage message = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress(connection.FromEmail, emailDefinition.FromEmailDisplayName),
                Subject = emailDefinition.Subject,
                Body = emailDefinition.BodyHtml
            })
            {
                writer.WriteLine(attachmentText);
                writer.Flush();
                stream.Position = 0;     // read from the start of what was written

                message.Attachments.Add(new Attachment(stream, "books.csv", "text/csv"));
                message.To.Add(new MailAddress(emailDefinition.ToEmail, emailDefinition.ToEmailDisplayName));

                mailClient.Send(message);
            }
        }
    }
}