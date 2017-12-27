namespace BlogReadWrite.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq.Expressions;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Linq;
    using System.Threading;
    using System.Collections.ObjectModel;

    using BlogReadWrite.Utilities;
    using BlogReadWrite.Models;

    public class AddBlogPostViewModel : INotifyPropertyChanged
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

        #region Public Data

        public string BlogPostTitle { get; set; }

        public string BlogPostContent { get; set; }

        public string WindowTitle { get; set; }

        public string ApplictionName { get; set; }

        /// <summary>
        /// Gets or sets the client secret to use for the blogger.
        /// </summary>
        public string SecretFileName
        {
            get
            {
                return _secretFileName;
            }
            private set
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
            private set
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
             !string.IsNullOrEmpty(BlogPostTitle) &&
             !string.IsNullOrEmpty(BlogPostContent) &&
             SelectedBlog != null;

        public bool IsDataForUpdatePost => IsDataForNewPost && SelectedPost != null;

        public ObservableCollection<Google.Apis.Blogger.v3.Data.Blog> Blogs => _blogRepostory.Blogs;

        public ObservableCollection<Google.Apis.Blogger.v3.Data.Post> BlogPosts => _blogRepostory.BlogPosts;

        /// <summary>Gets or sets the selected blog.</summary>
        public Google.Apis.Blogger.v3.Data.Post SelectedPost
        {
            get { return _selectedPost; }
            set
            {
                _selectedPost = value;
                OnPropertyChanged(() => SelectedPost);
                OnPropertyChanged(() => IsDataForUpdatePost);
            }
        }

        /// <summary>Gets or sets the selected blog.</summary>
        public Google.Apis.Blogger.v3.Data.Blog SelectedBlog
        {
            get { return _selectedBlog; }
            set
            {
                _selectedBlog = value;
                if (_selectedBlog != null)
                {
                    SelectedPost = null;
                    _blogRepostory.GetBlogPosts(_selectedBlog.Id).Wait();
                    OnPropertyChanged(() => BlogPosts);
                }
            }
        }

        #endregion

        #region Private Data

        /// <summary>
        /// The default client secret file.
        /// </summary>
        private string _defaultSecretFileName;

        /// <summary>
        /// The client secret file.
        /// </summary>
        private string _secretFileName;
       
        private Google.Apis.Blogger.v3.Data.Blog _selectedBlog;

        private Google.Apis.Blogger.v3.Data.Post _selectedPost;

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

        /// <summary>
        /// The add a new blog post command.
        /// </summary>
        private ICommand _newBlogPostCommand;

        /// <summary>
        /// The update existing blog post command.
        /// </summary>
        private ICommand _updateBlogPostCommand;

        private BlogRepository _blogRepostory;

        #endregion

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

        /// <summary>
        /// Gets the available blogs command.
        /// </summary>
        public ICommand NewBlogPostCommand => _newBlogPostCommand ??
                                            (_newBlogPostCommand =
                                                    new CommandHandler(NewBlogPostCommandAction, true));

        /// <summary>
        /// Gets the available blogs command.
        /// </summary>
        public ICommand UpdateBlogPostCommand => _updateBlogPostCommand ??
                                            (_updateBlogPostCommand =
                                                    new CommandHandler(UpdateBlogPostCommandAction, true));

        #endregion

        #region Command Actions

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
                _blogRepostory.SecretFileName = SecretFileName;
                OnPropertyChanged("");
            }
        }

        public void SetDefaultSecretFileNameCommandAction()
        {
            SecretFileName = DefaultSecretFileName;
            _blogRepostory.SecretFileName = SecretFileName;
        }

        /// <summary>
        /// The command action to set to get the blogs.
        /// </summary>
        public async void GetBlogsCommandAction()
        {
            await _blogRepostory.GetBlogs();
            OnPropertyChanged(() => Blogs);
        }

        public async void NewBlogPostCommandAction()
        {
            await _blogRepostory.AddBlogPost(BlogPostTitle, BlogPostContent, _selectedBlog.Id);
            await _blogRepostory.GetBlogs();
            OnPropertyChanged(() => Blogs);
        }

        public async void UpdateBlogPostCommandAction()
        {
            await _blogRepostory.UpdateBlogPost(BlogPostTitle, BlogPostContent, _selectedBlog.Id, _selectedPost.Id);
            await _blogRepostory.GetBlogPosts(_selectedBlog.Id);
            OnPropertyChanged(() => BlogPosts);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AddBlogPostViewModel" /> class.
        /// </summary>
        public AddBlogPostViewModel()
        {
            _defaultSecretFileName = Properties.Settings.Default.DefaultSecretFileName;
            _blogRepostory = new BlogRepository();
        }

        #endregion

    }
}
