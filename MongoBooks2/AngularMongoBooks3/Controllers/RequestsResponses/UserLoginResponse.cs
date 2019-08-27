namespace AngularMongoBooks3.Controllers.RequestsResponses
{
    public class UserLoginResponse
    {
        public string Name { get; set; }

        public int ErrorCode { get; set; }

        public string FailReason { get; set; }

        public string UserId { get; set; }

        public string Description { get; set; }

        public string Email { get; set; }

        public UserLoginResponse()
        {
            Name = string.Empty;
            ErrorCode = (int)UserResponseCode.Success;
            FailReason = string.Empty;
            UserId = string.Empty;
            Description = string.Empty;
            Email = string.Empty;
        }
    }
    }
