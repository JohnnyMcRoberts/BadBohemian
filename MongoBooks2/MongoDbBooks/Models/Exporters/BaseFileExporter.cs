// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseFileExporter.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The abstract base file writer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace MongoDbBooks.Models.Exporters
{
    using System.IO;
    using System.Windows.Forms;

    public abstract class BaseFileExporter : IFileExporter
    {
        public string OutputFilePath { get; set; }

        public abstract bool WriteToFile(string filename);
        public abstract string GetFilter();

        public bool GetNewFileName(string filter, out string newFileName)
        {
            newFileName = null;
            if (string.IsNullOrEmpty(OutputFilePath))
            {
                OutputFilePath = Path.GetTempPath();
            }

            SaveFileDialog fileDialog = new SaveFileDialog
            {
                FileName = OutputFilePath,
                Filter = filter,
                FilterIndex = 4,
                RestoreDirectory = true
            };

            if (fileDialog.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            newFileName = fileDialog.FileName;
            return true;
        }

        public bool WriteToFile(out string fileName)
        {
            if (GetNewFileName(GetFilter(), out fileName))
            {
                return WriteToFile(fileName);
            }

            return false;
        }
    }
}
