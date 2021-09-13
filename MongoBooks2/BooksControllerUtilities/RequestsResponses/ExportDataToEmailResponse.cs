namespace BooksControllerUtilities.RequestsResponses
{
    public class ExportDataToEmailResponse
    {
        public string DestinationEmail { get; set; }

        public bool SentSuccessfully { get; set; }

        public string Error { get; set; }
    }
}
