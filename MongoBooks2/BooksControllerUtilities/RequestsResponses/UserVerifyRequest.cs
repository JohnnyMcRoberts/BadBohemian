namespace BooksControllerUtilities.RequestsResponses
{
    public class UserVerifyRequest
    {
        public string UserId { get; set; }
        public string ConfirmationCode { get; set; }
    }
}
