namespace BooksMailbox
{
    public class StmpConnection
    {
        /// <summary>
        /// The default host is for gmail.
        /// </summary>
        private const string DefaultHost = "smtp.gmail.com";

        /// <summary>
        /// The default port.
        /// </summary>
        private const int DefaultPort = 587;
        
        /// <summary>
        /// The name of the mail host.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The port to connect to.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The name of the e-mail account to send from.
        /// </summary>
        public string FromEmail { get; set; }

        /// <summary>
        /// The password of the account to send from.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Initialize a new instance of <see cref="StmpConnection"/>.
        /// </summary>
        /// <param name="fromEmail">The e-mail account to send from.</param>
        /// <param name="password">The password of the e-mail account.</param>
        /// <param name="host">The e-mail host to use, by default gmail.</param>
        /// <param name="port">The port to send e-mails via, by default 587.</param>
        public StmpConnection(string fromEmail, string password, string host = DefaultHost, int port = DefaultPort)
        {
            FromEmail = fromEmail;
            Password = password;
            Host = host;
            Port = port;
        }
    }
}
