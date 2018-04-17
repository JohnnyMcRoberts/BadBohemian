// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooksImportExport.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The selected month to html file exporter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksImportExport.Exporters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web.UI;
    
    using BooksCore.Books;
    using BooksCore.Interfaces;
    using BooksImportExport.Interfaces;

    public class SelectedMonthToHtmlFileExporter : IBooksFileExport
    {
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

        #region Private Data

        private TalliedMonth _selectedMonthTally;

        private IList<MonthlyReportsTally> _reportsTallies;

        #endregion

        #region Public Data

        /// <summary>
        /// Gets the export method name.
        /// </summary>
        public string Name => "Selected month to HTML";

        /// <summary>
        /// Gets the export file type extension.
        /// </summary>
        public string Extension => "html";

        /// <summary>
        /// Gets the export file type filter.
        /// </summary>
        public string Filter => @"All files (*.*)|*.*|HTML files (*.html)|*.html";

        public IList<MonthlyReportsTally> ReportsTallies => _reportsTallies;

        public bool ForBlog { get; set; }

        #endregion

        #region Utility Methods

        private void GetMonthlyReportTitleAndContent(out string title, out string content)
        {
            StringWriter sw = new StringWriter();

            HtmlTextWriter writer = new HtmlTextWriter(sw);

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

            title = _selectedMonthTally.DisplayString;
            content = sw.ToString();
        }

        private void WriteCharts(HtmlTextWriter writer)
        {
            writer.WriteLine();
            writer.AddStyleAttribute("font-size", "14pt");
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.Write("Charts");
            writer.RenderEndTag();
            writer.WriteLine();

            // Write script includes
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "pagesPieChart");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();

            // Write google charts script include
            writer.WriteLine();
            writer.AddAttribute(HtmlTextWriterAttribute.Src, "https://www.gstatic.com/charts/loader.js");
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.RenderEndTag();
            writer.WriteLine();

            // Get the json for chart

            Dictionary<string, int> pagesPerCountry = new Dictionary<string, int>();

            foreach (BookRead book in _selectedMonthTally.BooksRead)
            {
                if (pagesPerCountry.ContainsKey(book.Nationality))
                    pagesPerCountry[book.Nationality] += book.Pages;
                else
                    pagesPerCountry.Add(book.Nationality, book.Pages);

            }

            List<KeyValuePair<string, int>> sortedCountryTotals = pagesPerCountry.OrderByDescending(x => x.Value).ToList();

            string json = string.Empty;
            for (int i = 0; i < sortedCountryTotals.Count; i++)
            {
                if (i == 0)
                    json += "['Country', 'Pages']";

                json += ",\n";

                json += $"['{sortedCountryTotals[i].Key}', {sortedCountryTotals[i].Value}]";
            }

            // Write the script itself
            writer.WriteLine();
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.Write(@"// Load google charts
google.charts.load('current', {'packages':['corechart']});
google.charts.setOnLoadCallback(drawChart);

// Draw the chart and set the chart values
function drawChart() {
  var data = google.visualization.arrayToDataTable([
  "+ json + @"]);

  // Optional; add a title and set the width and height of the chart
  var options = {'title':'Pages by country', 'width':550, 'height':400};

  // Display the chart inside the <div> element with id=""piechart""
  var chart = new google.visualization.PieChart(document.getElementById('pagesPieChart'));
  chart.draw(data, options);
}");
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

        private void WriteIndividualBookTable(HtmlTextWriter writer, BookRead book)
        {
            writer.WriteLine();
            writer.AddStyleAttribute("font-size", "12pt");
            writer.AddStyleAttribute("width", ForBlog ? "90%" : "75%");
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

        private void AddBookImageTableCell(HtmlTextWriter writer, BookRead book)
        {
            writer.AddStyleAttribute("width", ForBlog ? "30%" : "20%");
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

        private void AddBookValueTableCell(HtmlTextWriter writer, string dataTitle, string dataValue)
        {
            writer.AddStyleAttribute("width", ForBlog ? "30%" : "20%");
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
            writer.AddStyleAttribute("width", ForBlog ? "90%" : "60%");
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

        private void WriteTotalsTableDataRow(HtmlTextWriter writer, Tuple<string, string, string> tableRow)
        {
            string rowHeader = tableRow.Item1;
            string rowOverallValue = tableRow.Item2;
            string rowMonthValue = tableRow.Item3;

            writer.AddStyleAttribute("border", "none");
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            {
                // Write the header cell.
                writer.AddStyleAttribute("width", ForBlog ? "30%" : "20%");
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

        private void WriteTotalsTableDataRowValueCell(HtmlTextWriter writer, string rowDataValue)
        {
            writer.AddStyleAttribute("width", ForBlog ? "30%" : "20%");
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

        private void WriteTotalsTableHeaderCell(HtmlTextWriter writer, string headerText)
        {
            writer.AddStyleAttribute("width", ForBlog ? "30%" : "20%");
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

        /// <summary>
        /// Writes the data for this export to the file specified.
        /// </summary>
        /// <param name="filename">The file to write to.</param>
        /// <param name="geographyProvider">The geography data provider.</param>
        /// <param name="booksReadProvider">The books data provider.</param>
        /// <param name="errorMessage">The error message if unsuccessful.</param>
        /// <returns>True if written successfully, false otherwise.</returns>
        public bool WriteToFile(
            string filename,
            IGeographyProvider geographyProvider,
            IBooksReadProvider booksReadProvider,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            _selectedMonthTally = booksReadProvider.SelectedMonthTally;
            _reportsTallies = booksReadProvider.ReportsTallies;

            try
            {
                // Set up the file content.
                string title;
                string content;
                GetMonthlyReportTitleAndContent(out title, out content);

                // Write out the content
                TextWriter textWriter = new StreamWriter(filename, false, Encoding.Default); //overwrite original file
                textWriter.Write(content);

                // Tidy up
                textWriter.Close();
            }
            catch (Exception e)
            {
                errorMessage = e.ToString();
                return false;
            }

            return true;
        }
    }
}