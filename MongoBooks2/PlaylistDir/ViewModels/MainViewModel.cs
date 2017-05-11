namespace PlaylistDir.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Windows.Forms;
    using System.Windows.Input;

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

        #region Private variables

        /// <summary>
        /// The select playlist command.
        /// </summary>
        private ICommand _selectPlaylistCommand;

        /// <summary>
        /// The select output directory command.
        /// </summary>
        private ICommand _selectDirectoryCommand;

        /// <summary>
        /// The copy playlist command.
        /// </summary>
        private ICommand _copyPlaylistCommand;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the playlist file to expand.
        /// </summary>
        public string Playlist { get; private set; }

        /// <summary>
        /// Gets or sets the directory to copy the files to.
        /// </summary>
        public string OutputDirectory { get; private set; }

        #endregion

        #region Commands 

        /// <summary>
        /// Select the playlist command.
        /// </summary>
        public ICommand SelectPlaylistCommand
        {
            get
            {
                return _selectPlaylistCommand ??
                    (_selectPlaylistCommand =
                        new CommandHandler(() => SelectPlaylistCommandAction(), true));
            }
        }

        /// <summary>
        /// Select the playlist command.
        /// </summary>
        public ICommand SelectDirectoryCommand
        {
            get
            {
                return _selectDirectoryCommand ??
                    (_selectDirectoryCommand =
                        new CommandHandler(() => SelectDirectoryCommandAction(), true));
            }
        }

        /// <summary>
        /// Select the playlist command.
        /// </summary>
        public ICommand CopyPlaylistCommand
        {
            get
            {
                return _copyPlaylistCommand ??
                    (_copyPlaylistCommand =
                        new CommandHandler(() => CopyPlaylistCommandAction(), true));
            }
        }
        #endregion

        #region Command Handlers

        /// <summary>
        /// Selects the playlist file to copy from.
        /// </summary>
        public void SelectPlaylistCommandAction()
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = @"WPL File (.wpl)|*.wpl";
                fileDialog.FilterIndex = 4;
                fileDialog.RestoreDirectory = true;

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    Playlist = fileDialog.FileName;
                    OnPropertyChanged(() => Playlist);
                }
            }
        }

        /// <summary>
        /// The selects the output directory to put the files into.
        /// </summary>
        public void SelectDirectoryCommandAction()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    OutputDirectory = dialog.SelectedPath;
                    OnPropertyChanged(() => OutputDirectory);
                }
            }
        }

        /// <summary>
        /// The reads the playlist file and copies the file to the directory.
        /// </summary>
        public void CopyPlaylistCommandAction()
        {
        }

        #endregion

    }
}
