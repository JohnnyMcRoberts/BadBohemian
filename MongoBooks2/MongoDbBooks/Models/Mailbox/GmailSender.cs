// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GmailSender.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The send via gmail class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MongoDbBooks.Models.Mailbox
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;

    public class GmailSender
    {
        #region Public Data

        public string MessageText { get; set; }

        public string SourceEmail { get; set; }

        public string SourcePassword { get; set; }

        public string DestinationEmail { get; set; }

        #endregion

        #region Utility Methods

        public bool SendEmail(List<string> attachmentFiles, out string errorMessage)
        {
            bool sentEmail = true;
            errorMessage = string.Empty;
            try
            {
                string emailText = GetEmailTextForMessage();

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(SourceEmail);
                    mail.To.Add(DestinationEmail);
                    mail.Subject = "Export Books";
                    mail.Body = emailText;
                    mail.IsBodyHtml = true;

                    foreach (Attachment attachFile in attachmentFiles.Select(attachmentFile => new Attachment(attachmentFile)))
                    {
                        mail.Attachments.Add(attachFile);
                    }

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

        #endregion

        #region Public Methods

        private string GetEmailTextForMessage()
        {
            string emailText = "<h1>Export Books Notes</h1> <div>";

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
            return emailText;
        }

        #endregion

    }
}
