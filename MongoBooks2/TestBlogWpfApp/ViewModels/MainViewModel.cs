using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TestBlogWpfApp.ViewModels
{
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


        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            _title = "Blogger Test";
            //_emailAddress = string.Empty;
        }

        #endregion // Constructors


    }
}
