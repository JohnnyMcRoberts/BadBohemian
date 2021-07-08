namespace AngularMongoBooks3.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    using BooksControllerUtilities.RequestsResponses;
    using BooksControllerUtilities.Settings;

    using BooksControllerUtilities;

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        #region Private Data

        private readonly UsersControllerUtilities _usersControllerUtilities;
        
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

            UserAddResponse response = _usersControllerUtilities.AddNewUser(addRequest);

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

            UserVerifyResponse response = _usersControllerUtilities.VerifyNewUser(verifyRequest);

            return Ok(response);
        }

        #endregion

        #region Constructor

        public UsersController(
            IOptions<MongoDbSettings> dbConfig,
            IOptions<SmtpConfig> mailConfig)
        {
            _usersControllerUtilities = new UsersControllerUtilities(dbConfig.Value, mailConfig.Value);
        }

        #endregion
    }
}