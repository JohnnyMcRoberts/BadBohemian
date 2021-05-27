namespace AngularMongoBooks3.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    using AngularMongoBooks3.Controllers.RequestsResponses;
    using AngularMongoBooks3.Controllers.Settings;

    using BooksCore.Users;
    using BooksDatabase.Implementations;
    using BooksMailbox;

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        #region Constants

        private readonly string AuthCodeText = "AUTH-CODE";
        
        private readonly string ActivateTimeText = "ACTIVATE-TIME";

        private readonly string MessageSubject = "McBob's Books Authentication";

        private readonly string MessageBodyTop = 
            @"<body>
                <p>Please enter the following code to activate the new user login:</p>";

        private readonly string MessageBodyAuthCode =
            @"<h4>AUTH-CODE</h4>";

        private readonly string MessageBodyActivateTime =
            @"<p>This activation code will expire at: <b>ACTIVATE-TIME</b><p>";

        private readonly string MessageBodyBottom = 
            @"<p>Thanks and enjoy the trip!<p>
                </body>";

        #endregion

        #region Private Data

        private readonly UserDatabase _userDatabase;

        private readonly SmtpConfig _smtpConfig;

        #endregion

        #region Local Methods

        private void SendAuthorizationEmail(User newUser)
        {
            // Set up the message
            string messageBody = MessageBodyTop;
            
            string messageAuth = 
                MessageBodyAuthCode.Replace(AuthCodeText, newUser.VerificationCode.ToString());
            messageBody += messageAuth;

            string messageTime =
                MessageBodyActivateTime.Replace(ActivateTimeText, newUser.VerificationByTime.ToString("HH:mm:ss tt"));
            messageBody += messageTime;

            messageBody += MessageBodyBottom;

            // Set up the mail connection parameters
            StmpConnection connection =
                new StmpConnection(_smtpConfig.EmailAddress, _smtpConfig.Password);

            // Set up the message parameters
            HtmlEmailDefinition emailDefinition =
                new HtmlEmailDefinition
                {
                    FromEmailDisplayName = _smtpConfig.Username,
                    ToEmail = newUser.Email,
                    ToEmailDisplayName = newUser.Name,
                    Subject = MessageSubject,
                    BodyHtml = messageBody
                };

            SmtpEmailer.SendHtmlEmail(connection, emailDefinition);
        }

        #endregion

        #region HTTP Handlers

        /// <summary>
        /// Adds a new user login.
        /// </summary>
        /// <param name="addRequest">The check new user login to try to add.</param>
        /// <returns>The action result.</returns>
        [HttpPost("AddNewUser")]
        public IActionResult AddNewUser([FromBody] UserAddRequest addRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserAddResponse response = new UserAddResponse
            {
                ErrorCode = (int)UserResponseCode.Success,
                Name = addRequest.Name,
                FailReason = "",
                UserId = ""
            };

            if (_userDatabase.LoadedItems.Any(x => x.Name == addRequest.Name))
            {
                response.ErrorCode = (int)UserResponseCode.DuplicateName;
                response.FailReason = "Cannot Add as User name already exists";
                return Ok(response);
            }

            if (_userDatabase.LoadedItems.Any(x => x.Email == addRequest.Email))
            {
                response.ErrorCode = (int)UserResponseCode.DuplicateEmail;
                response.FailReason = "Cannot Add as User e-mail already exists";
                return Ok(response);
            }

            User newUser =
                new User(addRequest.Name, addRequest.Password, addRequest.Email, addRequest.Description);
            _userDatabase.AddNewItemToDatabase(newUser);

            // Send the authorization e-mail.
            SendAuthorizationEmail(newUser);

            // return the unverified e-mail to the user display
            response.UserId = newUser.Id.ToString();
            return Ok(response);
        }

        #endregion

        #region Constructor

        public UsersController(
            IOptions<MongoDbSettings> dbConfig,
            IOptions<SmtpConfig> mailConfig)
        {
            MongoDbSettings mongoDbSettings = dbConfig.Value;
            _smtpConfig = mailConfig.Value;
            _userDatabase = new UserDatabase(mongoDbSettings.DatabaseConnectionString);
        }

        #endregion
    }
}