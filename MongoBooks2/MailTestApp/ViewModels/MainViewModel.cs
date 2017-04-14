// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The view model for the mail box tester MainViewModel.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace MailTestApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Windows.Input;
    using System.Windows.Forms;

    using ActiveUp.Net.Mail;
    using HtmlBuilders;
    using mshtml;
    using Newtonsoft.Json;

    /// <summary>
    /// Lets a mailbox be contacted, or an image selected.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        void OnPropertyChanged<T>(Expression<Func<T>> sExpression)
        {
            if (sExpression == null) throw new ArgumentNullException("sExpression");

            MemberExpression body = sExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Body must be a member expression");
            }
            OnPropertyChanged(body.Member.Name);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members

        #region Nested classes

        /// <summary>
        /// Wrapper for an image an address as a string and an URI.
        /// </summary>
        public class ImageDetail
        {
            /// <summary>
            /// Gets or sets the address of the image.
            /// </summary>
            public string WebAddress { get; set; }

            /// <summary>
            /// Gets or sets the URI for the image.
            /// </summary>
            public Uri Image { get; set; }
        }

        #endregion 

        #region Private Data

        #region Mailbox data

        /// <summary>
        /// The e-mail address to read from.
        /// </summary>
        private string _emailAddress;

        /// <summary>
        /// The password for the e-mail address.
        /// </summary>
        private string _password;

        /// <summary>
        /// Whether have enough details to connect to the mailbox.
        /// </summary>
        private bool _isValidToConnect;

        /// <summary>
        /// The text of mailbox message.
        /// </summary>
        private string _mailItemsText;

        /// <summary>
        /// The port for the IMAP server.
        /// </summary>
        private const int ImapPort = 993;

        /// <summary>
        /// The IMAP server for Gmail.
        /// </summary>
        private const string ImapServerAddress = "imap.gmail.com";

        #endregion

        #region Image Search Details

        /// <summary>
        /// The address of the web browser.
        /// </summary>
        private string _webAddress;

        /// <summary>
        /// The term to build up the search string from.
        /// </summary>
        private string _searchTerm;

        /// <summary>
        /// The body of HTML of the browser page.
        /// </summary>
        private string _theHtml;

        /// <summary>
        /// The loaded HTML document.
        /// </summary>
        private HTMLDocument _loadedDocument;

        /// <summary>
        /// The URI for the browser.
        /// </summary>
        private Uri _findImageUrl;

        /// <summary>
        /// The list of image web addresses displayed in the page.
        /// </summary>
        private List<string> _imagesInPage;

        /// <summary>
        /// The images shown on the page as URI and string pairs.
        /// </summary>
        private readonly ObservableCollection<ImageDetail> _imagesOnPage;

        /// <summary>
        /// The selected image and web address pair.
        /// </summary>
        private ImageDetail _selectedDetail;

        #endregion

        #region Commands

        /// <summary>
        /// The read e-mail command.
        /// </summary>
        private ICommand _readEmailCommand;

        /// <summary>
        /// The browser navigate to page command.
        /// </summary>
        private ICommand _goToPageNavigationCommand;

        /// <summary>
        /// The update search item in the brower command.
        /// </summary>
        private ICommand _updateSearchTermCommand;

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the e-mail address to read from.
        /// </summary>
        public string EmailAdress
        {
            get
            {
                return _emailAddress;
            }
            set
            {
                _emailAddress = value;
                ValidateCredentials();
                OnPropertyChanged(() => EmailAdress);
            }
        }

        /// <summary>
        /// Gets or sets the password for the e-mail address.
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                ValidateCredentials();
                OnPropertyChanged(() => Password);
            }
        }

        /// <summary>
        /// Gets or sets the text of mailbox message.
        /// </summary>
        public string MailItemsText
        {
            get
            {
                return _mailItemsText;
            }
            set
            {
                _mailItemsText = value;
                OnPropertyChanged(() => MailItemsText);
            }
        }

        /// <summary>
        /// Gets a value indicating whether have enough details to connect to the mailbox.
        /// </summary>
        public bool IsValidToConnect
        {
            get
            {
                return _isValidToConnect;
            }
            private set
            {
                _isValidToConnect = value;
                OnPropertyChanged(() => IsValidToConnect);
            }
        }

        /// <summary>
        /// Gets or sets the browser URI.
        /// </summary>
        public Uri FindImageUrl
        {
            get { return _findImageUrl; }
            private set { _findImageUrl = value; OnPropertyChanged(() => FindImageUrl); }
        }

        /// <summary>
        /// Gets or sets the browser web address.
        /// </summary>
        public string WebAddress
        {
            get
            {
                return _webAddress;
            }
            set
            {
                _webAddress = value;
                FindImageUrl = new Uri(value);
                OnPropertyChanged(() => WebAddress);
            }
        }

        /// <summary>
        /// Gets or sets the search term to use for the image search.
        /// </summary>
        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                _searchTerm = value;
                OnPropertyChanged(() => SearchTerm);
            }
        }
        
        /// <summary>
        /// Gets or sets the loaded document HTML string.
        /// </summary>
        public string TheHtml
        {
            get { return _theHtml; }
            set
            {
                if (value != null)
                {
                    _theHtml = value;
                    GetImagesInPage();
                }
                else
                {
                    _theHtml = string.Empty;
                }

                OnPropertyChanged(() => TheHtml);
            }
        }

        /// <summary>
        /// Gets or sets the loaded document and updates the list of images accordingly.
        /// </summary>
        public HTMLDocument LoadedDocument
        {
            get
            {
                return _loadedDocument;
            }
            set
            {
                _loadedDocument = value;
                GetImagesInPage();
                OnPropertyChanged(() => LoadedDocument);
                OnPropertyChanged(() => DocumentTitle);
                OnPropertyChanged(() => ImagesInPage);
                OnPropertyChanged(() => LoadedHtml);
            }
        }

        /// <summary>
        /// Gets the loaded document title.
        /// </summary>
        public string DocumentTitle
        {
            get
            {
                if (_loadedDocument != null)
                {
                    return _loadedDocument.title + " : " + _loadedDocument.url;
                }

                return "Nothing loaded";
            }
        }

        /// <summary>
        /// Gets the list of strings containg the web addresses of the images on the page.
        /// </summary>
        public List<string> ImagesInPage => _imagesInPage;

        /// <summary>
        /// Gets the loaded document HTML as a string.
        /// </summary>
        public string LoadedHtml
        {
            get
            {
                if (_loadedDocument?.body?.innerHTML != null)
                {
                    return _loadedDocument.body.innerHTML;
                }

                return "Nothing loaded";
            }
        }

        /// <summary>
        /// Gets the image details for what's in the browser.
        /// </summary>
        public ObservableCollection<ImageDetail> ImagesOnPage => _imagesOnPage;

        /// <summary>
        /// Gets or sets the selected image detail.
        /// </summary>
        public ImageDetail SelectedImage
        {
            get
            {
                return _selectedDetail;
            }
            set
            {
                if (value != null && value != _selectedDetail)
                {
                    _selectedDetail = value;
                }

                OnPropertyChanged(() => MatchedImageUri);
            }
        }

        /// <summary>
        /// Gets the URI for the selected image.
        /// </summary>
        public Uri MatchedImageUri => _selectedDetail.Image;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            _emailAddress = string.Empty;
            _password = string.Empty;
            _isValidToConnect = false;
            WebAddress = "https://www.google.com/search?q=J.G. Ballard&amp;tbm=isch";

            _theHtml = "Dummy TextBox";
            _imagesOnPage = new ObservableCollection<ImageDetail>();
            _selectedDetail = new ImageDetail
            {
                Image = new Uri("http://img.timeoutshanghai.com/201304/20130401044423690_Medium.jpg")
            };
        }

        #endregion

        #region Commands 

        /// <summary>
        /// Gets the read e-mail command.
        /// </summary>
        public ICommand ReadEmailCommand
        {
            get
            {
                return _readEmailCommand ??
                    (_readEmailCommand =
                        new CommandHandler(() => ReadEmailCommandAction(), true));
            }
        }

        /// <summary>
        /// Gets the browser navigate to page command.
        /// </summary>
        public ICommand GoToPageNavigationCommand
        {
            get
            {
                return _goToPageNavigationCommand ??
                    (_goToPageNavigationCommand =
                        new CommandHandler(() => GoToPageNavigationCommandAction(), true));
            }
        }

        /// <summary>
        /// Gets the update search item in the brower command.
        /// </summary>
        public ICommand UpdateSearchTermCommand
        {
            get
            {
                return _updateSearchTermCommand ??
                    (_updateSearchTermCommand =
                        new CommandHandler(() => UpdateSearchTermCommandAction(), true));
            }
        }

        #endregion

        #region Command Handlers

        #region Mailbox

        /// <summary>
        /// The read e-mail command action.
        /// Uses the password and email to connect to the mailbox using IMAP v4 security and reads the e-mails.
        /// </summary>
        public void ReadEmailCommandAction()
        {
            string msg = "Getting Mail from '" + _emailAddress + "' with password '" + _password + "'";
            MessageBox.Show(msg);

            string selectedMailBox = "INBOX";

            using (var clientImap4 = new Imap4Client())
            {
                try
                {
                    clientImap4.ConnectSsl(ImapServerAddress, ImapPort);

                    // Make log in and load all MailBox.
                    clientImap4.Login(_emailAddress, _password);

                    var mailBox = clientImap4.SelectMailbox(selectedMailBox);

                    string messageItemText = string.Empty;
                    foreach (int messageId in mailBox.Search("ALL").AsEnumerable().OrderByDescending(x => x))
                    {
                        byte[] message = mailBox.Fetch.Message(messageId);
                        ActiveUp.Net.Mail.Message imapMessage = Parser.ParseMessage(message);

                        messageItemText +=
                        "From: " + imapMessage.From.Name + ", Subject: " + imapMessage.Subject + "\n";

                        if (imapMessage.Subject.ToLower().Contains("tallies"))
                        {
                            messageItemText += "\n";

                            messageItemText += imapMessage.BodyText.Text;

                            messageItemText += "\n";
                        }
                    }

                    MailItemsText = messageItemText;
                    clientImap4.Disconnect();
                }
                catch (Exception e)
                {
                    string error = e.Message + " : " + e.InnerException;
                    MessageBox.Show(error);
                }
            }
        }

        #endregion

        #region Image search

        /// <summary>
        /// The browser navigate to page command - TBD.
        /// </summary>
        private void GoToPageNavigationCommandAction()
        {
            MessageBox.Show(@"Go To Page");

            WebAddress = "https://www.google.com/search?q=Stefan+Zweig&amp;tbm=isch";
        }

        /// <summary>
        /// The update search item in the brower command.
        /// Builds up a google search address for the search term and navigates the browser to there.
        /// </summary>
        private void UpdateSearchTermCommandAction()
        {
            WebAddress = "https://www.google.com/search?q=" + _searchTerm + "&amp;tbm=isch";
        }

        #endregion

        #endregion

        #region Utility Functions

        /// <summary>
        /// Sets the valid to connect flag based on the e-mail address and password.
        /// </summary>
        private void ValidateCredentials()
        {
            IsValidToConnect = _password != null && _password.Length > 1 && _emailAddress != null && _emailAddress.Length > 3 && _emailAddress.Contains("@");
        }

        /// <summary>
        /// Gets the images from the browser html page ready to be displayed.
        /// </summary>
        private void GetImagesInPage()
        {
            LoadImagesFromDocument();

            if (TheHtml != string.Empty)
            {
                HtmlTag tag = HtmlTag.Parse(TheHtml); // as there should always be a root tag

                GetImagesFromImgTags(tag);

                GetImagesFromDivTags(tag);
            }

            _imagesInPage = _imagesInPage.Distinct().ToList();

            UpdateImagesOnPage();
        }

        /// <summary>
        /// Updates the address and URI pairs for the images from the strings.
        /// </summary>
        private void UpdateImagesOnPage()
        {
            _imagesOnPage.Clear();
            foreach (string imageAddress in _imagesInPage)
            {
                Uri imageUri;
                try
                {
                    imageUri = new Uri(imageAddress);
                }
                catch (Exception)
                {
                    continue;
                }

                _imagesOnPage.Add(new ImageDetail {WebAddress = imageAddress, Image = imageUri});
            }

            OnPropertyChanged(() => ImagesOnPage);
        }

        /// <summary>
        /// Gets the addresses for .png and .jpg files from "rg_meta" class "div" tags json.
        /// This is the way a top level Google Image search provides images.
        /// </summary>
        /// <param name="tag">The parsed HTML document tag.</param>
        private void GetImagesFromDivTags(HtmlTag tag)
        {
            try
            {
                // find looks recursively through the entire DOM tree
                IEnumerable<HtmlTag> inputFields = tag.Find(t => string.Equals(t.TagName, "div"));

                foreach (HtmlTag inputField in inputFields.Where(inputField => inputField.ContainsKey("class")))
                {
                    try
                    {
                        string divClass = inputField["class"];
                        if (divClass == "rg_meta")
                        {
                            if (inputField.Contents == null || !inputField.Contents.Any())
                            {
                                continue;
                            }

                            IHtmlElement contents = inputField.Contents.First();
                            Dictionary<string, string> jsonValues =
                                JsonConvert.DeserializeObject<Dictionary<string, string>>(contents.ToString());

                            if (!jsonValues.ContainsKey("ity"))
                            {
                                continue;
                            }

                            if (jsonValues["ity"] != "jpg" && jsonValues["ity"] != "png")
                            {
                                continue;
                            }

                            if (jsonValues.ContainsKey("ou"))
                            {
                                _imagesInPage.Add(jsonValues["ou"]);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Gets the addresses for .png and .jpg files from "irc_mi" class image tags.
        /// This is the way a detailed Google search provides images.
        /// </summary>
        /// <param name="tag">The parsed HTML document tag.</param>
        private void GetImagesFromImgTags(HtmlTag tag)
        {
            try
            {
                // find looks recursively through the entire DOM tree
                IEnumerable<HtmlTag> inputFields = tag.Find(t => string.Equals(t.TagName, "img"));

                foreach (HtmlTag inputField in inputFields.Where(inputField => inputField.ContainsKey("class")))
                {
                    string imgClass = inputField["class"];
                    try
                    {
                        if (imgClass == "irc_mi")
                        {
                            string imageSrc = inputField["src"];
                            if (imageSrc.ToLower().EndsWith(".jpg") || imageSrc.ToLower().EndsWith(".png"))
                            {
                                _imagesInPage.Add(imageSrc);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Gets the addresses for .png and .jpg files from "img" tags "src" field the html.
        /// This is the way wikipedia provides images.
        /// </summary>
        private void LoadImagesFromDocument()
        {
            _imagesInPage = new List<string> { "No Images Loaded" };

            if (_loadedDocument?.images != null)
            {
                _imagesInPage.Clear();

                foreach (IHTMLImgElement img in _loadedDocument.images)
                {
                    string imageSrc = img.src;
                    string imageLowSrc = img.lowsrc;
                    if (imageSrc != null &&
                        (imageSrc.ToLower().EndsWith(".jpg") || imageSrc.ToLower().EndsWith(".png") ||
                         imageSrc.ToLower().Contains(".jpg")))
                    {
                        _imagesInPage.Add(imageSrc);
                    }
                    else if (imageLowSrc != null &&
                             (imageLowSrc.ToLower().EndsWith(".jpg") || imageLowSrc.ToLower().EndsWith(".png")))
                    {
                        _imagesInPage.Add(imageLowSrc);
                    }
                }
            }
        }

        #endregion
    }
}
