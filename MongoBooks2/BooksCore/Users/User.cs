// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The user MongoDb entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Users
{
    using System;

    using MongoDB.Bson.Serialization.Attributes;

    using BooksCore.Base;
    using BooksCore.Security;

    /// <summary>
    /// The MongoDb entity for a user.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class User : BaseEntity
    {
        #region Constants

        private const string HashAlgorithm = "SHA512";

        #endregion

        /// <summary>
        /// Gets or sets the hash of the user password.
        /// </summary>
        [BsonElement("password_hash")]
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [BsonElement("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the description of the user.
        /// </summary>
        [BsonElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date added.
        /// </summary>
        [BsonElement("date_added")]
        public DateTime DateAdded { get; set; }

        /// <summary>
        /// Gets or sets the URI string for a .jpg/.png image for the user.
        /// </summary>
        [BsonElement("image_uri")]
        public string ImageUri { get; set; }

        /// <summary>
        /// Gets the image URI ready to be displayed.
        /// </summary>
        public Uri DisplayImage => 
            string.IsNullOrEmpty(ImageUri) ? new Uri("pack://application:,,,/Images/camera_image_cancel-32.png") : new Uri(ImageUri);

        #region Public methods

        public bool VerifyPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return SimpleHash.VerifyHash(password, HashAlgorithm, PasswordHash);
        }

        public bool UpdatePassword(string password)
        {
            // Check an actual password
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Check a changed password
            byte[] salt = null;
            var passwordHash = SimpleHash.ComputeHash(password, HashAlgorithm, ref salt);
            if (PasswordHash == passwordHash)
                return false;

            // Update & say OK
            PasswordHash = passwordHash;
            return true;
        }

        #endregion

        #region Constructors

        public User(string name, string password, string email, string description)
        {
            Name = name;
            Description = description;
            Email = email;
            DateAdded = DateTime.Now;
            ImageUri = string.Empty;

            byte[] salt = null;
            PasswordHash = SimpleHash.ComputeHash(password, HashAlgorithm, ref salt);
        }

        public User()
        {
            Name = string.Empty;
            Description = string.Empty;
            Email = string.Empty;
            PasswordHash = string.Empty;
            DateAdded = DateTime.Now;
            ImageUri = string.Empty;
        }

        #endregion
    }
}
