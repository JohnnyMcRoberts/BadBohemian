// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileExporter.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The interface for the file exporter classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace MongoDbBooks.Models.Exporters
{
    public interface IFileExporter
    {
        string OutputFilePath { get; set; }

        string GetFilter();

        bool GetNewFileName(string filter, out string newFileName);

        bool WriteToFile(string filename);

        bool WriteToFile(out string fileName);
    }
}
