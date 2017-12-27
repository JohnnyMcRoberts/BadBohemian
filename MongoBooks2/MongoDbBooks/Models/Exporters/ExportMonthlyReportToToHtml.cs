// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportMonthlyReportToToHtml.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The file exporter to get an html file for a selected monthly report.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MongoDbBooks.Models.Exporters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using MongoDbBooks.ViewModels;
    using MongoDbBooks.ViewModels.PlotGenerators;

    public class ExportMonthlyReportToToHtml : BaseFileExporter
    {
        #region Private Data

        private readonly TalliedMonth _selectedMonthTally;

        private readonly IList<ReportsViewModel.MonthlyReportsTally> _reportsTallies;

        private readonly IList<string> _chartFiles;

        private readonly bool _forBlog;

        #endregion

        #region Public Data

        public string Filter => @"All files (*.*)|*.*|HTML files (*.html)|*.html";

        public IList<ReportsViewModel.MonthlyReportsTally> ReportsTallies => _reportsTallies;

        #endregion

        #region Constants

        public const string StylesString =
@"
table, th, td {
    border: 1px solid black;
    border-collapse: collapse;
}
th {
    padding: 5px;
    text-align: left;    
}
td {
    padding: 5px;
    text-align: left;    
}";

        #endregion

        #region IFileExporter overides

        public override string GetFilter()
        {
            return Filter;
        }

        public override bool WriteToFile(string filename)
        {
            Properties.Settings.Default.MonthlyReportFile = filename;
            Properties.Settings.Default.Save();

            StreamWriter sw = new StreamWriter(filename, false, Encoding.Default); //overwrite original file
            
            HtmlTextWriter writer = new HtmlTextWriter(sw);

            writer.RenderBeginTag(HtmlTextWriterTag.Html);
            
            writer.RenderBeginTag(HtmlTextWriterTag.Head);

            WriteHeaderContent(writer);

            writer.RenderEndTag();
            writer.WriteLine();

            writer.RenderBeginTag(HtmlTextWriterTag.Body);

            WriteTitle(writer);
            WriteIndividualBooks(writer);
            WriteTotalsTable(writer);
            WriteCharts(writer);

            writer.RenderEndTag();

            writer.RenderEndTag();

            // tidy up
            sw.Close();

            return true;
        }

        #endregion

        #region Utility Methods

        private static void WriteCharts(HtmlTextWriter writer)
        {
            writer.WriteLine();
            writer.AddStyleAttribute("font-size", "14pt");
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.Write("Charts");
            
            writer.RenderEndTag();
            writer.WriteLine();
        }

        private static void WriteHeaderContent(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Name, "viewport");
            writer.AddAttribute(HtmlTextWriterAttribute.Content, "width=device-width, initial-scale=1");
            writer.RenderBeginTag(HtmlTextWriterTag.Meta);
            writer.RenderEndTag();
            writer.WriteLine();

            writer.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
            writer.AddAttribute(HtmlTextWriterAttribute.Href,
                "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css");
            writer.RenderBeginTag(HtmlTextWriterTag.Link);
            writer.RenderEndTag();
            writer.WriteLine();

            writer.AddAttribute(HtmlTextWriterAttribute.Src,
                "https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js");
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.RenderEndTag();
            writer.WriteLine();


            writer.AddAttribute(HtmlTextWriterAttribute.Src,
                "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js");
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.RenderEndTag();
            writer.WriteLine();

            writer.RenderBeginTag(HtmlTextWriterTag.Style);
            writer.Write(StylesString);
            writer.RenderEndTag();
            writer.WriteLine();
        }

        private void WriteTitle(HtmlTextWriter writer)
        {
            writer.WriteLine();
            writer.RenderBeginTag(HtmlTextWriterTag.H1);
            writer.Write("Monthly Report for " + _selectedMonthTally.DisplayString);
            writer.RenderEndTag();
            writer.WriteLine();
        }

        private void WriteIndividualBooks(HtmlTextWriter writer)
        {
            writer.WriteLine();
            writer.AddStyleAttribute("font-size", "14pt");
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.Write("Individual books");

            foreach (BookRead book in _selectedMonthTally.BooksRead)
            {
                WriteIndividualBookTable(writer, book);
            }

            writer.RenderEndTag();
            writer.WriteLine();
        }

        private static void WriteIndividualBookTable(HtmlTextWriter writer, BookRead book)
        {
            writer.WriteLine();
            writer.AddStyleAttribute("font-size", "12pt");
            writer.AddStyleAttribute("width", "75%");
            writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                {
                    AddBookImageTableCell(writer, book);
                    AddBookValueTableCell(writer, "Title: ", book.Title);
                }
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                {
                    AddBookValueTableCell(writer, "Author: ", book.Author);
                }
                writer.RenderEndTag();
                writer.WriteLine();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                {
                    AddBookValueTableCell(writer, "Pages: ", book.Pages.ToString());
                }
                writer.RenderEndTag();
                writer.WriteLine();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                {
                    AddBookValueTableCell(writer, "Nationality: ", book.Nationality);
                }
                writer.RenderEndTag();
                writer.WriteLine();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                {
                    AddBookValueTableCell(writer, "Original Language: ", book.OriginalLanguage);
                }
                writer.RenderEndTag();
                writer.WriteLine();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                {
                    AddBookValueTableCell(writer, "Date: ", book.DateString);
                }
                writer.RenderEndTag();
                writer.WriteLine();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                {
                    AddBookValueTableCell(writer, "Format: ", book.Format.ToString());
                }
                writer.RenderEndTag();
                writer.WriteLine();

                if (!string.IsNullOrEmpty(book.Note))
                {
                    AddBookNotesToTableCells(writer, book);
                }

            }
            writer.RenderEndTag();

            writer.WriteLine();
            writer.RenderBeginTag(HtmlTextWriterTag.Br);
            writer.RenderEndTag();
            writer.WriteLine();
        }

        private static void AddBookNotesToTableCells(HtmlTextWriter writer, BookRead book)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Th);
            {
                writer.RenderBeginTag(HtmlTextWriterTag.B);
                writer.Write("Notes: ");
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
            writer.WriteLine();

            writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "2");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            {
                writer.Write(book.Note);
            }
            writer.RenderEndTag();
            writer.WriteLine();
        }

        private static void AddBookImageTableCell(HtmlTextWriter writer, BookRead book)
        {
            writer.AddStyleAttribute("width", "20%");
            writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, "7");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Src, book.ImageUrl);
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, "No Image Provided");
                writer.AddAttribute(HtmlTextWriterAttribute.Height, "200");
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        private static void AddBookValueTableCell(HtmlTextWriter writer, string dataTitle, string dataValue)
        {
            writer.AddStyleAttribute("width", "20%");
            writer.RenderBeginTag(HtmlTextWriterTag.Th);
            {
                writer.RenderBeginTag(HtmlTextWriterTag.B);
                writer.Write(dataTitle);
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
            writer.WriteLine();

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            {
                writer.Write(dataValue);
            }
            writer.RenderEndTag();
        }

        private void WriteTotalsTable(HtmlTextWriter writer)
        {
            writer.WriteLine();
            writer.AddStyleAttribute("font-size", "14pt");
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.Write("Tables");
            writer.RenderEndTag();
            writer.WriteLine();

            writer.AddStyleAttribute("font-size", "12pt");
            writer.AddStyleAttribute("width", "60%");
            writer.AddStyleAttribute("border", "none");

            List<Tuple<string, string, string>> tableRowValues = GetTotalsTableRowValues();

            writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            {
                // Write the  main header row.
                writer.AddStyleAttribute("border", "none");
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "3");
                    writer.AddStyleAttribute("font-size", "24pt");
                    writer.AddStyleAttribute("text-align", "center");
                    writer.AddStyleAttribute("background-color", "Silver");
                    writer.AddStyleAttribute("border", "none");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    {
                        writer.Write("Monthly Totals");
                    }
                    writer.RenderEndTag();
                    writer.WriteLine();
                }
                writer.RenderEndTag();
                writer.WriteLine();

                // Write the sub-headers row.
                writer.AddStyleAttribute("border", "none");
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                {
                    WriteTotalsTableHeaderCell(writer, string.Empty);
                    WriteTotalsTableHeaderCell(writer, ReportsTallies[0].DisplayTitle);
                    WriteTotalsTableHeaderCell(writer, ReportsTallies[1].DisplayTitle);
                }
                writer.RenderEndTag();
                writer.WriteLine();

                foreach (Tuple<string, string, string> tableRow in tableRowValues)
                {
                    WriteTotalsTableDataRow(writer, tableRow);
                    writer.WriteLine();
                }
            }
            writer.RenderEndTag();

            writer.WriteLine();
        }

        private static void WriteTotalsTableDataRow(HtmlTextWriter writer, Tuple<string, string, string> tableRow)
        {
            string rowHeader = tableRow.Item1;
            string rowOverallValue = tableRow.Item2;
            string rowMonthValue = tableRow.Item3;

            writer.AddStyleAttribute("border", "none");
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            {
                // Write the header cell.
                writer.AddStyleAttribute("width", "20%");
                writer.AddStyleAttribute("text-align", "left");
                writer.AddStyleAttribute("background-color", "Beige");
                writer.AddStyleAttribute("border", "none");
                writer.RenderBeginTag(HtmlTextWriterTag.Th);
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.B);
                    writer.Write(rowHeader);
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();
                writer.WriteLine();

                // Write the data value cells.
                WriteTotalsTableDataRowValueCell(writer, rowOverallValue);
                WriteTotalsTableDataRowValueCell(writer, rowMonthValue);
            }
            writer.RenderEndTag();
        }

        private static void WriteTotalsTableDataRowValueCell(HtmlTextWriter writer, string rowDataValue)
        {
            writer.AddStyleAttribute("width", "20%");
            writer.AddStyleAttribute("text-align", "right");
            writer.AddStyleAttribute("background-color", "LightSteelBlue");
            writer.AddStyleAttribute("border", "none");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            {
                writer.Write(rowDataValue);
            }
            writer.RenderEndTag();
            writer.WriteLine();
        }

        private static void WriteTotalsTableHeaderCell(HtmlTextWriter writer, string headerText)
        {
            writer.AddStyleAttribute("width", "20%");
            writer.AddStyleAttribute("text-align", "center");
            writer.AddStyleAttribute("font-size", "18pt");
            writer.AddStyleAttribute("background-color", string.IsNullOrEmpty(headerText) ? "Beige" : "LightSteelBlue");
            writer.AddStyleAttribute("border", "none");
            writer.RenderBeginTag(HtmlTextWriterTag.Th);
            {
                writer.RenderBeginTag(HtmlTextWriterTag.B);
                writer.Write(headerText);
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
            writer.WriteLine();
        }

        private List<Tuple<string, string, string>> GetTotalsTableRowValues()
        {
            List<Tuple<string, string, string>> rowValues = new List<Tuple<string, string, string>>
            {
                new Tuple<string, string, string>(
                    "Days", ReportsTallies[0].TotalDays.ToString(), ReportsTallies[1].TotalDays.ToString()),
                new Tuple<string, string, string>(
                    "Total Books", ReportsTallies[0].TotalBooks.ToString(), ReportsTallies[1].TotalBooks.ToString()),
                new Tuple<string, string, string>(
                    "Book", ReportsTallies[0].TotalBookFormat.ToString(), ReportsTallies[1].TotalBookFormat.ToString()),
                new Tuple<string, string, string>(
                    "Comic", ReportsTallies[0].TotalComicFormat.ToString(), ReportsTallies[1].TotalComicFormat.ToString()),
                new Tuple<string, string, string>(
                    "Audio", ReportsTallies[0].TotalAudioFormat.ToString(), ReportsTallies[1].TotalAudioFormat.ToString()),
                new Tuple<string, string, string>(
                    "% in English", ReportsTallies[0].PercentageInEnglish.ToString("N2"), ReportsTallies[1].PercentageInEnglish.ToString("N2")),
                new Tuple<string, string, string>(
                    "% in Translation", ReportsTallies[0].PercentageInTranslation.ToString("N2"), ReportsTallies[1].PercentageInTranslation.ToString("N2")),
                new Tuple<string, string, string>(
                    "Page Rate", ReportsTallies[0].PageRate.ToString("N2"), ReportsTallies[1].PageRate.ToString("N2")),
                new Tuple<string, string, string>(
                    "Days per Book", ReportsTallies[0].DaysPerBook.ToString("N2"), ReportsTallies[1].DaysPerBook.ToString("N2")),
                new Tuple<string, string, string>(
                    "Pages per Book", ReportsTallies[0].PagesPerBook.ToString("N2"), ReportsTallies[1].PagesPerBook.ToString("N2")),
                new Tuple<string, string, string>(
                    "Books per Year", ReportsTallies[0].BooksPerYear.ToString("N2"), ReportsTallies[1].BooksPerYear.ToString("N2"))
            };
            return rowValues;
        }

        #endregion

        #region Constructors

        public ExportMonthlyReportToToHtml(
            TalliedMonth selectedMonthTally,
            IList<ReportsViewModel.MonthlyReportsTally> reportsTallies,
            IList<string> chartFiles,
            bool forBlog = false)
        {
            _forBlog = forBlog;
            _selectedMonthTally = selectedMonthTally;
            _reportsTallies = reportsTallies;
            _chartFiles = chartFiles;
            OutputFilePath = Properties.Settings.Default.MonthlyReportFile;
        }

        #endregion
    }
}
