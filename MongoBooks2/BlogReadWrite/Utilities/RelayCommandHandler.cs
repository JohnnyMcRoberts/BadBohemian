// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelayCommandHandler.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The relay command handler utility.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogReadWrite.Utilities
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// The relay command handler takes a parameter and is used for template types.
    /// </summary>
    public class RelayCommandHandler : ICommand
    {
        /// <summary>
        /// The handler action for the command.
        /// </summary>
        private readonly Action<object> _handler;

        /// <summary>
        /// Whether the item is enabled.
        /// </summary>
        private bool _isEnabled;

        /// <summary>
        /// The can execute command changed event.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the command is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }

            set
            {
                if (value == _isEnabled)
                {
                    return;
                }

                _isEnabled = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets if the command can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter (ignored).
        /// </param>
        /// <returns>True if can execute the command, otherwise false.</returns>
        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        /// <summary>
        /// Executes the command handler.
        /// </summary>
        /// <param name="parameter">
        /// The parameter for the handler.
        /// </param>
        public void Execute(object parameter)
        {
            _handler(parameter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommandHandler"/> class.
        /// </summary>
        /// <param name="handler">
        /// The command handler.
        /// </param>
        public RelayCommandHandler(Action<object> handler)
        {
            _handler = handler;
        }
    }
}
