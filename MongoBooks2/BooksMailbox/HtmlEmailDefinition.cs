namespace BooksMailbox
{
    public class HtmlEmailDefinition
    {
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
        
        /// <summary>
        /// The e-mail subject text.
        /// </summary>
        public string Subject { get; set; }
        
        /// <summary>
        /// The e-mail body HTML.
        /// </summary>
        public string BodyHtml { get; set; }
    }
}
