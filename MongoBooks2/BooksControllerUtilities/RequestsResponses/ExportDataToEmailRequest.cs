﻿namespace BooksControllerUtilities.RequestsResponses
{
    public class ExportDataToEmailRequest
    {
        public string DestinationEmail { get; set; }
        public string Format { get; set; }
        public string Source { get; set; }
    }
}
