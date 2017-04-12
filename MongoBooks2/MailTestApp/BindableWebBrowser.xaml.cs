using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MailTestApp
{
    using mshtml;

    /// <summary>
    /// Interaction logic for BindableWebBrowser.xaml
    /// </summary>
    public partial class BindableWebBrowser : UserControl
    {
        private const string _SkipSourceChange = "Skip";

        private string _docHtml = "No HTML Loaded";

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
                if (!_SkipSourceChange.Equals(browser.Tag))
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
                string docHtml = htmlDocument.body.outerHTML;
                DocumentHtml = docHtml;
                DocHtml = docHtml;
            }
        }

        private void Browser_Navigated(object sender, NavigationEventArgs e)
        {
            WebBrowser browser = sender as WebBrowser;
            if (browser != null)
            {
                if (BindableSource != e.Uri.ToString())
                {
                    browser.Tag = _SkipSourceChange;
                    this.BindableSource = browser.Source.AbsoluteUri;
                    browser.Tag = null;
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
