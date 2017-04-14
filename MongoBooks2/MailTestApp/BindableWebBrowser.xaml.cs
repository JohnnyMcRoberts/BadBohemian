// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindableWebBrowser.xaml.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The Interaction logic for BindableWebBrowser.xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace MailTestApp
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Navigation;
    using mshtml;

    /// <summary>
    /// Interaction logic for <see cref="BindableWebBrowser" />
    /// </summary>
    public partial class BindableWebBrowser
    {
        private const string SkipSourceChange = "Skip";

        private string _docHtml = "No HTML Loaded";

        private HTMLDocument _loadedHtmlDocument;

        public BindableWebBrowser()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseBack, BrowseBack, CanBrowseBack));
            CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseForward, BrowseForward, CanBrowseForward));
            CommandBindings.Add(new CommandBinding(NavigationCommands.Refresh, Refresh, TrueCanExecute));
        }

        public string BindableSource
        {
            get { return (string)GetValue(BindableSourceProperty); }
            set { SetValue(BindableSourceProperty, value); }
        }

        public string DocumentHtml
        {
            get { return (string)GetValue(DocumentHtmlProperty); }
            set { SetValue(DocumentHtmlProperty, value); }
        }

        public string DocHtml
        {
            get { return _docHtml; }
            set { _docHtml = value; SetValue(DocHtmlProperty, value); }
        }

        public HTMLDocument LoadedHtmlDocument
        {
            get { return _loadedHtmlDocument; }
            set { _loadedHtmlDocument = value; SetValue(LoadedHtmlDocumentProperty, value); }
        }

        public bool ShouldHandleNavigated
        {
            get { return (bool)GetValue(ShouldHandleNavigatedProperty); }
            set { SetValue(ShouldHandleNavigatedProperty, value); }
        }

        public static readonly DependencyProperty BindableSourceProperty =
                                                        DependencyProperty.RegisterAttached(
                                                                "BindableSource",
                                                                typeof(string),
                                                                typeof(BindableWebBrowser));

        public static readonly DependencyProperty DocumentHtmlProperty =
                                                        DependencyProperty.RegisterAttached(
                                                                "DocumentHtml",
                                                                typeof(string),
                                                                typeof(BindableWebBrowser));

        public static readonly DependencyProperty DocHtmlProperty =
                                                        DependencyProperty.RegisterAttached(
                                                                "DocHtml",
                                                                typeof(string),
                                                                typeof(BindableWebBrowser));

        public static readonly DependencyProperty LoadedHtmlDocumentProperty =
                                                        DependencyProperty.RegisterAttached(
                                                                "LoadedHtmlDocument",
                                                                typeof(HTMLDocument),
                                                                typeof(BindableWebBrowser));

        public static readonly DependencyProperty ShouldHandleNavigatedProperty =
                                                        DependencyProperty.RegisterAttached(
                                                                "ShouldHandleNavigated",
                                                                typeof(Boolean),
                                                                typeof(BindableWebBrowser));

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == BindableSourceProperty)
            {
                BindableSourcePropertyChanged(e);
            }
            else if(e.Property == DocumentHtmlProperty)
            {
                DocumentHtmlPropertyChanged(e);
            }
            else if (e.Property == ShouldHandleNavigatedProperty)
            {
                ShouldHandleNavigatedPropertyChanged(e);
            }
        }

        public void ShouldHandleNavigatedPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (browser != null)
            {
                if ((bool)e.NewValue)
                {
                    browser.Navigated += new NavigatedEventHandler(Browser_Navigated);
                }
                else
                {
                    browser.Navigated -= new NavigatedEventHandler(Browser_Navigated);
                }
            }
        }

        public void DocumentHtmlPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (browser != null)
            {
                if (browser.Document != null)
                    DocumentHtml = browser.Document.ToString();
                //string uri = e.NewValue as string;
                //if (!_SkipSourceChange.Equals(browser.Tag))
                //{
                //    browser.Source = !string.IsNullOrEmpty(uri) ? new Uri(uri) : null;

                //    //Or, if you prefer...
                //    //webBrowser.NavigateToString((string)e.NewValue); 
                //}
            }
        }

        public void BindableSourcePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (browser != null)
            {
                string uri = e.NewValue as string;
                if (!SkipSourceChange.Equals(browser.Tag))
                {
                    browser.LoadCompleted += WebBrowserLoaded;
                    browser.Source = !string.IsNullOrEmpty(uri) ? new Uri(uri) : null;

                    //Or, if you prefer...
                    //webBrowser.NavigateToString((string)e.NewValue); 
                }
            }
        }

        public void WebBrowserLoaded(object sender, NavigationEventArgs e)
        {
            WebBrowser webBrowser = sender as WebBrowser;
            var htmlDocument = webBrowser?.Document as HTMLDocument;

            if (htmlDocument != null)
            {
                LoadedHtmlDocument = htmlDocument;
                string docHtml = htmlDocument.body.outerHTML;
                DocumentHtml = docHtml;
                DocHtml = docHtml;
            }
        }

        private void Browser_Navigated(object sender, NavigationEventArgs e)
        {
            WebBrowser webBrowser = sender as WebBrowser;
            if (webBrowser != null)
            {
                if (BindableSource != e.Uri.ToString())
                {
                    webBrowser.Tag = SkipSourceChange;
                    BindableSource = webBrowser.Source.AbsoluteUri;
                    webBrowser.Tag = null;
                }
            }
        }

        private void CanBrowseBack(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = browser.CanGoBack;
        }

        private void BrowseBack(object sender, ExecutedRoutedEventArgs e)
        {
            browser.GoBack();
        }

        private void CanBrowseForward(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = browser.CanGoForward;
        }

        private void BrowseForward(object sender, ExecutedRoutedEventArgs e)
        {
            browser.GoForward();
        }

        private void TrueCanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }

        private void Refresh(object sender, ExecutedRoutedEventArgs e)
        {
            try { browser.Refresh(); }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}
