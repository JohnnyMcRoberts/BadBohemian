namespace NextAzureWebApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    using BooksControllerUtilities;

    using BooksControllerUtilities.RequestsResponses;
    using BooksControllerUtilities.Settings;

    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        #region Private Data

        private readonly UserLoginControllerUtilities _userLoginController;

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

            UserLoginResponse response = _userLoginController.UserLogin(loginRequest);

            return Ok(response);
        }

        #endregion

        #region Constructor

        public UserLoginController(IOptions<MongoDbSettings> dbConfig)
        {
            MongoDbSettings mongoDbSettings = dbConfig.Value;
            _userLoginController = new UserLoginControllerUtilities(mongoDbSettings);
        }

        #endregion
    }
}