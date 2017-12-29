namespace PlaylistDir.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Xml.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Linq;
    using System.Collections.ObjectModel;

    public class MainViewModel : INotifyPropertyChanged
    {
        public  class PlaylistSong
        {
            public int TrackNumber { get; set; }

            public string Artist { get; set; }

            public string Song { get; set; }

            public string Album { get; set; }
        }


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

        /// <summary>
        /// The show playlist command.
        /// </summary>
        private ICommand _showPlaylistCommand;

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

        public ObservableCollection<PlaylistSong> SongsReadFromPlaylist { get; private set; }

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
        /// Copy the playlist command.
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

        /// <summary>
        /// Show the playlist command.
        /// </summary>
        public ICommand ShowPlaylistCommand
        {
            get
            {
                return _showPlaylistCommand ??
                    (_showPlaylistCommand =
                        new CommandHandler(() => ShowPlaylistCommandAction(), true));
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
        /// The reads the playlist file and populates the display list.
        /// </summary>
        public void ShowPlaylistCommandAction()
        {
            List<string> songs = GetPlaylistSongs();

            string sourceDirectory = Path.GetDirectoryName(Playlist);

            SongsReadFromPlaylist.Clear();

            int trackNumber = 1;
            foreach (var song in songs)
            {
                var songPath = Path.Combine(sourceDirectory, song);
                TagLib.File file = TagLib.File.Create(songPath);

                PlaylistSong songFile = new PlaylistSong() { TrackNumber = trackNumber };
                songFile.Artist = file.Tag.FirstAlbumArtist;
                songFile.Song = file.Tag.Title;
                songFile.Album = file.Tag.Album;
                SongsReadFromPlaylist.Add(songFile);
                trackNumber++;
            }

            OnPropertyChanged(() => SongsReadFromPlaylist);
        }

        /// <summary>
        /// The reads the playlist file and copies the file to the directory.
        /// </summary>
        public void CopyPlaylistCommandAction()
        {
            List<string> songs = GetPlaylistSongs();

            string sourceDirectory = Path.GetDirectoryName(Playlist);

            foreach (var song in songs)
            {
                var songPath = Path.Combine(sourceDirectory, song);
                var outputSong = Path.Combine(OutputDirectory, Path.GetFileName(song));
                File.Copy(songPath, outputSong);
            }
        }

        #endregion

        #region Utility Functions

        private List<string> GetPlaylistSongs()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Playlist);

            List<string> songs = new List<string>();
            XmlNode body = null;
            foreach (XmlNode child in doc.DocumentElement.ChildNodes)
            {
                if (child.Name == "body")
                {
                    body = child;
                    break;
                }
            }

            XmlNode seq = null;
            if (body != null)
            {

                foreach (XmlNode child in body.ChildNodes)
                {
                    if (child.Name == "seq")
                    {
                        seq = child;
                        break;
                    }
                }
            }

            if (seq != null)
            {
                foreach (XmlNode child in seq.ChildNodes)
                {
                    if (child.Name == "media")
                    {
                        foreach (XmlAttribute attribute in child.Attributes)
                        {
                            if (attribute.Name == "src")
                            {
                                songs.Add(attribute.Value);
                            }
                        }
                    }
                }
            }

            return songs;
        }

        #endregion

        public MainViewModel()
        {
            SongsReadFromPlaylist = new ObservableCollection<PlaylistSong>();
        }
    }
}
