
namespace MongoDbBooks.ViewModels.Utilities
{
    using System.Windows;

    public static class DialogCloserExtension
    {
        // Register the attached property.
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached(
            "DialogResult",
            typeof(bool?),
            typeof(DialogCloserExtension),
            new PropertyMetadata(DialogResultChanged));

        // Handle the property has changed by actually closing the window itself which
        // was passed in as part of the argument list.
        private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window != null && window.IsVisible)
            {
                window.DialogResult = e.NewValue as bool?;
            }
        }

        // Property getter.
        public static object GetDialogResult(Window target)
        {
            bool? bReturn = null;

            try
            {
                if (target != null)
                {
                    bReturn = target.GetValue(DialogResultProperty) as bool?;
                }
            }
            catch
            {
                // Just eat the exception if there is one so that the application will not crash.
            }
            return (object)bReturn;
        }

        // Property setter.
        public static void SetDialogResult(Window target, bool? value)
        {
            if (target != null)
            {
                target.SetValue(DialogResultProperty, value);
            }
        }
    }
}
