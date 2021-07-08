namespace BooksControllerUtilities.RequestsResponses
{
    public class UserVerifyResponse
    {
        public string UserId { get; set; }

        public int ErrorCode { get; set; }

        public string FailReason { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Email { get; set; }
    }
}
