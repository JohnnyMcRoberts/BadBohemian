namespace WpfEmbeddedBrowser.ViewModels
{
    using CefSharp;
    using HtmlAgilityPack;

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;
    using System.Windows.Input;

    public class WpfEmbeddedBrowserMainViewModel : INotifyPropertyChanged
    {
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

        #region INotifyPropertyChanged Members

        void OnPropertyChanged<T>(Expression<Func<T>> sExpression)
        {
            if (sExpression == null) throw new ArgumentNullException(nameof(sExpression));

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
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members

        #region Constants

        /// <summary>
        /// The default web address.
        /// </summary>
        private const string DefaultWebAddress = "http://www.safexone.com/images/old/default.png";

        /// <summary>
        /// The term to build up the search string from.
        /// </summary>
        private const string DefaultImageAddress = "http://www.safexone.com/images/old/default.png";

        #endregion

        #region Private Data

        /// <summary>
        /// The web browser.
        /// </summary>
        private static IWebBrowser _browser;

        /// <summary>
        /// The result of the dialog.
        /// </summary>
        private bool? _dialogResult;

        /// <summary>
        /// The browser web address.
        /// </summary>
        private string _browserAddress = DefaultWebAddress;

        /// <summary>
        /// The term to build up the search string from.
        /// </summary>
        private string _searchTerm;

        /// <summary>
        /// The address of the web browser.
        /// </summary>
        private string _webAddress;

        /// <summary>
        /// The URI for the browser.
        /// </summary>
        private Uri _findImageUrl;

        /// <summary>
        /// The images shown on the page as URI and string pairs.
        /// </summary>
        private ObservableCollection<ImageDetail> _imagesOnPage;

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
        /// The update search item in the brower command.
        /// </summary>
        private ICommand _updateSearchTermCommand;

        /// <summary>
        /// The update images command.
        /// </summary>
        private ICommand _updateImagesCommand;

        /// <summary>
        /// The navigate back command.
        /// </summary>
        private ICommand _navigationBackCommand;

        /// <summary>
        /// The navigate forward command.
        /// </summary>
        private ICommand _navigationForwardCommand;

        /// <summary>
        /// Whether can navigate back.
        /// </summary>
        private static bool _canNavigateBack;

        /// <summary>
        /// Whether can navigate forward.
        /// </summary>
        private static bool _canNavigateForward;

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

        /// <summary>
        /// Gets whether can navigate back.
        /// </summary>
        public bool CanNavigateBack => _canNavigateBack;

        /// <summary>
        /// Gets whether can navigate forward.
        /// </summary>
        public bool CanNavigateForward => _canNavigateForward;

        /// <summary>
        /// Gets or sets the loaded document and updates the list of images accordingly.
        /// </summary>
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

        /// <summary>
        /// Get/set current address of web browser URI.
        /// </summary>
        public string BrowserAddress
        {
            get
            {
                return _browserAddress;
            }
            set
            {
                if (_browserAddress != value)
                {
                    _browserAddress = value;
                    OnPropertyChanged(() => BrowserAddress);
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the navigate forward command.
        /// </summary>
        public ICommand NavigationForwardCommand => 
            _navigationForwardCommand ?? (_navigationForwardCommand = new CommandHandler(NavigationForwardCommandAction, true));

        /// <summary>
        /// Gets the navigate back command.
        /// </summary>
        public ICommand NavigationBackCommand => 
            _navigationBackCommand ?? (_navigationBackCommand = new CommandHandler(NavigationBackCommandAction, true));

        /// <summary>
        /// Gets the complete selection command.
        /// </summary>
        public ICommand CompleteSelectionCommand =>
            _completeSelectionCommand ?? (_completeSelectionCommand = new CommandHandler(CompleteSelectionCommandAction, true));

        /// <summary>
        /// Gets the update search item in the brower command.
        /// </summary>
        public ICommand UpdateSearchTermCommand =>
            _updateSearchTermCommand ?? (_updateSearchTermCommand = new CommandHandler(UpdateSearchTermCommandAction, true));

        /// <summary>
        /// Gets the update images command.
        /// </summary>
        public ICommand UpdateImagesCommand =>
            _updateImagesCommand ?? (_updateImagesCommand = new CommandHandler(UpdateImagesCommandAction, true));

        #endregion

        #region Command Handers

        /// <summary>
        /// The navigate forward command handler.
        /// </summary>
        private void NavigationForwardCommandAction()
        {
            _browser?.Forward();
        }

        /// <summary>
        /// The navigate back command handler.
        /// </summary>
        private void NavigationBackCommandAction()
        {
            _browser?.Back();
        }

        /// <summary>
        /// The update images from the web page.
        /// </summary>
        private void UpdateImagesCommandAction()
        {
            try
            {
                _imagesOnPage = new ObservableCollection<ImageDetail>();
                HtmlWeb web = new HtmlWeb();

                HtmlDocument htmlDoc = web.Load(_browserAddress);

                HtmlNodeCollection collection = htmlDoc.DocumentNode.SelectNodes("//img");

                if (collection != null && collection.Count > 0)
                {
                    foreach (HtmlNode node in collection)
                    {
                        if (node.Attributes == null || node.Attributes.Count == 0)
                        {
                            continue;
                        }

                        foreach (HtmlAttribute attribute in node.Attributes)
                        {
                            if (attribute.Name.ToLower() != "src" || string.IsNullOrEmpty(attribute.Value))
                            {
                                continue;
                            }

                            Uri imageUri;
                            try
                            {
                                imageUri = new Uri(attribute.Value);
                            }
                            catch (Exception)
                            {
                                continue;
                            }


                            _imagesOnPage.Add(new ImageDetail { WebAddress = attribute.Value, Image = imageUri });

                        }

                    }
                }

                _imagesInPage = _imagesInPage.Distinct().ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Exception loading images \n" + e);
            }

            OnPropertyChanged(() => ImagesOnPage);
        }

        /// <summary>
        /// The update search item in the brower command.
        /// Builds up a google search address for the search term and navigates the browser to there.
        /// </summary>
        private void UpdateSearchTermCommandAction()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return;

            string fixedInput = Regex.Replace(SearchTerm, "[^a-zA-Z0-9% ._]", string.Empty);
            string[] splitWords = fixedInput.Split(' ');

            string address = "https://www.google.com/search?q=";
            for (int i = 0; i < splitWords.Length; i++)
            {
                if (i != 0)
                {
                    address += "+";
                }

                address += splitWords[i];
            }
            address += "&amp;tbm=isch";
            BrowserAddress = address;
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


        #endregion

        #region Public Methods

        /// <summary>
        /// Registers the browser so can navigate forward and back.
        /// </summary>
        /// <param name="browser"></param>
        public static void RegisterTestResources(IWebBrowser browser)
        {
            _browser = browser;
        }

        public void LoadingStateChanged(LoadingStateChangedEventArgs loadingStateChangedEventArgs)
        {
            _canNavigateBack = loadingStateChangedEventArgs.CanGoBack;
            _canNavigateForward = loadingStateChangedEventArgs.CanGoForward;

            OnPropertyChanged(() => CanNavigateBack);
            OnPropertyChanged(() => CanNavigateForward);
        }

        #endregion
    }
}
