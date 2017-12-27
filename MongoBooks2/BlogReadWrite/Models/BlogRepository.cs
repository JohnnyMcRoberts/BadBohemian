
namespace BlogReadWrite.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;

    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Blogger.v3;
    using Google.Apis.Services;


    public class BlogRepository
    {
        #region Private data

        private UserCredential _credential;

        private BloggerService _service;

        #endregion

        #region Public Data

        public string ApplicationName { get; set; }

        public string SecretFileName { get; set; }

        public ObservableCollection<Google.Apis.Blogger.v3.Data.Blog> Blogs { get; private set; }

        public ObservableCollection<Google.Apis.Blogger.v3.Data.Post> BlogPosts { get; private set; }

        #endregion

        #region Constructors

        public BlogRepository()
        {
            Blogs = new ObservableCollection<Google.Apis.Blogger.v3.Data.Blog>();
            BlogPosts = new ObservableCollection<Google.Apis.Blogger.v3.Data.Post>();
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
                        select new Google.Apis.Blogger.v3.Data.Blog
                        {
                            Id = blog.Id,
                            Name = blog.Name
                        };

            foreach (var blog in blogs)
                Blogs.Add(blog);
        }

        public async Task GetBlogPosts(string blogId)
        {
            await AuthenticateAsync();
            var list = await _service.Posts.List(blogId).ExecuteAsync();

            BlogPosts.Clear();
            var posts = from post in list.Items
                        select new Google.Apis.Blogger.v3.Data.Post
                        {
                            Title = post.Title,
                            Content = post.Content,
                            Id = post.Id
                        };

            foreach (var post in posts)
                BlogPosts.Add(post);
        }

        public async Task AddBlogPost(string title, string content, string blogId)
        {
            await AuthenticateAsync();

            var newPost = new Google.Apis.Blogger.v3.Data.Post()
            {
                Kind = "blogger#post",
                Blog = new Google.Apis.Blogger.v3.Data.Post.BlogData() { Id = blogId },
                Title = title,
                Content = content
            };

            try
            {
                var insertRequest = _service.Posts.Insert(newPost, blogId);
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

            var newPost = new Google.Apis.Blogger.v3.Data.Post()
            {
                Kind = "blogger#post",
                Id = postId,
                Blog = new Google.Apis.Blogger.v3.Data.Post.BlogData() { Id = blogId },
                Title = title,
                Content = content
            };

            try
            {
                var insertRequest = _service.Posts.Update(newPost, blogId, postId);
                await insertRequest.ExecuteAsync();

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
                return;

            UserCredential credential;
            var stream = new FileStream(SecretFileName, FileMode.Open, FileAccess.Read);

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
                ApplicationName = ApplicationName
            };

            _service = new BloggerService(initializer);
        }

        #endregion

    }
}
