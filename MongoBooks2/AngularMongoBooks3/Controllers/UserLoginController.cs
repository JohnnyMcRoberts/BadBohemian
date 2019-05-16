namespace AngularMongoBooks3.Controllers
{
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    using AngularMongoBooks3.Controllers.RequestsResponses;
    using AngularMongoBooks3.Controllers.Settings;

    using BooksCore.Users;
    using BooksDatabase.Implementations;

    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        #region Private Data

        private readonly UserDatabase _userDatabase;

        #endregion

        #region HTTP Handlers

        /// <summary>
        /// Adds a new user login.
        /// </summary>
        /// <param name="loginRequest">The check new user login to try to add.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public IActionResult Post([FromBody] UserLoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserLoginResponse response = new UserLoginResponse { Name = loginRequest.Name };

            // First check that the user exists
            User userLogin = _userDatabase.LoadedItems.FirstOrDefault(x => x.Name == loginRequest.Name);

            if (userLogin == null)
            {
                response.ErrorCode = (int)UserResponseCode.UnknownUser;
                response.FailReason = "Could not find this user.";
                return Ok(response);
            }

            // Check the password
            if (!userLogin.VerifyPassword(loginRequest.Password))
            {
                response.ErrorCode = (int)UserResponseCode.IncorrectPassword;
                response.FailReason = "Incorrect password please try again.";
                return Ok(response);
            }

            // Correct password so populate the login response
            response.UserId = userLogin.Id.ToString();
            response.Description = userLogin.Description;
            response.Email = userLogin.Email;

            return Ok(response);
        }

        #endregion

        #region Constructor

        public UserLoginController(IOptions<MongoDbSettings> dbConfig)
        {
            MongoDbSettings mongoDbSettings = dbConfig.Value;
            _userDatabase = new UserDatabase(mongoDbSettings.DatabaseConnectionString);
        }

        #endregion
    }
}
