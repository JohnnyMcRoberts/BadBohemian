// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Nation.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The user MongoDb entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace MongoDbBooks.Models.Database
{
    using System;

    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// The MongoDb entity for a user.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class User : BaseEntity
    {
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
        /// Gets or sets the date added.
        /// </summary>
        [BsonElement("date_added")]
        public DateTime DateAdded { get; set; }

        /// <summary>
        /// Gets or sets the URI string for a .jpg/.png image for the user.
        /// </summary>
        [BsonElement("image_uri")]
        public string ImageUri { get; set; }
    }
}
