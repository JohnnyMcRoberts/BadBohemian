// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseEntity.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The base MongoDb entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace MongoDbBooks.Models.Database
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// The base MongoDb entity.
    /// </summary>
    public class BaseEntity
    {
        [BsonId]
        /// <summary>
        /// Gets or sets the unique entity identifier.
        /// </summary>
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        /// <summary>
        /// Gets or sets the entity name.
        /// </summary>
        public string Name { get; set; }
    }
}