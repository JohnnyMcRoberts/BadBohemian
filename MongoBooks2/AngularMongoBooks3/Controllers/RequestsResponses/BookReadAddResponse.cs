namespace AngularMongoBooks3.Controllers.RequestsResponses
{
    using System;
    using AngularMongoBooks3.Controllers.DataClasses;

    public enum BookReadAddResponseCode
    {
        Success = 0,
        Duplicate,
        UnknownUser,
        InvalidItem
    };

    public class BookReadAddResponse
    {
        public Book NewItem { get; set; }

        public int ErrorCode { get; set; }

        public string FailReason { get; set; }

        public string UserId { get; set; }
    }
}
