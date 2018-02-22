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
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The password helper.
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// The password property.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password", 
                typeof(string), 
                typeof(PasswordHelper), 
                new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        /// <summary>
        /// The attach property.
        /// </summary>
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached(
                "Attach",
                typeof(bool), 
                typeof(PasswordHelper), 
                new PropertyMetadata(false, Attach));

        /// <summary>
        /// The is updating property.
        /// </summary>
        private static readonly DependencyProperty IsUpdatingProperty =
           DependencyProperty.RegisterAttached(
               "IsUpdating", 
               typeof(bool),
               typeof(PasswordHelper));

        /// <summary>
        /// The set attach.
        /// </summary>
        /// <param name="dp">
        /// The dependency property.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }

        /// <summary>
        /// The get attach.
        /// </summary>
        /// <param name="dp">
        /// The dependency property.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }

        /// <summary>
        /// The get password.
        /// </summary>
        /// <param name="dp">
        /// The dependency property.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        /// <summary>
        /// The set password.
        /// </summary>
        /// <param name="dp">
        /// The dependency property.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        /// <summary>
        /// The get is updating.
        /// </summary>
        /// <param name="dp">
        /// The dependency property.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        /// <summary>
        /// The set is updating.
        /// </summary>
        /// <param name="dp">
        /// The dependency property.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        /// <summary>
        /// The on password property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event.
        /// </param>
        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox == null)
            {
                return;
            }
            passwordBox.PasswordChanged -= PasswordChanged;

            if (!GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }

            passwordBox.PasswordChanged += PasswordChanged;
        }

        /// <summary>
        /// The attach.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event.
        /// </param>
        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (passwordBox == null)
            {
                return;
            }

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        /// <summary>
        /// The password changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox?.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
}
