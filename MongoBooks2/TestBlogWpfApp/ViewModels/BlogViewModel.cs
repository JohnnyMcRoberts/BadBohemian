namespace TestBlogWpfApp.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;

    public class BlogViewModel : INotifyPropertyChanged
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


        /// <summary>Gets the post view models. Updating this collection should be from the UI thread.</summary>
        public ObservableCollection<PostViewModel> Posts { get; private set; }

        private string id;
        /// <summary>Gets or sets the blog id.</summary>
        public string Id
        {
            get { return id; }
            set
            {
                Id = value;
                OnPropertyChanged(() => Id);
            }
        }

        private string name;
        /// <summary>Gets or sets the blog name.</summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(() => Name);
            }
        }

        private readonly MainViewModel repository;

        public BlogViewModel(MainViewModel repository)
        {
            this.repository = repository;
            Posts = new ObservableCollection<PostViewModel>();
        }


        public BlogViewModel(MainViewModel repository, string id, string name)
        {
            this.id = id;
            this.name = name;
            this.repository = repository;
            Posts = new ObservableCollection<PostViewModel>();
        }

        internal void RefreshPosts()
        {
            Task.Run(async () => await RefreshPostsAsync());
        }

        /// <summary>
        /// Asynchronously refreshes the posts. It calls the repository get posts method in order to refill the
        /// <see cref="Posts"/> collection.
        /// </summary>
        internal async Task RefreshPostsAsync()
        {
            var posts = await repository.GetPostsAsync(id);
            Posts.Clear();
            foreach (var p in posts)
            {
                Posts.Add(new PostViewModel
                {
                    Title = p.Title,
                    Content = p.Content
                });
            }
        }
    }
}
