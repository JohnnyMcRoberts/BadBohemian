// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlogRepository.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The blog repository model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogReadWrite.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Blogger.v3;
    using Google.Apis.Blogger.v3.Data;
    using Google.Apis.Services;

    /// <summary>
    /// The blog repository model class.
    /// </summary>
    public class BlogRepository
    {
        #region Private data

        /// <summary>
        /// The user credential.
        /// </summary>
        private UserCredential _credential;

        /// <summary>
        /// The blogger service.
        /// </summary>
        private BloggerService _service;

        #endregion

        #region Public Data

        /// <summary>
        /// Gets or sets the name of the application to display.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the secret file name.
        /// </summary>
        public string SecretFileName { get; set; }

        /// <summary>
        /// Gets the list of blogs to display.
        /// </summary>
        public ObservableCollection<Blog> Blogs { get; }

        /// <summary>
        /// Gets the list of blog posts to display.
        /// </summary>
        public ObservableCollection<Post> BlogPosts { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogRepository" /> class.
        /// </summary>
        public BlogRepository()
        {
            Blogs = new ObservableCollection<Blog>();
            BlogPosts = new ObservableCollection<Post>();
        }

        #endregion

        #region Public functions

        public async Task GetBlogs()
        {
            await AuthenticateAsync();

            var list = await _service.Blogs.ListByUser("self").ExecuteAsync();

            Blogs.Clear();
            if (list.Items == null)
            {
                return;
            }

            var blogs = from blog in list.Items
                        select new Blog
                        {
                            Id = blog.Id,
                            Name = blog.Name
                        };

            foreach (var blog in blogs)
            {
                Blogs.Add(blog);
            }
        }

        public async Task GetBlogPosts(string blogId)
        {
            await AuthenticateAsync();
            var request = _service.Posts.List(blogId);
            request.MaxResults = 100;
            var list = await request.ExecuteAsync();

            BlogPosts.Clear();
            var posts = from post in list.Items
                        select new Post
                        {
                            Title = post.Title,
                            Content = post.Content,
                            Id = post.Id
                        };

            foreach (var post in posts)
            {
                BlogPosts.Add(post);
            }
        }

        public async Task AddBlogPost(string title, string content, string blogId)
        {
            await AuthenticateAsync();

            Post newPost = new Post()
            {
                Kind = "blogger#post",
                Blog = new Post.BlogData() { Id = blogId },
                Title = title,
                Content = content
            };

            try
            {
                PostsResource.InsertRequest insertRequest = _service.Posts.Insert(newPost, blogId);
                await insertRequest.ExecuteAsync();
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        public async Task UpdateBlogPost(string title, string content, string blogId, string postId)
        {
            await AuthenticateAsync();

            var newPost = new Post()
            {
                Kind = "blogger#post",
                Id = postId,
                Blog = new Post.BlogData() { Id = blogId },
                Title = title,
                Content = content
            };

            try
            {
                PostsResource.UpdateRequest updateRequest = _service.Posts.Update(newPost, blogId, postId);
                await updateRequest.ExecuteAsync();
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        #endregion

        #region Utility functions

        private async Task AuthenticateAsync()
        {
            if (_service != null)
            {
                return;
            }
            
            FileStream stream = new FileStream(SecretFileName, FileMode.Open, FileAccess.Read);

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(20));
            CancellationToken ct = cts.Token;

            _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                new[] { BloggerService.Scope.Blogger },
                "user",
                ct
                );

            BaseClientService.Initializer initializer = new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = ApplicationName
            };

            _service = new BloggerService(initializer);
        }

        #endregion
    }
}
