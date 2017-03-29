// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMailReader.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   The MailReader interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MongoDbBooks.Models.Mailbox
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The MailReader interface.
    /// </summary>
    public interface IMailReader
    {
        /// <summary>
        /// Gets the reader name.
        /// </summary>
        string ReaderName { get; }

        /// <summary>
        /// The validate the email and password.
        /// </summary>
        /// <param name="emailAddress"> The email address.</param>
        /// <param name="password"> The password.</param>
        /// <returns>
        /// True if valid for the mailbox type, false otherwise.
        /// </returns>
        bool ValidateEmailAndPassword(string emailAddress, string password);

        /// <summary>
        /// Tries to read books from mailbox.
        /// </summary>
        /// <param name="emailAddress"> The email address.</param>
        /// <param name="password"> The password.</param>
        /// <param name="errorMessage"> The error message if fails.</param>
        /// <param name="books"> The books if read and parsed OK.</param>
        /// <returns>
        /// True if got the books read from the mailbox, if false an error is provided.
        /// </returns>
        bool ReadBooksFromMail(
            string emailAddress,
            string password,
            out string errorMessage,
            out ObservableCollection<IBookRead> books);
    }
}
