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
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows.Forms;
    using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

    using BooksCore.Provider;
    using BooksCore.Interfaces;
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
        private GeographyProvider _geographyProvider;

        /// <summary>
        /// The output file.
        /// </summary>
        private BooksReadProvider _booksReadProvider;

        /// <summary>
        /// The output file.
        /// </summary>
        private string _outputFile;

        /// <summary>
        /// The selected export type.
        /// </summary>
        private ExportType _selectedExportType;

        /// <summary>
        /// The export file error.
        /// </summary>
        private string _exportErrorMessage;

        /// <summary>
        /// The selected datae for the month books exports.
        /// </summary>
        private DateTime _selectedMonth;

        /// <summary>
        /// The first available date with books.
        /// </summary>
        private DateTime _firstMonth;

        /// <summary>
        /// The last available date with books.
        /// </summary>
        private DateTime _lastMonth;

        /// <summary>
        /// The refresh date range command.
        /// </summary>
        private ICommand _refreshDateRangeCommand;

        /// <summary>
        /// The select output file command.
        /// </summary>
        private ICommand _selectOutputFileCommand;

        /// <summary>
        /// The export file command.
        /// </summary>
        private ICommand _exportFileCommand;

        /// <summary>
        /// The input file.
        /// </summary>
        private string _inputFile;

        /// <summary>
        /// The selected import type.
        /// </summary>
        private ImportType _selectedImportType;

        /// <summary>
        /// The import data table.
        /// </summary>
        private DataTable _importDataTable;

        /// <summary>
        /// The import file error.
        /// </summary>
        private string _importErrorMessage;

        /// <summary>
        /// The select input file command.
        /// </summary>
        private ICommand _selectInputFileCommand;

        /// <summary>
        /// The import file command.
        /// </summary>
        private ICommand _importFileCommand;

        #endregion

        #region Public data

        /// <summary>
        /// Gets or sets the column chart type.
        /// </summary>
        public IBooksReadProvider BooksReadProvider
        {
            get
            {
                if (_booksReadProvider == null)
                {
                    GetProviders(out _geographyProvider, out _booksReadProvider);
                }

                return _booksReadProvider;
            }
        }

        /// <summary>
        /// Gets or sets the column chart type.
        /// </summary>
        public IGeographyProvider GeographyProvider
        {
            get
            {
                if (_geographyProvider == null)
                {
                    GetProviders(out _geographyProvider, out _booksReadProvider);
                }

                return _geographyProvider;
            }
        }

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
        /// Gets the refresh date range command.
        /// </summary>
        public ICommand RefreshDateRangeCommand =>
            _refreshDateRangeCommand ?? (_refreshDateRangeCommand = new CommandHandler(RefreshDateRangeCommandAction, true));

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
        /// Gets the select input file command.
        /// </summary>
        public ICommand SelectInputFileCommand =>
            _selectInputFileCommand ?? (_selectInputFileCommand = new CommandHandler(SelectInputFileCommandAction, true));

        /// <summary>
        /// Gets the export file command.
        /// </summary>
        public ICommand ImportFileCommand =>
            _importFileCommand ?? (_importFileCommand = new CommandHandler(ImportFileCommandAction, true));

        /// <summary>
        /// Gets the first month.
        /// </summary>
        public DateTime LastMonth
        {
            get
            {
                return _lastMonth;
            }

            private set
            {
                if (_lastMonth != value)
                {
                    _lastMonth = value;
                    OnPropertyChanged(() => LastMonth);
                }
            }
        }

        /// <summary>
        /// Gets the first month.
        /// </summary>
        public DateTime FirstMonth
        {
            get
            {
                return _firstMonth;
            }

            private set
            {
                if (_firstMonth != value)
                {
                    _firstMonth = value;
                    OnPropertyChanged(() => FirstMonth);
                }
            }
        }

        /// <summary>
        /// Gets the selected month.
        /// </summary>
        public DateTime SelectedMonth
        {
            get
            {
                return _selectedMonth;
            }

            set
            {
                if (_selectedMonth != value)
                {
                    _selectedMonth = value;
                    OnPropertyChanged(() => SelectedMonth);

                    // Get the data.
                    if (BooksReadProvider != null)
                    {
                        BooksReadProvider.SelectedMonth = _selectedMonth;
                    }
                }
            }
        }

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

        /// <summary>
        /// Gets the input file command.
        /// </summary>
        public string InputFile
        {
            get
            {
                return _inputFile;
            }

            private set
            {
                if (_inputFile != value)
                {
                    _inputFile = value;
                    OnPropertyChanged(() => IsValidInputFile);
                    OnPropertyChanged(() => InputFile);
                }
            }
        }

        /// <summary>
        /// Gets the import error.
        /// </summary>
        public string ImportErrorMessage
        {
            get
            {
                return _importErrorMessage;
            }

            private set
            {
                if (_importErrorMessage != value)
                {
                    _importErrorMessage = value;
                    OnPropertyChanged(() => ImportErrorMessage);
                }
            }
        }

        /// <summary>
        /// Gets or sets the import type.
        /// </summary>
        public ImportType SelectedImportType
        {
            get
            {
                return _selectedImportType;
            }

            set
            {
                if (value != _selectedImportType)
                {
                    _selectedImportType = value;
                    OnPropertyChanged(() => SelectedImportType);
                    OutputFile = String.Empty;
                }
            }
        }

        /// <summary>
        /// Gets if have a valid input file set.
        /// </summary>
        public bool IsValidInputFile => !string.IsNullOrEmpty(InputFile);

        /// <summary>
        /// Gets the import types and titles.
        /// </summary>
        public Dictionary<ImportType, string> ImportTypesByTitle { get; private set; }

        /// <summary>
        /// Gets the import data table.
        /// </summary>
        public DataTable ImportDataTable
        {
            get
            {
                return _importDataTable;
            }

            private set
            {
                _importDataTable = value;
                OnPropertyChanged(() => ImportDataTable);
            }
        }

        #endregion

        #region Command handlers

        /// <summary>
        /// The refresh date range command action.
        /// </summary>
        private void RefreshDateRangeCommandAction()
        {
            // Get the data.
            if (BooksReadProvider == null)
            {
                return;
            }

            DateTime lastMonth = FirstMonth = BooksReadProvider.TalliedMonths.Last().MonthDate;
            FirstMonth = BooksReadProvider.TalliedMonths.First().MonthDate;
            lastMonth = lastMonth.AddMonths(1);
            lastMonth = lastMonth.AddDays(-1);
            LastMonth = lastMonth;
            _selectedMonth = lastMonth.AddMonths(-1);
        }

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
            if (BooksReadProvider != null)
            {
                string error;
                ExportErrorMessage = 
                    !exporter.WriteToFile(OutputFile, GeographyProvider, BooksReadProvider, out error) ? error : string.Empty;
            }
        }

        /// <summary>
        /// The select output file command action.
        /// </summary>
        private void SelectInputFileCommandAction()
        {
            // Get the exporter.
            IBooksFileImport importer = GetSelectedFileImporter();

            // Set up the save file dialog.
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = importer.Filter,
                FilterIndex = 4,
                RestoreDirectory = true
            };

            // If exporting set the output file.
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                InputFile = fileDialog.FileName;
            }
        }

        /// <summary>
        /// The import file command action.
        /// </summary>
        private void ImportFileCommandAction()
        {
            // Get the exporter.
            IBooksFileImport importer = GetSelectedFileImporter();

            // Get the data.
            string error;
            if (!importer.ReadFromFile(InputFile, out error))
            {
                ImportErrorMessage = error;
            }
            else
            {
                ImportErrorMessage = string.Empty;
                Type importeditemType = importer.ImportType;
                var importedItems = new List<object>();
                foreach(var item in importer.ImportedItems)
                    importedItems.Add(item);

                ImportDataTable = ToDataTable(importedItems, importeditemType);

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

        /// <summary>
        /// Gets a data table from a list of items of a given type.
        /// </summary>
        public static DataTable ToDataTable(IList<object> data, Type itemType)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(itemType);
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (object item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// Gets a data table from a list of items of a given type.
        /// </summary>
        public static DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// Gets the selected file exporter.
        /// </summary>
        /// <returns>The file exporter.</returns>
        private IBooksFileImport GetSelectedFileImporter()
        {
            Type importerType = SelectedImportType.GetGeneratorClass();
            object instance = Activator.CreateInstance(importerType);
            IBooksFileImport importer = (IBooksFileImport)instance;
            return importer;
        }

        /// <summary>
        /// Sets up the importer types.
        /// </summary>
        private void SetupImportTypesByTitle()
        {
            ImportTypesByTitle = new Dictionary<ImportType, string>();
            foreach (ImportType selection in Enum.GetValues(typeof(ImportType)))
            {
                string title = selection.GetTitle();
                ImportTypesByTitle.Add(selection, title);
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
            LastMonth = DateTime.Now;
            SelectedMonth = LastMonth.AddDays(-2);
            FirstMonth = SelectedMonth.AddDays(-2);

            _selectedImportType = ImportType.BooksFromCsv;
            _inputFile = string.Empty;
            SetupImportTypesByTitle();
            ImportDataTable = new DataTable();
        }

        #endregion
    }
}
