namespace WebRequestTester.Models
{
    public class TestEmailParameters
    {
        /// <summary>
        /// The name of the e-mail account to send from.
        /// </summary>
        public string FromEmail { get; set; }

        /// <summary>
        /// The password of the account to send from.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The name of the e-mail account to send to.
        /// </summary>
        public string ToEmail { get; set; }

        /// <summary>
        /// The display name of the e-mail account to send to.
        /// </summary>
        public string ToEmailDisplayName { get; set; }

        /// <summary>
        /// The display name of the e-mail account to send to.
        /// </summary>
        public string FromEmailDisplayName { get; set; }
    }
}
