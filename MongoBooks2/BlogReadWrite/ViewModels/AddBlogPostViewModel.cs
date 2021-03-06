﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddBlogPostViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The add blog post view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogReadWrite.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;

    using BlogReadWrite.Models;
    using BlogReadWrite.Utilities;

    /// <summary>
    /// The add blog post view model class.
    /// </summary>
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

        /// <summary>
        /// Gets or sets the title for the blog post.
        /// </summary>
        public string BlogPostTitle { get; set; }

        /// <summary>
        /// Gets or sets the content for the blog post.
        /// </summary>
        public string BlogPostContent { get; set; }

        /// <summary>
        /// Gets or sets the title for the window.
        /// </summary>
        public string WindowTitle { get; set; }

        /// <summary>
        /// Gets or sets the calling application name.
        /// </summary>
        public string ApplictionName { get; set; }

        /// <summary>
        /// Gets the client secret to use for the blogger.
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

        /// <summary>
        /// Gets or sets the default secret file name.
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether there is a default secret file name already set up.
        /// </summary>
        public bool IsDefaultSecretFileName => !string.IsNullOrEmpty(DefaultSecretFileName);

        /// <summary>
        /// Gets a value indicating whether the secret file has been set up.
        /// </summary>
        public bool HaveSecretFile => !string.IsNullOrEmpty(SecretFileName);

        /// <summary>
        /// Gets a value indicating whether enough data is provided to add a new blog post.
        /// </summary>
        public bool IsDataForNewPost => !string.IsNullOrEmpty(SecretFileName) &&
             !string.IsNullOrEmpty(BlogPostTitle) &&
             !string.IsNullOrEmpty(BlogPostContent) &&
             SelectedBlog != null;

        /// <summary>
        /// Gets a value indicating whether enough data is provided to update an existing blog post.
        /// </summary>
        public bool IsDataForUpdatePost => IsDataForNewPost && SelectedPost != null;

        /// <summary>
        /// Gets the blogs.
        /// </summary>
        public ObservableCollection<Google.Apis.Blogger.v3.Data.Blog> Blogs => _blogRepostory.Blogs;

        /// <summary>
        /// Gets the selected blog posts.
        /// </summary>
        public ObservableCollection<Google.Apis.Blogger.v3.Data.Post> BlogPosts => _blogRepostory.BlogPosts;

        /// <summary>
        /// Gets or sets the selected blog.
        /// </summary>
        public Google.Apis.Blogger.v3.Data.Post SelectedPost
        {
            get
            {
                return _selectedPost;
            }

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
            get
            {
                return _selectedBlog;
            }

            set
            {
                _selectedBlog = value;
                if (_selectedBlog != null)
                {
                    SelectedPost = null;
                    using (new WaitCursor())
                    {
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(
                            async () => { await _blogRepostory.GetBlogPosts(_selectedBlog.Id); });
                    }

                    OnPropertyChanged(() => BlogPosts);
                    OnPropertyChanged(() => IsDataForNewPost);
                    OnPropertyChanged(() => IsDataForUpdatePost);
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

        /// <summary>
        /// The selected blog.
        /// </summary>
        private Google.Apis.Blogger.v3.Data.Blog _selectedBlog;

        /// <summary>
        /// The selected blog post.
        /// </summary>
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

        /// <summary>
        /// The close window command.
        /// </summary>
        private ICommand _closeWindowCommand;

        /// <summary>
        /// The blog repository.
        /// </summary>
        private readonly BlogRepository _blogRepostory;

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

        /// <summary>
        /// Gets the select image for nation command.
        /// </summary>
        public ICommand CloseWindowCommand => _closeWindowCommand ??
                                            (_closeWindowCommand =
                                             new RelayCommandHandler(CloseWindow) { IsEnabled = true });

        #endregion

        #region Command Actions

        /// <summary>
        /// The command action to select the secret file.
        /// </summary>
        public async void SelectSecretFileCommandAction()
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
                OnPropertyChanged(string.Empty);

                using (new WaitCursor())
                {
                    await _blogRepostory.GetBlogs();
                }

                OnPropertyChanged(() => Blogs);
            }
        }

        /// <summary>
        /// The command action to set the secret file to the previously selected default.
        /// </summary>
        public async void SetDefaultSecretFileNameCommandAction()
        {
            SecretFileName = DefaultSecretFileName;
            _blogRepostory.SecretFileName = SecretFileName;
            using (new WaitCursor())
            {
                await _blogRepostory.GetBlogs();
            }

            OnPropertyChanged(() => Blogs);
        }

        /// <summary>
        /// The command action to set to get the blogs.
        /// </summary>
        public async void GetBlogsCommandAction()
        {
            using (new WaitCursor())
            {
                await _blogRepostory.GetBlogs();
            }

            OnPropertyChanged(() => Blogs);
        }

        /// <summary>
        /// The command action to add a new post to the blog.
        /// </summary>
        public async void NewBlogPostCommandAction()
        {
            using (new WaitCursor())
            {
                await _blogRepostory.AddBlogPost(BlogPostTitle, BlogPostContent, _selectedBlog.Id);
                await _blogRepostory.GetBlogPosts(_selectedBlog.Id);
            }

            OnPropertyChanged(() => BlogPosts);
        }

        /// <summary>
        /// The command action to update a blog post.
        /// </summary>
        public async void UpdateBlogPostCommandAction()
        {
            using (new WaitCursor())
            {
                await _blogRepostory.UpdateBlogPost(BlogPostTitle, BlogPostContent, _selectedBlog.Id, _selectedPost.Id);
                await _blogRepostory.GetBlogPosts(_selectedBlog.Id);
            }

            OnPropertyChanged(() => BlogPosts);
        }

        /// <summary>
        /// The command action to close the dialog window.
        /// </summary>
        /// <param name="parameter">The parameter for the window.</param>
        public void CloseWindow(object parameter)
        {
            Window window = parameter as Window;
            window?.Close();
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
            _selectedBlog = null;
            _selectedPost = null;
        }

        #endregion
    }
}
