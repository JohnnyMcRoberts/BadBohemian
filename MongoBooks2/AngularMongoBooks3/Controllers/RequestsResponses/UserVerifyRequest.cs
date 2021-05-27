namespace AngularMongoBooks3.Controllers.RequestsResponses
{
    public class UserVerifyRequest
    {
        public string UserId { get; set; }
        public string ConfirmationCode { get; set; }
    }
}
