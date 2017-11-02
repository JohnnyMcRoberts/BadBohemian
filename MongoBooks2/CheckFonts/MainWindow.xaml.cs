namespace CheckFonts
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq.Expressions;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows.Forms;
    using System.Windows.Media;

    using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
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

        private DemoCommand _selectOriginalFontCommand;

        private DemoCommand _selectUpdatedFontCommand;

        private DemoCommand _findMissingCharactersCommand;

        private string _originalFontFile;

        private string _updatedFontFile;

        private string _missingCharacters;

        private string _originalFontFamily;

        private bool _isUpdating;

        public bool IsUpdating
        {
            get { return _isUpdating; }
            set
            {
                if (value != _isUpdating)
                {
                    _isUpdating = value;
                    OnPropertyChanged(() => IsUpdating);
                }
            }
        }

        public string OriginalFontFile
        {
            get
            {
                return _originalFontFile;
            }

            set
            {
                _originalFontFile = value;
                OnPropertyChanged(() => OriginalFontFile);
            }
        }

        public string UpdatedFontFile
        {
            get
            {
                return _updatedFontFile;
            }

            set
            {
                _updatedFontFile = value;
                OnPropertyChanged(() => UpdatedFontFile);
            }
        }

        public string MissingCharacters
        {
            get
            {
                return _missingCharacters;
            }

            set
            {
                _missingCharacters = value;
                OnPropertyChanged(() => MissingCharacters);
            }
        }

        public string OriginalFontFamily
        {
            get
            {
                return _originalFontFamily;
            }

            set
            {
                _originalFontFamily = value;
                OnPropertyChanged(() => OriginalFontFamily);
            }
        }

        public string FontsDirectory { get; set; }

        public ICommand SelectOriginalFontCommand
        {
            get
            {
                if (_selectOriginalFontCommand == null)
                    _selectOriginalFontCommand = new DemoSelectOriginalFontCommand(this);
                return _selectOriginalFontCommand;
            }
        }

        public ICommand SelectUpdatedFontCommand
        {
            get
            {
                if (_selectUpdatedFontCommand == null)
                    _selectUpdatedFontCommand = new DemoSelectUpdatedFontCommand(this);
                return _selectUpdatedFontCommand;
            }
        }

        public ICommand FindMissingCharactersCommand
        {
            get
            {
                if (_findMissingCharactersCommand == null)
                    _findMissingCharactersCommand = new DemoFindMissingCharactersCommand(this);
                return _findMissingCharactersCommand;
            }
        }
        

        public MainWindow()
        {
            InitializeComponent();

            FontsDirectory = @"D:\\Temp\\fonts";
            OriginalFontFile = @"batang.ttc";
            UpdatedFontFile = @"msgothic.ttc";
            _missingCharacters = string.Empty;
            _originalFontFamily = "Arial";
            _isUpdating = false;

            DataContext = this;
        }
    }

    /// <summary>
    /// Base class for Demo Commands
    /// </summary>
    abstract class DemoCommand : ICommand
    {
        /// <summary>
        /// MainWindow instance
        /// </summary>
        protected MainWindow ParentWindow;

        protected bool GetFontFile(ref string fileName)
        {
            bool setFile = false;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.FileName = fileName;

            fileDialog.Filter = @"All files (*.*)|*.*|TTC Files (*.ttc)|*.ttc|TTF Files (*.ttf)|*.ttf";
            fileDialog.FilterIndex = 1;
            fileDialog.RestoreDirectory = true;
            fileDialog.InitialDirectory = ParentWindow.FontsDirectory;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                setFile = true;
                fileName = fileDialog.FileName;
            }
            return setFile;
        }

        /// <summary>
        /// Constructs DemoCommand
        /// </summary>
        /// <param name="parentWindow">MainWindow instance</param>
        public DemoCommand(MainWindow parentWindow)
        {
            if (parentWindow == null)
                throw new ArgumentNullException("parentWindow");

            ParentWindow = parentWindow;
        }


        #region ICommand Members

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// 
        /// DemoCommands are executable by default
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require
        /// data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// 
        /// Override to provide command implementation
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data
        /// to be passed, this object can be set to null.</param>
        public abstract void Execute(object parameter);

        #endregion // ICommand Members
    }

    /// <summary>
    /// Demo Select original font file Command Handler
    /// </summary>
    class DemoSelectOriginalFontCommand : DemoCommand
    {
        public DemoSelectOriginalFontCommand(MainWindow parentWindow)
            : base(parentWindow)
        {

        }

        public override void Execute(object parameter)
        {
            string fileName = ParentWindow.OriginalFontFile;
            bool setFile = GetFontFile(ref fileName);

            if (setFile)
            {
                ParentWindow.OriginalFontFile = fileName;
                ParentWindow.FontsDirectory = Path.GetDirectoryName(fileName);
            }
        }
    }

    /// <summary>
    /// Demo Select updated font file Command Handler
    /// </summary>
    class DemoSelectUpdatedFontCommand : DemoCommand
    {
        public DemoSelectUpdatedFontCommand(MainWindow parentWindow)
            : base(parentWindow)
        {

        }

        public override void Execute(object parameter)
        {
            string fileName = ParentWindow.UpdatedFontFile;
            bool setFile = GetFontFile(ref fileName);

            if (setFile)
            {
                ParentWindow.UpdatedFontFile = fileName;
                ParentWindow.FontsDirectory = Path.GetDirectoryName(fileName);
            }
        }
    }

    /// <summary>
    /// Demo Find missing font charcters file Command Handler
    /// </summary>
    class DemoFindMissingCharactersCommand : DemoCommand
    {
        private static List<ushort> GetOrderedUnicodeCharactersForFontFile(string fontLocation)
        {
            var families = Fonts.GetFontFamilies(fontLocation);
            Dictionary<ushort, ushort> unicodeCharacters = new Dictionary<ushort, ushort>();

            foreach (FontFamily family in families)
            {
                var typefaces = family.GetTypefaces();
                foreach (Typeface typeface in typefaces)
                {
                    GlyphTypeface glyph;
                    typeface.TryGetGlyphTypeface(out glyph);
                    IDictionary<int, ushort> characterMap = glyph.CharacterToGlyphMap;

                    foreach (KeyValuePair<int, ushort> kvp in characterMap)
                    {
                        if (!unicodeCharacters.ContainsKey(kvp.Value))
                            unicodeCharacters.Add(kvp.Value, kvp.Value);
                    }
                }
            }

            List<ushort> orderedUnicodeCharacters = unicodeCharacters.Keys.ToList().OrderBy(x => x).ToList();
            return orderedUnicodeCharacters;
        }

        public DemoFindMissingCharactersCommand(MainWindow parentWindow)
            : base(parentWindow)
        {

        }

        public override void Execute(object parameter)
        {
            ParentWindow.OriginalFontFamily = 
                Fonts.GetFontFamilies(ParentWindow.OriginalFontFile).First().FamilyNames.Values.First();
            string outputMessage = "No Missing Characters";

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (o, ea) =>
            {

                List<ushort> originalUnicodeCharacters = GetOrderedUnicodeCharactersForFontFile(ParentWindow.OriginalFontFile);
                List<ushort> updatedUnicodeCharacters = GetOrderedUnicodeCharactersForFontFile(ParentWindow.UpdatedFontFile);

                List<ushort> missingCharacters =
                    originalUnicodeCharacters.Where(original => !updatedUnicodeCharacters.Contains(original)).ToList();
                
                if (missingCharacters.Any())
                {

                    outputMessage = $"{missingCharacters.Count} Missing Unicode character \n";
                    foreach (ushort key in missingCharacters)
                    {
                        outputMessage += $"Unicode character = u{key} : {Convert.ToChar(key)} \n";
                    }
                }
            };

            worker.RunWorkerCompleted += (o, ea) =>
            {
                ParentWindow.IsUpdating = false;
                ParentWindow.MissingCharacters = outputMessage;
            };

            ParentWindow.IsUpdating = true;
            worker.RunWorkerAsync();




        }
    }
}
