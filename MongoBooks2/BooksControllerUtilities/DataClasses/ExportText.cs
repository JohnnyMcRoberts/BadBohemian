namespace BooksControllerUtilities.DataClasses
{
    public class ExportText
    {
        public string Format { get; set; }
        public string FormattedText { get; set; }

        public ExportText()
        {
            Format = string.Empty;
            FormattedText = string.Empty;
        }
    }
}
