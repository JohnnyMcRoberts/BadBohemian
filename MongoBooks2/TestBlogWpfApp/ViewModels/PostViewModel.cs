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
    using System.Web;

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

    public class PostViewModel : INotifyPropertyChanged
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

        //public string DisplayContent =>  Windows.Data.Html.HtmlUtilities.ConvertToText(content);

        private string content;
        /// <summary>Gets or sets the post content.</summary>
        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                OnPropertyChanged(() => Content);
            }
        }

        private string title;
        /// <summary>Gets or sets the post title.</summary>
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged(() => Title);
            }
        }

        private string id;
        /// <summary>Gets or sets the post Id.</summary>
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged(() => Id);
            }
        }

    }
}
