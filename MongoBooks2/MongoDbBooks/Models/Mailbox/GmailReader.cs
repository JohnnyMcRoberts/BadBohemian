// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GmailReader.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   Defines the GmailReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MongoDbBooks.Models.Mailbox
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ActiveUp.Net.Mail;

    /// <summary>
    /// The gmail reader.
    /// </summary>
    public class GmailReader : IMailReader
    {
        #region Constants

        /// <summary>
        /// The IMAP port.
        /// </summary>
        private const int ImapPort = 993;

        /// <summary>
        /// The IMAP server address.
        /// </summary>
        private const string ImapServerAddress = "imap.gmail.com";

        /// <summary>
        /// The standard mailbox name.
        /// </summary>
        private const string StandardMailBoxName = "INBOX";

        /// <summary>
        /// The day names.
        /// </summary>
        private readonly string[] _dayNames =
        {
            "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"
        };

        /// <summary>
        /// The month names.
        /// </summary>
        private readonly string[] _monthNames =
        {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "Novemeber", "December",
        };

        /// <summary>
        /// The name for the reader.
        /// </summary>
        private readonly string _readerName = "Gmail Mailbox Reader";

        #endregion

        #region Private data

        /// <summary>
        /// The email address to read books from.
        /// </summary>
        private string _emailAddress;

        /// <summary>
        /// The password to connect to the email with.
        /// </summary>
        private string _password;

        #endregion

        #region Utility functions

        /// <summary>
        /// The get totals from counts.
        /// </summary>
        /// <param name="counts">The counts.</param>
        /// <param name="ttlPages">The total pages.</param>
        /// <param name="ttlAudio">The total audio.</param>
        /// <param name="ttlComics">The total comics.</param>
        /// <param name="ttlBooks">The total books.</param>
        /// <param name="ttlRead">The total read.</param>
        /// <returns>
        /// True if extracted the totals Ok, false otherwise.
        /// </returns>
        private static bool GetTotalsFromCounts(string counts, out ulong ttlPages, out ushort ttlAudio, out ushort ttlComics, out ushort ttlBooks, out ushort ttlRead)
        {
            ttlPages = ttlAudio = ttlComics = ttlBooks = ttlRead = 0;

            // eg is it of the form
            //
            // 643 (587 - 47 - 9) [169923]
            //  ==>
            //  [0] = 643
            //  [1] = (587
            //  [2] = -
            //  [3] = 47
            //  [4] = -
            //  [5] = 9)
            //  [6] = [169923]

            // as occasionally miss out the spaces around the dashes pad them
            string padded = counts.Replace("-", " - ");

            char[] delimiterChars = { ' ' };
            string[] words = padded.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
            int numWords = words.Length;

            if (numWords < 7)
            {
                return false;
            }

            // ttlRead
            if (!ushort.TryParse(words[0], out ttlRead))
            {
                return false;
            }

            // ttlBooks = "(580"
            if (words[1][0] != '(')
            {
                return false;
            }

            if (!ushort.TryParse(words[1].Substring(1), out ttlBooks))
            {
                return false;
            }

            if (words[2][0] != '-')
            {
                return false;
            }

            // ttlComics = "46"
            if (!ushort.TryParse(words[3], out ttlComics))
            {
                return false;
            }

            if (words[4][0] != '-')
            {
                return false;
            }

            // ttlAudio = "9)"
            if (words[5][words[5].Length - 1] != ')')
            {
                return false;
            }

            if (!ushort.TryParse(words[5].Remove(1), out ttlAudio))
            {
                return false;
            }

            // ttlPages = "[167941]"
            if (words[6][0] != '[')
            {
                return false;
            }

            if (words[6][words[6].Length - 1] != ']')
            {
                return false;
            }

            string trimmed = words[6].Substring(1, words[6].Length - 2);
            if (!ulong.TryParse(trimmed, out ttlPages))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The extract counts from end of line.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="counts">The counts.</param>
        /// <returns>
        /// True if a valid counts section, false otherwise.
        /// </returns>
        private static bool ExtractCountsFromEndOfLine(ref string section, ref string counts)
        {
            // eg is it of the form
            //
            // ABC DEF ZYX 643 (587 - 47 - 9) [169923]
            //  ==>
            //  [0..n-8] = ABC DEF ZYX
            //  [n-7] = 643
            //  [n-6] = (587
            //  [n-5] = -
            //  [n-4] = 47
            //  [n-3] = -
            //  [n-2] = 9)
            //  [n-1] = [169923]
            // as occasionally miss out the spaces around the dashes pad them
            string padded = section.Replace("-", " - ");

            // as occasionally mess up the bit around the brackets replace any odd chars with spaces
            // should use regex for this...
            //string pattern = @"^).\[$";
            padded = Regex.Replace(padded, @"[^\w\[\]\(\)\-]", " ");

            //Regex regex = new Regex(@"^.\[$");
            //padded = regex.Replace(padded, " [");
            char[] delimiterChars = { ' ' };
            string[] words = padded.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
            int numWords = words.Length;

            if (numWords < 8)
            {
                return false;
            }

            int tempInt;
            if (!int.TryParse(words[numWords - 7], out tempInt))
            {
                return false;
            }

            if (words[numWords - 6][0] != '(')
            {
                return false;
            }

            if (words[numWords - 5][0] != '-')
            {
                return false;
            }

            if (!int.TryParse(words[numWords - 4], out tempInt))
            {
                return false;
            }

            if (words[numWords - 3][0] != '-')
            {
                return false;
            }

            if (words[numWords - 2][words[numWords - 2].Length - 1] != ')')
            {
                return false;
            }

            if (words[numWords - 1][0] != '[')
            {
                return false;
            }

            if (words[numWords - 1][words[numWords - 1].Length - 1] != ']')
            {
                return false;
            }

            section = string.Empty;
            for (int i = 0; i < (numWords - 7); i++)
            {
                if (i > 0)
                {
                    section += " ";
                }

                section += words[i];
            }

            counts = string.Empty;
            for (int j = numWords - 7; j < numWords; j++)
            {
                if (j > (numWords - 7))
                {
                    counts += " ";
                }

                counts += words[j];
            }

            return true;
        }

        /// <summary>
        /// Checks if the line of text is a valid book read line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>True if valid, false otherwise.</returns>
        private static bool IsBookReadLine(string line)
        {
            // eg is it 
            // Monday 23rd January 2017 :- Augustín Fernández Mallo :- Nocilla Experience :- 205
            string[] delimiterStrs = { " :- " };
            string[] words = line.Split(delimiterStrs, StringSplitOptions.RemoveEmptyEntries);

            return words.Length > 3;
        }

        /// <summary>
        /// Checks if is a book tally line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>True if it is a tally line, false otherwise</returns>
        private static bool IsBookTallyLine(string line)
        {
            // eg is it of the form
            //
            // 643 (587 - 47 - 9) [169923]
            //  ==>
            //  [0] = 643
            //  [1] = (587
            //  [2] = -
            //  [3] = 47
            //  [4] = -
            //  [5] = 9)
            //  [6] = [169923]
            char[] delimiterChars = { ' ' };
            string[] words = line.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length < 7)
            {
                return false;
            }

            int tempInt;
            if (!int.TryParse(words[0], out tempInt))
            {
                return false;
            }

            if (words[1][0] != '(')
            {
                return false;
            }

            if (words[2][0] != '-')
            {
                return false;
            }

            if (!int.TryParse(words[3], out tempInt))
            {
                return false;
            }

            if (words[4][0] != '-')
            {
                return false;
            }

            if (words[5][words[5].Length - 1] != ')')
            {
                return false;
            }

            if (words[6][0] != '[')
            {
                return false;
            }

            if (words[6][words[6].Length - 1] != ']')
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The read emails using the standard IMAP v4 interface.
        /// </summary>
        /// <param name="errorMessage">The error Message.</param>
        /// <returns>True if connected ok.</returns>
        private bool ConnectToMailboxUsingStdImap4(out string errorMessage)
        {
            errorMessage = string.Empty;
            string selectedMailBox = StandardMailBoxName;

            using (var clientImap4 = new Imap4Client())
            {
                try
                {
                    clientImap4.ConnectSsl(ImapServerAddress, ImapPort);
                    clientImap4.Login(_emailAddress, _password);

                    Mailbox mailbox = clientImap4.SelectMailbox(selectedMailBox);

                    if (mailbox.Permission != MailboxPermission.ReadWrite)
                    {
                        errorMessage = "Mailbox permission is " + mailbox.Permission;
                    }

                }
                catch (Exception e)
                {
                    errorMessage = e.Message + " : " + e.InnerException;
                }
            }

            return string.IsNullOrEmpty(errorMessage);
        }

        /// <summary>
        /// Reads the emails using the standard IMAP v4 interface.
        /// </summary>
        /// <param name="errorMessage">The error Message.</param>
        /// <param name="books">The books read if successful.</param>
        /// <returns>True if read books ok.</returns>
        private bool ReadEmailsUsingStdImap4(out string errorMessage, out List<IBookRead> books)
        {
            string selectedMailBox = StandardMailBoxName;
            books = null;

            using (var clientImap4 = new Imap4Client())
            {
                try
                {
                    clientImap4.ConnectSsl(ImapServerAddress, ImapPort);
                    clientImap4.Login(_emailAddress, _password); // Make log in and load all MailBox.

                    Mailbox mailbox = clientImap4.SelectMailbox(selectedMailBox);

                    string messageItemText = string.Empty;
                    foreach (var messageId in mailbox.Search("ALL").AsEnumerable().OrderByDescending(x => x))
                    {
                        var message = mailbox.Fetch.Message(messageId);
                        var imapMessage = Parser.ParseMessage(message);
                        messageItemText +=
                            "messageId: " + messageId +
                            " From: " + imapMessage.From.Name +
                            " (" + imapMessage.From.Email + ")" +
                            ", Subject: " + imapMessage.Subject + "\n";

                        List<string> messageLines = new List<string>();
                        if (imapMessage.Subject.ToLower().Contains("tallies"))
                        {
                            messageItemText += "\n";

                            string text = imapMessage.BodyText.Text;
                            List<string> bookEntries;
                            if (!GetBookEntriesFromPlainText(text, out bookEntries))
                            {
                                continue;
                            }

                            if (bookEntries.Count > 0)
                            {
                                bookEntries.Reverse();

                                List<BookReadFromGmail> emailedBooks = GetEmailedBooksRead(bookEntries);
                                messageItemText = bookEntries.Aggregate(messageItemText, (current, book) => current + (book + "\n"));
                                messageLines = bookEntries;

                                if (emailedBooks != null && emailedBooks.Count > 0)
                                {
                                    books = emailedBooks.Cast<IBookRead>().ToList();
                                }

                                if (books != null && books.Count > 0)
                                {
                                    errorMessage = string.Empty;
                                    return true;
                                }
                            }
                            else
                            {
                                messageItemText = HandleInvalidMessageData(text, messageItemText, messageLines);
                            }

                            messageItemText += "\n";
                        }

                        if (messageLines.Count > 0)
                        {
                            // Got the latest stop.
                            break;
                        }
                    }

                    errorMessage = messageItemText;
                    clientImap4.Disconnect();
                }
                catch (Exception e)
                {
                    errorMessage = e.Message + " : " + e.InnerException;
                }
            }

            return books != null;
        }

        /// <summary>
        /// This gets the book entries from a block of plain text.
        /// </summary>
        /// <param name="text"> The text. </param>
        /// <param name="bookEntries">The book entries.</param>
        /// <returns>
        /// True if got books from the text, false otherwise.
        /// </returns>
        private bool GetBookEntriesFromPlainText(string text, out List<string> bookEntries)
        {
            string[] delimiterStrs = { "\r\n\r\n" };
            string[] blocks = text.Split(delimiterStrs, StringSplitOptions.RemoveEmptyEntries);

            if (blocks.Length < 1)
            {
                bookEntries = new List<string>();
                return false;
            }

            List<string> betterBlocks = new List<string>();
            foreach (string block in blocks)
            {
                if (_dayNames.Any(day => block.ToLower().StartsWith(day.ToLower())))
                {
                    betterBlocks.Add(block.Replace("\r\n", " "));
                }
            }

            betterBlocks = SplitOutMultipleBooksInSingleBlock(betterBlocks);

            bookEntries = new List<string>();
            foreach (var block in betterBlocks)
            {
                string date = string.Empty;
                string author = string.Empty;
                string title = string.Empty;
                string pages = string.Empty;
                string counts = string.Empty;
                if (IsBookReadLine(block, ref date, ref author, ref title, ref pages, ref counts))
                {
                    bookEntries.Add(block);
                }
            }

            return true;
        }

        /// <summary>
        /// This splits out blocks where there is more than one book in a single block.
        /// </summary>
        /// <param name="blocks">The list of text blocks.</param>
        /// <returns>
        /// True if got books from the text, false otherwise.
        /// </returns>
        private List<string> SplitOutMultipleBooksInSingleBlock(List<string> blocks)
        {
            List<string> updatedBlocks = new List<string>();
            foreach (var block in blocks)
            {
                bool splitBlock = false;
                List<string> splitBlocks = new List<string>();
                foreach (string dayName in _dayNames)
                {
                    // if there are 2 entries in the one block ie ... [12345] Monday 1st January 2012 :- An Author : a book ...
                    // split that into 2 items and replace in the blocks list
                    string daySplit = "] " + dayName;
                    if (block.Contains(daySplit))
                    {
                        string[] splitterStrings = { daySplit };
                        string[] items = block.Split(splitterStrings, StringSplitOptions.RemoveEmptyEntries);
                        splitBlock = true;

                        List<string> splitParts = new List<string> { items[0] + "]" };
                        for (int i = 1; i < items.Length; i++)
                        {
                            string daySection = dayName + items[i].TrimEnd();
                            if (!daySection.EndsWith("]"))
                            {
                                daySection += "]";
                            }

                            splitParts.Add(daySection);
                        }

                        splitBlocks.AddRange(splitParts);
                    }
                }

                if (splitBlock == false)
                {
                    updatedBlocks.Add(block);
                }
                else
                {
                    updatedBlocks.AddRange(splitBlocks);
                }
            }

            blocks = updatedBlocks;
            return blocks;
        }

        /// <summary>
        /// Check if a book read line.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="date">The date.</param>
        /// <param name="author">The author.</param>
        /// <param name="title">The title.</param>
        /// <param name="pages">The pages.</param>
        /// <param name="counts">The counts.</param>
        /// <returns>
        /// True if a valid book read entry block, false otherwise.
        /// </returns>
        private bool IsBookReadLine(string block, ref string date, ref string author, ref string title, ref string pages, ref string counts)
        {
            // eg is it 
            // Monday 23rd January 2017 :- Augustín Fernández Mallo :- Nocilla Experience :- 205
            string[] delimiterStrs = { " :- " };
            string[] section = block.Split(delimiterStrs, StringSplitOptions.RemoveEmptyEntries);

            if (section.Length < 3)
            {
                return false;
            }

            // must be at least 3 starting with {date} :- {author} :- {title} & ending with {counts} eg 643 (587 - 47 - 9) [169923]
            date = section[0];
            author = section[1];

            if (section.Length == 3)
            {
                title = section[2];
                pages = null;
                if (!ExtractCountsFromEndOfLine(ref title, ref counts))
                {
                    return false;
                }
            }
            else
            {
                title = section[2];
                pages = section[3];
                if (!ExtractCountsFromEndOfLine(ref pages, ref counts))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The get emailed books read.
        /// </summary>
        /// <param name="bookEntries">The book entries.</param>
        /// <returns>The list of books read that can be extracted the list of strings.</returns>
        private List<BookReadFromGmail> GetEmailedBooksRead(IEnumerable<string> bookEntries)
        {
            List<BookReadFromGmail> emailedBooks = bookEntries.Select(GetEmailedBookFromEntry).ToList();

            if (emailedBooks.Count <= 1)
            {
                return emailedBooks;
            }

            for (int i = 1; i < emailedBooks.Count; i++)
            {
                emailedBooks[i].SetupPreviousBook(emailedBooks[i - 1]);
            }

            return emailedBooks;
        }

        /// <summary>
        /// The get emailed book from text block.
        /// </summary>
        /// <param name="bookReadText">The book read text.</param>
        /// <returns>The book read.</returns>
        private BookReadFromGmail GetEmailedBookFromEntry(string bookReadText)
        {
            BookReadFromGmail newBook = new BookReadFromGmail();

            string date = string.Empty;
            string author = string.Empty;
            string title = string.Empty;
            string pages = string.Empty;
            string counts = string.Empty;
            if (IsBookReadLine(bookReadText, ref date, ref author, ref title, ref pages, ref counts))
            {
                newBook.Date = GetDateFromString(date);
                newBook.DateString = date;
                newBook.Author = author;
                newBook.Title = title;
                ushort numPages;
                if (ushort.TryParse(pages, out numPages))
                {
                    newBook.Pages = numPages;
                }

                ulong ttlPages;
                ushort ttlAudio, ttlComics, ttlBooks, ttlRead;

                if (GetTotalsFromCounts(counts, out ttlPages, out ttlAudio, out ttlComics, out ttlBooks, out ttlRead))
                {
                    newBook.TotalPages = ttlPages;

                    newBook.TotalRead = ttlRead;
                    newBook.TotalBooks = ttlBooks;
                    newBook.TotalComics = ttlComics;
                    newBook.TotalAudio = ttlAudio;
                }
            }

            return newBook;
        }

        /// <summary>
        /// The get date from string.
        /// </summary>
        /// <param name="date">The date string.</param>
        /// <returns>The Date structure the books was read.</returns>
        private DateTime GetDateFromString(string date)
        {
            int day = DateTime.Today.Day;
            int month = DateTime.Today.Month;
            int year = DateTime.Today.Year;

            char[] delimiterChars = { ' ' };
            string[] words = date.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);

            // this should get ..
            // words[0] = "Wednesday"
            // words[1] = "4th"
            // words[2] = "January"
            // words[3] = "2017"

            // initial check is that everything is there...
            if (words.Length == 4)
            {
                if (words[1].Length >= 3)
                {
                    // remove the 'st', 'nd' 'rd' or 'th' at the end of the day of the month string
                    string dayStr = words[1].Remove(words[1].Length - 2);
                    if (!int.TryParse(dayStr, out day))
                    {
                        return DateTime.Today;
                    }
                }

                bool foundMonth = false;
                for (int i = 0; i < _monthNames.Length; i++)
                {
                    if (words[2].ToLower().Equals(_monthNames[i].ToLower()))
                    {
                        month = 1 + i;
                        foundMonth = true;
                        break;
                    }
                }

                if (!foundMonth)
                {
                    return DateTime.Today;
                }

                if (!int.TryParse(words[3], out year))
                {
                    return DateTime.Today;
                }
            }

            return new DateTime(year, month, day);
        }

        /// <summary>
        /// The handle invalid message data.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="messageItemText">
        /// The message item text.
        /// </param>
        /// <param name="messageLines">
        /// The message lines.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string HandleInvalidMessageData(string text, string messageItemText, ICollection<string> messageLines)
        {
            bool haveBookReadAwaitingTally = false;

            using (StringReader reader = new StringReader(text))
            {
                string line;
                int lineCount = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    // Do something with the line
                    messageItemText += line + "\n";
                    lineCount++;

                    if (lineCount <= 4)
                    {
                        messageItemText += "...";
                    }

                    if (IsBookReadLine(line))
                    {
                        messageLines.Add(line);
                        haveBookReadAwaitingTally = true;
                    }

                    if (haveBookReadAwaitingTally && IsBookTallyLine(line))
                    {
                        messageLines.Add(line);
                        haveBookReadAwaitingTally = false;
                    }
                }
            }

            return messageItemText;
        }

        #endregion 

        #region Implementation of IMailReader 

        /// <summary>
        /// Gets the reader name.
        /// </summary>
        public string ReaderName => _readerName;

        /// <summary>
        /// The validate the email and password.
        /// </summary>
        /// <param name="emailAddress"> The email address.</param>
        /// <param name="password"> The password.</param>
        /// <returns>
        /// True if valid for the mailbox type, false otherwise.
        /// </returns>
        public bool ValidateEmailAndPassword(string emailAddress, string password)
        {
            return emailAddress.ToLower().Contains("gmail") || emailAddress.ToLower().Contains("googlemail");
        }

        /// <summary>
        /// The read books from mail.
        /// </summary>
        /// <param name="emailAddress"> The email address.</param>
        /// <param name="password"> The password.</param>
        /// <param name="errorMessage"> The error message if fails.</param>
        /// <param name="books"> The books if read and parsed OK.</param>
        /// <returns>
        /// True if got the books read from the mailbox, if false an error is provided.
        /// </returns>
        public bool ReadBooksFromMail(
            string emailAddress, 
            string password, 
            out string errorMessage, 
            out ObservableCollection<IBookRead> books)
        {
            _emailAddress = emailAddress;
            _password = password;
            List<IBookRead> emailedBooks;
            bool result = ReadEmailsUsingStdImap4(out errorMessage, out emailedBooks);
            books = (emailedBooks == null) ? 
                new ObservableCollection<IBookRead>() : 
                new ObservableCollection<IBookRead>(emailedBooks);
            return result;
        }

        /// <summary>
        /// Tries to connect to a mailbox.
        /// </summary>
        /// <param name="emailAddress"> The email address.</param>
        /// <param name="password"> The password.</param>
        /// <param name="errorMessage"> The error message if fails.</param>
        /// <returns>
        /// True if got connected to the mailbox, if false an error is provided.
        /// </returns>
        public bool ConnectToMailbox(
            string emailAddress,
            string password,
            out string errorMessage)
        {
            _emailAddress = emailAddress;
            _password = password;
            return ConnectToMailboxUsingStdImap4(out errorMessage);
        }

        #endregion
        }
}
