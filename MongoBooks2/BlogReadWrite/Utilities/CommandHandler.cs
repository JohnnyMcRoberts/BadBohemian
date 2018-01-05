// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandHandler.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The command handler utility.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogReadWrite.Utilities
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// The command handler class.
    /// </summary>
    public class CommandHandler : ICommand
    {
        private readonly bool _canExecute;

        private readonly Action _action;

        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
