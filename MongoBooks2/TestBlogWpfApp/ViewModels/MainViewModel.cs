namespace TestBlogWpfApp.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Windows.Input;
    using System.Threading.Tasks;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Data;

    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Blogger.v3;
    using Google.Apis.Services;


    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Blogger.v3;
    using Google.Apis.Services;
    using System.IO;

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

        #region Private Data

        /// <summary>
        /// The e-mail address to read from.
        /// </summary>
        private string _emailAddress;

        /// <summary>
        /// The title to display.
        /// </summary>
        private string _title;

        /// <summary>
        /// The print command.
        /// </summary>
        private ICommand _getBlogsCommand;


        private UserCredential credential;
        private BloggerService service;


        #endregion

        #region Public Data

        /// <summary>
        /// Gets the title of the app.
        /// </summary>
        public string Title => _title;

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
                OnPropertyChanged(() => EmailAdress);
            }
        }

        public ObservableCollection<Google.Apis.Blogger.v3.Data.Blog> Blogs { get; set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            _title = "Blogger Test";
            //_emailAddress = string.Empty;

            Blogs = new ObservableCollection<Google.Apis.Blogger.v3.Data.Blog>();
        }

        #endregion // Constructors

        #region Commands

        /// <summary>
        /// Gets the select image for nation command.
        /// </summary>
        public ICommand GetBlogsCommand => _getBlogsCommand ??
                                            (_getBlogsCommand =
                                                    new CommandHandler(GetBlogsCommandAction, true));
        #endregion

        #region Command Actions

        /// <summary>
        /// The command action to set to get the blogs.
        /// </summary>
        public void GetBlogsCommandAction()
        {
            GetBlogsAsync();
        }

        #endregion

        #region Utility Functions

        private async Task AuthenticateAsync()
        {
            if (service != null)
                return;

            string file = @"C:\temp\client_id.json";
            UserCredential credential;
            var stream = new FileStream(file, FileMode.Open, FileAccess.Read);

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(20));
            CancellationToken ct = cts.Token;

            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.Load(stream).Secrets,
            new[] { BloggerService.Scope.BloggerReadonly },
            "user",
            ct
            );

            //credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            //    new Uri("ms-appx:///Assets/client_secrets.json"),
            //    new[] { BloggerService.Scope.BloggerReadonly },
            //    "user",
            //    CancellationToken.None);

            var initializer = new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "BloggerApp",
            };

            service = new BloggerService(initializer);
        }

        public async Task RepositoryGetBlogsAsync()
        {
            await AuthenticateAsync();
            
            var list = await service.Blogs.ListByUser("self").ExecuteAsync();


            Blogs.Clear();
            if (list.Items == null)
            {
                return;
            }

            var blogs = from blog in list.Items
                   select new Google.Apis.Blogger.v3.Data.Blog
                   {
                       Id = blog.Id,
                       Name = blog.Name
                   };

            foreach (var blog in blogs)
                Blogs.Add(blog);
        }

        /// <summary>Asynchronously gets all the blogs.</summary>
        public async void GetBlogsAsync()
        {
            await RepositoryGetBlogsAsync();
            

#if not_await
            // Fill the blogs collection should be from the thread that created the collection.
            //await UIUtils.InvokeFromUIThread(() =>
            {
                Blogs.Clear();
                foreach (var b in blogs)
                {
                    Blogs.Add(new BlogViewModel(repository)
                    {
                        Name = b.Name,
                        Id = b.Id
                    });
                }
            });
#endif
        }

#endregion

    }
}
