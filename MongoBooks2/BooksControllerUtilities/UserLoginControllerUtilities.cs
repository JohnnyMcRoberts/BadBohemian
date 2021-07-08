// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserLoginControllerUtilities.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The utilities class for the UserLoginController.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BooksControllerUtilities
{
    using System.Linq;

    using BooksCore.Users;

    using BooksDatabase.Implementations;

    using BooksControllerUtilities.RequestsResponses;
    using BooksControllerUtilities.Settings;

    public class UserLoginControllerUtilities : BaseControllerUtilities
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
        public UserLoginResponse UserLogin(UserLoginRequest loginRequest)
        {
            UserLoginResponse response = new UserLoginResponse { Name = loginRequest.Name };

            // First check that the user exists
            User userLogin = _userDatabase.LoadedItems.FirstOrDefault(x => x.Name == loginRequest.Name);

            if (userLogin == null)
            {
                response.ErrorCode = (int)UserResponseCode.UnknownUser;
                response.FailReason = "Could not find this user.";
                return response;
            }

            // Check the password
            if (!userLogin.VerifyPassword(loginRequest.Password))
            {
                response.ErrorCode = (int)UserResponseCode.IncorrectPassword;
                response.FailReason = "Incorrect password please try again.";
                return response;
            }

            // Correct password so populate the login response
            response.UserId = userLogin.Id.ToString();
            response.Description = userLogin.Description;
            response.Email = userLogin.Email;

            return response;
        }

        #endregion

        #region Constructor

        public UserLoginControllerUtilities(MongoDbSettings mongoDbSettings) : base(mongoDbSettings)
        {
            if (!mongoDbSettings.UseRemoteHost)
            {
                _userDatabase = new UserDatabase(mongoDbSettings.DatabaseConnectionString);
            }
            else
            {
                _userDatabase =
                    new UserDatabase(string.Empty, false)
                    {
                        MongoClientFunc = GetRemoteConnection
                    };
                _userDatabase.ConnectToDatabase();
            }
        }

        #endregion
    }
}
