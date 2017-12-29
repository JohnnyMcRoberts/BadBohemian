// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailboxLoaderViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The mailbox loader view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MongoDbBooks.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Windows.Input;

    using HtmlBuilders;
    using mshtml;
    using Newtonsoft.Json;
    
    using MongoDbBooks.ViewModels.Utilities;

    /// <summary>
    /// The image selection view model.
    /// </summary>
    public class ImageSelectionViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="expression">
        /// The string from the function expression.
        /// </param>
        /// <typeparam name="T">The type that has changed</typeparam>
        protected void OnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(@"expression");
            }

            MemberExpression body = expression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Body must be a member expression");
            }

            this.OnPropertyChanged(body.Member.Name);
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        /// <summary>
        /// The log.
        /// </summary>
        private log4net.ILog _log;

        /// <summary>
        /// The term to build up the search string from.
        /// </summary>
        private const string DefaultImageAddress = "http://www.safexone.com/images/old/default.png";

        /// <summary>
        /// The term to build up the search string from.
        /// </summary>
        private string _searchTerm;

        /// <summary>
        /// The address of the web browser.
        /// </summary>
        private string _webAddress;

        /// <summary>
        /// The loaded HTML document as a string.
        /// </summary>
        private string _theHtml;

        /// <summary>
        /// The URI for the browser.
        /// </summary>
        private Uri _findImageUrl;

        /// <summary>
        /// The loaded HTML document.
        /// </summary>
        private HTMLDocument _loadedDocument ;

        /// <summary>
        /// The images shown on the page as URI and string pairs.
        /// </summary>
        private readonly ObservableCollection<ImageDetail> _imagesOnPage;

        /// <summary>
        /// The selected image and web address pair.
        /// </summary>
        private ImageDetail _selectedDetail;

        /// <summary>
        /// The list of image web addresses displayed in the page.
        /// </summary>
        private List<string> _imagesInPage;

        /// <summary>
        /// The complete selection command.
        /// </summary>
        private ICommand _completeSelectionCommand;

        /// <summary>
        /// The browser navigate to page command.
        /// </summary>
        private ICommand _goToPageNavigationCommand;

        /// <summary>
        /// The update search item in the brower command.
        /// </summary>
        private ICommand _updateSearchTermCommand;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the search term to use for the image search.
        /// </summary>
        public string SelectedImageAddress => _selectedDetail == null ? "N/A" : _selectedDetail.WebAddress;

        /// <summary>
        /// Gets or sets the search term to use for the image search.
        /// </summary>
        public Uri SelectedImageUri => _selectedDetail == null ? new Uri("pack://application:,,,/Images/default.png") : _selectedDetail.Image;

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

                OnPropertyChanged(() => SelectedImageUri);
                OnPropertyChanged(() => SelectedImageAddress);
            }
        }

        /// <summary>
        /// Gets the image details for what's in the browser.
        /// </summary>
        public ObservableCollection<ImageDetail> ImagesOnPage => _imagesOnPage;

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
            }
        }

        /// <summary>
        /// Gets or sets the text of the .
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
        /// Gets or sets the search term to use for the image search.
        /// </summary>
        public Uri FindImageUrl
        {
            get { return _findImageUrl; }
            set
            {
                _findImageUrl = value;
                OnPropertyChanged(() => FindImageUrl);
            }
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

        #endregion

        #region Utility Functions

        private void GetImagesInPage()
        {
            try
            {
                LoadImagesFromDocument();

                if (TheHtml != string.Empty)
                {
                    HtmlTag tag = HtmlTag.Parse(TheHtml); // as there should always be a root tag

                    GetImagesFromImgTags(tag);

                    GetImagesFromDivTags(tag);
                }

                _imagesInPage = _imagesInPage.Distinct().ToList();
            }
            catch (Exception)
            {

            }
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

                _imagesOnPage.Add(new ImageDetail { WebAddress = imageAddress, Image = imageUri });
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

        #region Commands

        /// <summary>
        /// Gets the complete selection command.
        /// </summary>
        public ICommand CompleteSelectionCommand
        {
            get
            {
                return _completeSelectionCommand ??
                    (_completeSelectionCommand =
                        new CommandHandler(() => CompleteSelectionCommandAction(), true));
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

        #region Command Handers

        /// <summary>
        /// The browser navigate to page command - TBD.
        /// </summary>
        private void GoToPageNavigationCommandAction()
        {
            WebAddress = "https://www.google.com/search?q=Stefan+Zweig&amp;tbm=isch";
        }

        /// <summary>
        /// The update search item in the brower command.
        /// Builds up a google search address for the search term and navigates the browser to there.
        /// </summary>
        /// <param name="webAddress">The web address to use for the image search if not null.</param>
        private void UpdateSearchTermCommandAction(string webAddress = null)
        {
            if (webAddress == null)
            {
                WebAddress = "https://www.google.com/search?q=" + _searchTerm + "&amp;tbm=isch";
            }
            else
            {
                WebAddress = webAddress;
            }
        }

        /// <summary>
        /// The update search item in the brower command.
        /// Builds up a google search address for the search term and navigates the browser to there.
        /// </summary>
        private void CompleteSelectionCommandAction()
        {
            // Close the View.
            DialogResult = true;
        }


        public bool? DialogResult
        {
            get
            {
                return _dialogResult;
            }
            set
            {
                _dialogResult = value;
                OnPropertyChanged(() => DialogResult);
            }
        }

        private bool? _dialogResult;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSelectionViewModel"/> class.
        /// </summary>
        /// <param name="log">The log to write debug etc to.</param>
        /// <param name="searchTerm">The search term to use for the image search.</param>
        /// <param name="webAddress">The web address to use for the image search if not null.</param>
        public ImageSelectionViewModel(log4net.ILog log, string searchTerm, string webAddress = null)
        {
            _log = log;
            _searchTerm = searchTerm;
            UpdateSearchTermCommandAction(webAddress);
            
            _theHtml = "Dummy TextBox";
            _imagesOnPage = new ObservableCollection<ImageDetail>();
            _selectedDetail = new ImageDetail
            {
                Image = new Uri(DefaultImageAddress)
            };
        }

        #endregion
    }
}
