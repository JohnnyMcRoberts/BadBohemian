namespace TestBlogWpfApp.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Windows.Input;
    using System.Threading.Tasks;
    using System.Windows.Forms;    
    using System.Linq;
    using System.Threading;
    using System.Collections.ObjectModel;
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
    using Google.GData.Client;

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
        /// The default client secret file.
        /// </summary>
        private string _defaultSecretFileName;

        /// <summary>
        /// The client secret file.
        /// </summary>
        private string _secretFileName;

        /// <summary>
        /// The title to display.
        /// </summary>
        private string _title;

        /// <summary>
        /// The get blogs command.
        /// </summary>
        private ICommand _getBlogsCommand;

        /// <summary>
        /// The open client secret file command.
        /// </summary>
        private ICommand _selectSecretFileCommand;

        /// <summary>
        /// The set secret file from the default command.
        /// </summary>
        private ICommand _setDefaultSecretFileNameCommand;

        private ICommand _newBlogPostCommandCommand;

        private UserCredential credential;
        private BloggerService service;


        private string _newPostTitle;
        private string _newPostContent;

        private Google.Apis.Blogger.v3.Data.Blog _selectedBlog;

        private BlogViewModel _selectedBlogModel;
        #endregion

        #region Public Data

        /// <summary>
        /// Gets the title of the app.
        /// </summary>
        public string Title => _title;

        public string NewPostTitle
        {
            get { return _newPostTitle; }
            set
            {
                _newPostTitle = value;
                OnPropertyChanged(() => NewPostTitle);
                OnPropertyChanged(() => IsDataForNewPost);
            }
        }
        public string NewPostContent
        {
            get { return _newPostContent; }
            set
            {
                _newPostContent = value;
                OnPropertyChanged(() => NewPostContent);
                OnPropertyChanged(() => IsDataForNewPost);
            }
        }

        /// <summary>
        /// Gets or sets the client secret to use for the blogger.
        /// </summary>
        public string SecretFileName
        {
            get
            {
                return _secretFileName;
            }
            set
            {
                _secretFileName = value;
                OnPropertyChanged(() => SecretFileName);
                OnPropertyChanged(() => HaveSecretFile);
            }
        }

        public string DefaultSecretFileName
        {
            get
            {
                return _defaultSecretFileName;
            }
            set
            {
                if (_defaultSecretFileName != value)
                {
                    _defaultSecretFileName = value;
                    Properties.Settings.Default.DefaultSecretFileName = _defaultSecretFileName;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public bool IsDefaultSecretFileName => !string.IsNullOrEmpty(DefaultSecretFileName);

        public bool HaveSecretFile => !string.IsNullOrEmpty(SecretFileName);

        public bool IsDataForNewPost => !string.IsNullOrEmpty(SecretFileName) &&
             !string.IsNullOrEmpty(NewPostTitle) &&
             !string.IsNullOrEmpty(NewPostContent) &&
             SelectedBlog != null;

        public ObservableCollection<Google.Apis.Blogger.v3.Data.Blog> Blogs { get; set; }

        public ObservableCollection<PostViewModel> Posts { get
            {
                if (SelectedBlogModel == null)
                    return new ObservableCollection<PostViewModel>();
                else
                    return SelectedBlogModel.Posts;
                        
            } }

        /// <summary>Gets or sets the selected blog.</summary>
        public Google.Apis.Blogger.v3.Data.Blog SelectedBlog
        {
            get { return _selectedBlog; }
            set
            {
                _selectedBlog = value;
                if (_selectedBlog != null)
                {
                    SelectedBlogModel = new BlogViewModel(this, _selectedBlog.Id, _selectedBlog.Name);
                    RepositoryGetBlogPosts(_selectedBlog.Id);
                }
            }
        }

        public ObservableCollection<Google.Apis.Blogger.v3.Data.Post> BlogPosts { get; set; }

        public BlogViewModel SelectedBlogModel
        {
            get { return _selectedBlogModel; }
            set
            {
                _selectedBlogModel = value;
                if (_selectedBlogModel != null)
                    GetBlogPostsAsync();
                OnPropertyChanged(() => Posts);
                OnPropertyChanged(string.Empty);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            _title = "Blogger Test";
            _defaultSecretFileName = Properties.Settings.Default.DefaultSecretFileName;

            Blogs = new ObservableCollection<Google.Apis.Blogger.v3.Data.Blog>();
            BlogPosts = new ObservableCollection<Google.Apis.Blogger.v3.Data.Post>();
        }

        #endregion // Constructors

        #region Commands

        /// <summary>
        /// Gets the available blogs command.
        /// </summary>
        public ICommand GetBlogsCommand => _getBlogsCommand ??
                                            (_getBlogsCommand =
                                                    new CommandHandler(GetBlogsCommandAction, true));

        /// <summary>
        /// Select the secret file command.
        /// </summary>
        public ICommand SelectSecretFileCommand => _selectSecretFileCommand ??
                                            (_selectSecretFileCommand =
                                                    new CommandHandler(SelectSecretFileCommandAction, true));

        /// <summary>
        /// Gets the available blogs command.
        /// </summary>
        public ICommand SetDefaultSecretFileNameCommand => _setDefaultSecretFileNameCommand ??
                                            (_setDefaultSecretFileNameCommand =
                                                    new CommandHandler(SetDefaultSecretFileNameCommandAction, true));

        public ICommand NewBlogPostCommand => _newBlogPostCommandCommand ??
                                            (_newBlogPostCommandCommand =
                                                    new CommandHandler(NewBlogPostCommandAction, true));


        #endregion       

        #region Command Actions

        /// <summary>
        /// The command action to set to get the blogs.
        /// </summary>
        public void GetBlogsCommandAction()
        {
            GetBlogsAsync();
        }

        public void SelectSecretFileCommandAction()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = @"All files (*.*)|*.*|JSON Files (*.json)|*.json";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                SecretFileName = fileDialog.FileName;
                DefaultSecretFileName = fileDialog.FileName;
                OnPropertyChanged("");
            }
        }

        public void SetDefaultSecretFileNameCommandAction()
        {
            SecretFileName = DefaultSecretFileName;
        }

        public async void NewBlogPostCommandAction()
        {
            await RepositoryAddBlogPost(_newPostTitle, _newPostContent);
        }

        #endregion

        #region Utility Functions

        private async Task AuthenticateAsync()
        {
            if (service != null)
                return;
            
            UserCredential credential;
            var stream = new FileStream(_secretFileName, FileMode.Open, FileAccess.Read);

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(20));
            CancellationToken ct = cts.Token;

            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                new[] { BloggerService.Scope.Blogger },
                "user",
                ct
                );

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

            OnPropertyChanged(() => Blogs);
        }

        /// <summary>Asynchronously gets all the posts from a blog.</summary>
        public async void GetBlogPostsAsync()
        {
            _selectedBlogModel.RefreshPostsAsync();

            OnPropertyChanged(() => Posts);
        }

        public async Task RepositoryGetBlogPosts(string blogId)
        {
            await AuthenticateAsync();
            var list = await service.Posts.List(blogId).ExecuteAsync();
            BlogPosts.Clear();
            var posts = from post in list.Items
                   select new Google.Apis.Blogger.v3.Data.Post
                   {
                       Title = post.Title,
                       Content = post.Content
                   };

            foreach (var post in posts)
                BlogPosts.Add(post);
        }

        public async Task RepositoryAddBlogPost(string title, string content)
        {
            string blogId = _selectedBlog.Id;
            await AuthenticateAsync();
            

            var newPost = new Google.Apis.Blogger.v3.Data.Post()
            {
                Kind = "blogger#post",
                Blog = new Google.Apis.Blogger.v3.Data.Post.BlogData() { Id = blogId },
                Title = title,
                Content = "<div xmlns='http://www.w3.org/1999/xhtml'>" +
              "<p>Mr. Darcy has <em>proposed marriage</em> to me!</p>" +
              "<p>He is the last man on earth I would ever desire to marry.</p>" +
              "<p>Whatever shall I do?</p>" +
              "<p>" + content + "</p>" +
              "</div>"
            };

            try
            {
                var insertRequest = service.Posts.Insert(newPost, blogId);
                await insertRequest.ExecuteAsync();
                
            }
            catch(Exception e)
            {
                Console.Write(e);
            }

            
        }

        public async Task<IEnumerable<Google.Apis.Blogger.v3.Data.Post>> GetPostsAsync(string blogId)
        {
            await AuthenticateAsync();
            var list = await service.Posts.List(blogId).ExecuteAsync();
            return from post in list.Items
                   select new Google.Apis.Blogger.v3.Data.Post
                   {
                       Title = post.Title,
                       Content = post.Content
                   };
        }

        #endregion

    }
}
