using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbBooks.Models.Mailbox
{
    using System.IO;
    using System.Net;
    using System.Net.Mail;

    public class GmailSender
    {
        public bool SendEmail(out string errorMessage)
        {
            bool sentEmail = true;
            errorMessage = string.Empty;
            try
            {
                string emailText = "<div>";

                using (StringReader sr = new StringReader(MessageText))
                {
                    string line;
                    int lineCount = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        emailText += WebUtility.HtmlEncode(line);

                        if (lineCount > 0)
                        {
                            emailText += "<br>";
                        }

                        lineCount++;
                    }
                }

                emailText += "</div>";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(SourceEmail);
                    mail.To.Add(DestinationEmail);
                    mail.Subject = "Export Books";
                    mail.Body = "<h1>Export Books Notes</h1>" + emailText;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(SourceEmail, SourcePassword);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = @" Mailing Exception = " + ex.Message;
                sentEmail = false;
            }

            return sentEmail;
        }

        public string MessageText { get; set; }
        public string SourceEmail { get; set; }
        public string SourcePassword { get; set; }
        public string DestinationEmail { get; set; }
    }
}
