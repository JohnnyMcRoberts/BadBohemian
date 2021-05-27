namespace AngularMongoBooks3.Controllers
{
    using System;
    using System.Linq;

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

        private bool CheckForUserVerificationErrors(
            UserVerifyRequest verifyRequest,
            User userToVerify,
            UserVerifyResponse response,
            out IActionResult actionResult)
        {
            // default the result value.
            actionResult = null;

            if (userToVerify.Verified)
            {
                response.ErrorCode = (int)UserResponseCode.AlreadyVerifiedUser;
                response.FailReason = "Cannot Verify as User already verified";
                {
                    actionResult = Ok(response);
                    return true;
                }
            }

            if (userToVerify.VerificationByTime > DateTime.Now)
            {
                response.ErrorCode = (int)UserResponseCode.VerificationCodeTimedOut;
                response.FailReason = "Cannot Verify as code has timed out";
                {
                    actionResult = Ok(response);
                    return true;
                }
            }

            if (userToVerify.VerificationCode.ToString() != verifyRequest.ConfirmationCode.Trim())
            {
                response.ErrorCode = (int)UserResponseCode.IncorrectConfirmationCode;
                response.FailReason = "Cannot Verify as code is incorrect";
                {
                    actionResult = Ok(response);
                    return true;
                }
            }

            return false;
        }
        
        #endregion

        #region HTTP Handlers

        /// <summary>
        /// Adds a new user login.
        /// </summary>
        /// <param name="addRequest">The new user login to try to add.</param>
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

        /// <summary>
        /// Verifies a new user login.
        /// </summary>
        /// <param name="verifyRequest">The new user login to try to verify.</param>
        /// <returns>The action result.</returns>
        [HttpPost("VerifyNewUser")]
        public IActionResult VerifyNewUser([FromBody] UserVerifyRequest verifyRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserVerifyResponse response = new UserVerifyResponse
            {
                ErrorCode = (int)UserResponseCode.Success,
                FailReason = "",
                UserId = ""
            };

            User userToVerify = 
                _userDatabase.LoadedItems.FirstOrDefault(x => x.Id.ToString() == verifyRequest.UserId);

            if (userToVerify == null)
            {
                response.ErrorCode = (int)UserResponseCode.UnknownItem;
                response.FailReason = "Cannot Verify this unknown user";
                return Ok(response);
            }

            IActionResult actionResult;
            if (CheckForUserVerificationErrors(verifyRequest, userToVerify, response, out actionResult)) 
                return actionResult;

            // If here a valid user so set the flag and update.
            userToVerify.Verified = true;
            _userDatabase.UpdateDatabaseItem(userToVerify);

            // Send the successful fully populated response.
            response.Name = userToVerify.Name;
            response.Description = userToVerify.Description;
            response.Email = userToVerify.Email;
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