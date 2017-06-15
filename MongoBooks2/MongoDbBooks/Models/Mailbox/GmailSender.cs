using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbBooks.Models.Mailbox
{
    using System.Net;
    using System.Net.Mail;

    public class GmailSender
    {
        public void SendEmail()
        {
            try
            {
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("username", "password");
                smtp.EnableSsl = true;
                smtp.Send("sender@gamil.com", "receiver", "subject", "Email Body");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
