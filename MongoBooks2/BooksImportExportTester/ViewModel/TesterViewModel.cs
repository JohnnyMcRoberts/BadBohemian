// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The main view model for the books import export tester application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksImportExportTester.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using System.Windows.Forms;

    using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

    using BooksCore.Provider;
    using BooksImportExport.Exporters;
    using BooksImportExport.Interfaces;
    using BooksImportExport.Utilities;
    using BooksTesterUtilities.ViewModels;
    using BooksUtilities.ViewModels;

    /// <summary>
    /// The view model for the books editors tester application.
    /// </summary>
    public class TesterViewModel : BaseTesterViewModel
    {
        #region Private data

        /// <summary>
        /// The output file.
        /// </summary>
        private string _outputFile;

        /// <summary>
        /// The selected export type.
        /// </summary>
        private ExportType _selectedExportType;

        /// <summary>
        /// The select output file command.
        /// </summary>
        private ICommand _selectOutputFileCommand;

        /// <summary>
        /// The export file command.
        /// </summary>
        private ICommand _exportFileCommand;

        /// <summary>
        /// The export file error.
        /// </summary>
        private string _exportErrorMessage;

        #endregion

        #region Public data

        /// <summary>
        /// Gets or sets the column chart type.
        /// </summary>
        public ExportType SelectedExportType
        {
            get
            {
                return _selectedExportType;
            }

            set
            {
                if (value != _selectedExportType)
                {
                    _selectedExportType = value;
                    OnPropertyChanged(() => SelectedExportType);
                    OutputFile = String.Empty;
                }
            }
        }

        /// <summary>
        /// Gets if have a valid output file set.
        /// </summary>
        public bool IsValidOutputFile => !string.IsNullOrEmpty(OutputFile);

        /// <summary>
        /// Gets the export types and titles.
        /// </summary>
        public Dictionary<ExportType, string> ExportTypesByTitle { get; private set; }

        /// <summary>
        /// Gets the select output file command.
        /// </summary>
        public ICommand SelectOutputFileCommand =>
            _selectOutputFileCommand ?? (_selectOutputFileCommand = new CommandHandler(SelectOutputFileCommandAction, true));

        /// <summary>
        /// Gets the export file command.
        /// </summary>
        public ICommand ExportFileCommand =>
            _exportFileCommand ?? (_exportFileCommand = new CommandHandler(ExportFileCommandAction, true));

        /// <summary>
        /// Gets the output file command.
        /// </summary>
        public string OutputFile
        {
            get { return _outputFile; }
            private set
            {
                if (_outputFile != value)
                {
                    _outputFile = value;
                    OnPropertyChanged(() => IsValidOutputFile);
                    OnPropertyChanged(() => OutputFile);
                }
            }
        }

        /// <summary>
        /// Gets the export error.
        /// </summary>
        public string ExportErrorMessage
        {
            get
            {
                return _exportErrorMessage;
            }

            private set
            {
                if (_exportErrorMessage != value)
                {
                    _exportErrorMessage = value;
                    OnPropertyChanged(() => ExportErrorMessage);
                }
            }
        }
        #endregion

        #region Command handlers

        /// <summary>
        /// The select output file command action.
        /// </summary>
        private void SelectOutputFileCommandAction()
        {
            // Get the exporter.
            IBooksFileExport exporter = GetSelectedFileExporter();

            // Set up the save file dialog.
            SaveFileDialog fileDialog = new SaveFileDialog
            {
                Filter = exporter.Filter,
                FilterIndex = 4,
                RestoreDirectory = true
            };

            // If exporting set the output file.
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                OutputFile = fileDialog.FileName;
            }
        }

        /// <summary>
        /// The export file command action.
        /// </summary>
        private void ExportFileCommandAction()
        {
            // Get the exporter.
            IBooksFileExport exporter = GetSelectedFileExporter();

            // Get the data.
            GeographyProvider geographyProvider;
            BooksReadProvider booksReadProvider;
            if (GetProviders(out geographyProvider, out booksReadProvider))
            {
                string error;
                ExportErrorMessage = 
                    !exporter.WriteToFile(OutputFile, geographyProvider, booksReadProvider, out error) ? error : string.Empty;
            }
        }

        #endregion

        #region Utility Functions

        /// <summary>
        /// Gets the selected file exporter.
        /// </summary>
        /// <returns>The file exporter.</returns>
        private IBooksFileExport GetSelectedFileExporter()
        {
            Type exporterType = SelectedExportType.GetGeneratorClass();
            object instance = Activator.CreateInstance(exporterType);
            IBooksFileExport exporter = (IBooksFileExport)instance;
            return exporter;
        }

        /// <summary>
        /// Sets up the exporter types.
        /// </summary>
        private void SetupExportTypesByTitle()
        {
            ExportTypesByTitle = new Dictionary<ExportType, string>();
            foreach (ExportType selection in Enum.GetValues(typeof(ExportType)))
            {
                string title = selection.GetTitle();
                ExportTypesByTitle.Add(selection, title);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TesterViewModel"/> class. 
        /// </summary>
        public TesterViewModel()
        {
            _selectedExportType = ExportType.BooksToCsv;
            _outputFile = string.Empty;
            SetupExportTypesByTitle();
        }

        #endregion
    }
}
