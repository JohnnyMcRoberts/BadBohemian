namespace BooksControllerUtilities.RequestsResponses
{
    using BooksControllerUtilities.DataClasses;

    public enum BookReadAddResponseCode
    {
        Success = 0,
        Duplicate,
        UnknownUser,
        InvalidItem,
        UnknownItem
    };

    public class BookReadAddResponse
    {
        public Book NewItem { get; set; }

        public int ErrorCode { get; set; }

        public string FailReason { get; set; }

        public string UserId { get; set; }
    }
}
