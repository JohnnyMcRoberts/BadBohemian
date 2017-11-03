namespace CheckFonts
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Xml;

    using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        /// <summary>
        /// The select original font file command.
        /// </summary>
        private DemoCommand _selectOriginalFontCommand;

        /// <summary>
        /// The select updated font file command.
        /// </summary>
        private DemoCommand _selectUpdatedFontCommand;

        /// <summary>
        /// The select resource dictionary file command.
        /// </summary>
        private DemoCommand _selectResourceFileCommand;

        /// <summary>
        /// The find missing unicode characters command.
        /// </summary>
        private DemoCommand _findMissingCharactersCommand;

        /// <summary>
        /// The full path for the original font file.
        /// </summary>
        private string _originalFontFile;

        /// <summary>
        /// The full path for the updated font file.
        /// </summary>
        private string _updatedFontFile;

        /// <summary>
        /// The full path for the resource dictionary file.
        /// </summary>
        private string _resourceFile;

        /// <summary>
        /// The list of missing characters to display.
        /// </summary>
        private string _missingCharacters;

        /// <summary>
        /// The name of the original font family.
        /// </summary>
        private string _originalFontFamily;

        /// <summary>
        /// The name of the updated font family.
        /// </summary>
        private string _updatedFontFamily;

        /// <summary>
        /// The value indicating whether the process to get the missing characters is running.
        /// </summary>
        private bool _isRunning;

        /// <summary>
        /// Gets or sets a value indicating whether the process to get the missing characters is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                if (value != _isRunning)
                {
                    _isRunning = value;
                    OnPropertyChanged(() => IsRunning);
                }
            }
        }

        /// <summary>
        /// Gets or sets the full path for the original font file.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the full path for the updated font file.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the full path for the resource dictionary file.
        /// </summary>
        public string ResourceFile
        {
            get
            {
                return _resourceFile;
            }

            set
            {
                _resourceFile = value;
                OnPropertyChanged(() => ResourceFile);
            }
        }

        /// <summary>
        /// Gets or sets the list of missing characters to display.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the name of the original font family.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the name of the updated font family.
        /// </summary>
        public string UpdatedFontFamily
        {
            get
            {
                return _updatedFontFamily;
            }

            set
            {
                _updatedFontFamily = value;
                OnPropertyChanged(() => UpdatedFontFamily);
            }
        }

        /// <summary>
        /// Gets or sets the default directory to open into.
        /// </summary>
        public string DefaultDirectory { get; set; }

        /// <summary>
        /// Gets the select original font file command.
        /// </summary>
        public ICommand SelectOriginalFontCommand
        {
            get
            {
                if (_selectOriginalFontCommand == null)
                {
                    _selectOriginalFontCommand = new DemoSelectOriginalFontCommand(this);
                }

                return _selectOriginalFontCommand;
            }
        }

        /// <summary>
        /// Gets the select updated font file command.
        /// </summary>
        public ICommand SelectUpdatedFontCommand
        {
            get
            {
                if (_selectUpdatedFontCommand == null)
                    _selectUpdatedFontCommand = new DemoSelectUpdatedFontCommand(this);
                return _selectUpdatedFontCommand;
            }
        }

        /// <summary>
        /// Gets the select resource dictionary file command.
        /// </summary>
        public ICommand SelectResourceFileCommand
        {
            get
            {
                if (_selectResourceFileCommand == null)
                    _selectResourceFileCommand = new DemoSelectResourceFileCommand(this);
                return _selectResourceFileCommand;
            }
        }

        /// <summary>
        /// Gets the find missing unicode characters command.
        /// </summary>
        public ICommand FindMissingCharactersCommand
        {
            get
            {
                if (_findMissingCharactersCommand == null)
                    _findMissingCharactersCommand = new DemoFindMissingCharactersCommand(this);
                return _findMissingCharactersCommand;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            DefaultDirectory = @"D:\\Temp\\fonts";
            OriginalFontFile = @"batang.ttc";
            UpdatedFontFile = @"msgothic.ttc";
            _missingCharacters = string.Empty;
            _originalFontFamily = "Arial";
            _updatedFontFile = "Arial";
            _isRunning = false;

            DataContext = this;
        }
    }

    /// <summary>
    /// Base class for Demo Commands
    /// </summary>
    public abstract class DemoCommand : ICommand
    {
        /// <summary>
        /// MainWindow instance
        /// </summary>
        protected MainWindow ParentWindow;

        protected bool GetFile(ref string fileName, bool isFontFile = true)
        {
            bool setFile = false;
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                FileName = fileName,
                FilterIndex = 1,
                RestoreDirectory = true,
                InitialDirectory = ParentWindow.DefaultDirectory
            };

            if (isFontFile)
                fileDialog.Filter = @"All files (*.*)|*.*|TTC Files (*.ttc)|*.ttc|TTF Files (*.ttf)|*.ttf";
            else
                fileDialog.Filter = @"All files (*.*)|*.*|XAML Files (*.xaml)|*.xaml|XML Files (*.xml)|*.xml";

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
    public class DemoSelectOriginalFontCommand : DemoCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DemoSelectOriginalFontCommand"/> class.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        public DemoSelectOriginalFontCommand(MainWindow parentWindow)
            : base(parentWindow)
        {

        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// 
        /// Override to provide command implementation
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data
        /// to be passed, this object can be set to null.</param>
        public override void Execute(object parameter)
        {
            string fileName = ParentWindow.OriginalFontFile;
            bool setFile = GetFile(ref fileName);

            if (setFile)
            {
                ParentWindow.OriginalFontFile = fileName;
                ParentWindow.DefaultDirectory = Path.GetDirectoryName(fileName);
            }
        }
    }

    /// <summary>
    /// Demo Select updated font file Command Handler
    /// </summary>
    public class DemoSelectUpdatedFontCommand : DemoCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DemoSelectUpdatedFontCommand"/> class.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        public DemoSelectUpdatedFontCommand(MainWindow parentWindow)
            : base(parentWindow)
        {

        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// 
        /// Override to provide command implementation
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data
        /// to be passed, this object can be set to null.</param>
        public override void Execute(object parameter)
        {
            string fileName = ParentWindow.UpdatedFontFile;
            bool setFile = GetFile(ref fileName);

            if (setFile)
            {
                ParentWindow.UpdatedFontFile = fileName;
                ParentWindow.DefaultDirectory = Path.GetDirectoryName(fileName);
            }
        }
    }

    /// <summary>
    /// Demo Select the resource file Command Handler
    /// </summary>
    public class DemoSelectResourceFileCommand : DemoCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DemoSelectResourceFileCommand"/> class.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        public DemoSelectResourceFileCommand(MainWindow parentWindow)
            : base(parentWindow)
        {

        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// 
        /// Override to provide command implementation
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data
        /// to be passed, this object can be set to null.</param>
        public override void Execute(object parameter)
        {
            string fileName = ParentWindow.UpdatedFontFile;
            bool setFile = GetFile(ref fileName, false);

            if (setFile)
            {
                ParentWindow.ResourceFile = fileName;
                ParentWindow.DefaultDirectory = Path.GetDirectoryName(fileName);
            }
        }
    }

    /// <summary>
    /// Demo Find missing font charcters file Command Handler
    /// </summary>
    public class DemoFindMissingCharactersCommand : DemoCommand
    {
        /// <summary>
        /// Gets the unicode characters supported by the font file as an ordered list of numbers.
        /// </summary>
        /// <param name="fontFileLocation">The font file to get the characters from.</param>
        /// <returns>The ordered list.</returns>
        private static List<int> GetOrderedUnicodeCharactersForFontFile(string fontFileLocation)
        {
            var families = Fonts.GetFontFamilies(fontFileLocation);
            Dictionary<int, int> unicodeCharacters = new Dictionary<int, int>();

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
                        if (!unicodeCharacters.ContainsKey(kvp.Key))
                            unicodeCharacters.Add(kvp.Key, kvp.Key);
                    }
                }
            }

            List<int> orderedUnicodeCharacters = unicodeCharacters.Keys.ToList().OrderBy(x => x).ToList();
            return orderedUnicodeCharacters;
        }

        /// <summary>
        /// Gets the resource string variables from a file as a dictionary.
        /// </summary>
        /// <param name="resourceFile">The resource file to read from.</param>
        /// <param name="errorMessage">The error message to display.</param>
        /// <param name="resourceStrings">The resource string variables as a dictionary.</param>
        /// <returns>True if the file was parsed successfully.</returns>
        private static bool GetResourceStrings(string resourceFile, out string errorMessage, out Dictionary<string, string> resourceStrings)
        {
            // Initialise the variables.
            errorMessage = string.Empty;
            resourceStrings = new Dictionary<string, string>();

            // Load the xml from the resource file.
            XmlDocument xd = new XmlDocument();
            xd.Load(resourceFile);

            // Get the resource dictiony node.
            XmlNode nodeForResourceDictionary = 
                xd.ChildNodes.Cast<XmlNode>().FirstOrDefault(
                    child => child.HasChildNodes && child.NodeType == XmlNodeType.Element && child.Name == "ResourceDictionary");

            // Stop if nothing there.
            if (nodeForResourceDictionary == null)
            {
                errorMessage = "No ResourceDictionary in the file";
                return false;
            }

            // Get the names and values for the sys:String resources.
            foreach (XmlNode node in nodeForResourceDictionary.ChildNodes)
            {
                if (!node.HasChildNodes ||
                    node.NodeType != XmlNodeType.Element ||
                    node.Name != "sys:String" ||
                    node.Attributes == null ||
                    node.Attributes.Count == 0)
                {
                    continue;
                }
                
                resourceStrings.Add(node.Attributes[0].Value, node.InnerText);
            }

            // If nothing there return an error.
            if (!resourceStrings.Any())
            {
                errorMessage = "No strings in the resource file";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the unicode characters that are in the original font but are missing from the upadted font.
        /// </summary>
        /// <param name="originalUnicodeCharacters">The unicode characters in the original font.</param>
        /// <param name="updatedUnicodeCharacters">The unicode characters in the updated font.</param>
        /// <param name="outputMessage">The display message to append the missing items to.</param>
        /// <returns>The updated display message.</returns>
        private static string GetMissingCharactersFromComparison(
            List<int> originalUnicodeCharacters,
            List<int> updatedUnicodeCharacters,
            string outputMessage)
        {
            List<int> missingCharacters =
                originalUnicodeCharacters.Where(original => !updatedUnicodeCharacters.Contains(original)).ToList();

            if (missingCharacters.Any())
            {
                outputMessage += $"{missingCharacters.Count} missing Unicode characters \n";
                foreach (int key in missingCharacters)
                {
                    try
                    {
                        char keyCharacter = Convert.ToChar(key);
                        outputMessage += $"Unicode character = u+{key:X} : {keyCharacter} \n";
                    }
                    catch (Exception)
                    {
                        outputMessage += $"Unicode character = u+{key:X} : outside range \n";
                    }
                }
            }
            return outputMessage;
        }

        /// <summary>
        /// Gets the unicode characters that are in the resource file but are missing from the upadted font.
        /// </summary>
        /// <param name="resourceStrings">The resource string variables as a dictionary.</param>
        /// <param name="updatedAsDictionary">The unicode characters in the updated font as a dictionary.</param>
        /// <param name="outputMessage">The display message to append the missing items to.</param>
        /// <returns>The updated display message.</returns>
        private static string GetMissingCharactersFromResources(
            Dictionary<string, string> resourceStrings,
            Dictionary<int, int> updatedAsDictionary,
            string outputMessage)
        {
            int missingCount = 0;
            foreach (string resourceKey in resourceStrings.Keys)
            {
                string resoureStringValue = resourceStrings[resourceKey];
                List<int> stringAsUniCode = resoureStringValue.Select(Convert.ToInt32).ToList();

                bool failed = false;
                string failMessage = string.Empty;
                foreach (int key in stringAsUniCode)
                {
                    if (!updatedAsDictionary.ContainsKey(key))
                    {
                        failed = true;
                        failMessage =
                            $"Resource {resourceKey} ({resoureStringValue}) is missing Unicode character = u+{key:X} : {Convert.ToChar(key)} \n";
                        missingCount++;
                        break;
                    }
                }

                if (failed)
                {
                    outputMessage += failMessage;
                }
            }

            if (missingCount == 0)
            {
                outputMessage = @"No missing characters in the Resources" +"\n";
            }
            else
            {
                outputMessage = $"{missingCount} Resources with missing characters in the following resources\n" +
                                outputMessage;
            }
            return outputMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DemoFindMissingCharactersCommand"/> class.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        public DemoFindMissingCharactersCommand(MainWindow parentWindow)
            : base(parentWindow)
        {

        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// 
        /// Override to provide command implementation
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data
        /// to be passed, this object can be set to null.</param>
        public override void Execute(object parameter)
        {
            if (!File.Exists(ParentWindow.OriginalFontFile) ||
                !File.Exists(ParentWindow.UpdatedFontFile) ||
                !File.Exists(ParentWindow.ResourceFile))
            {
                MessageBox.Show(@"Must select valid files.");
            }

            // Set the fonts.
            ParentWindow.OriginalFontFamily = 
                Fonts.GetFontFamilies(ParentWindow.OriginalFontFile).First().FamilyNames.Values.First();

            ParentWindow.UpdatedFontFamily =
                Fonts.GetFontFamilies(ParentWindow.UpdatedFontFile).First().FamilyNames.Values.First();

            string outputMessage = @"No Missing Characters";
            
            string errorMessage;
            Dictionary<string, string> resourceStrings;
            bool canContinue = GetResourceStrings(ParentWindow.ResourceFile, out errorMessage, out resourceStrings);

            if (canContinue == false)
            {
                MessageBox.Show(errorMessage);
            }

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (o, ea) =>
            {

                List<int> originalUnicodeCharacters = GetOrderedUnicodeCharactersForFontFile(ParentWindow.OriginalFontFile);
                List<int> updatedUnicodeCharacters = GetOrderedUnicodeCharactersForFontFile(ParentWindow.UpdatedFontFile);

                Dictionary<int, int> updatedAsDictionary = updatedUnicodeCharacters.ToDictionary(unicodeCharacter => unicodeCharacter);

                outputMessage = string.Empty;
                outputMessage = GetMissingCharactersFromResources(resourceStrings, updatedAsDictionary, outputMessage);
                outputMessage = GetMissingCharactersFromComparison(originalUnicodeCharacters, updatedUnicodeCharacters, outputMessage);
            };

            worker.RunWorkerCompleted += (o, ea) =>
            {
                ParentWindow.IsRunning = false;
                ParentWindow.MissingCharacters = outputMessage;
            };

            ParentWindow.IsRunning = true;
            worker.RunWorkerAsync();
        }
    }
}
