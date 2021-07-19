namespace BooksControllerUtilities.Utilities
{
    using System.IO;
    using System.Text;

    public sealed class ExtendedStringWriter : StringWriter
    {
        private readonly Encoding _stringWriterEncoding;

        public ExtendedStringWriter(StringBuilder builder, Encoding desiredEncoding)
            : base(builder)
        {
            _stringWriterEncoding = desiredEncoding;
        }

        public override Encoding Encoding => _stringWriterEncoding;


        public static string GetSafeString(string fieldValue)
        {
            if (string.IsNullOrWhiteSpace(fieldValue))
            {
                return string.Empty;
            }

            return fieldValue;
        }
    }
}
