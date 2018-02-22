// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PasswordHelper.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   Defines the PasswordHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BooksUtilities.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    public class BaseViewModel : INotifyPropertyChanged
    {
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

            OnPropertyChanged(body.Member.Name);
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
    }
}
