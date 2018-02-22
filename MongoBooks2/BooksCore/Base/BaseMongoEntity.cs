// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseMongoEntity.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The base MongoDb entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Base
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// The base MongoDb entity.
    /// </summary>
    public abstract class BaseMongoEntity
    {
        /// <summary>
        /// Gets or sets the unique entity identifier.
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Gets the name to use for equivalence checks.
        /// </summary>
        public abstract string EquivalenceName { get; }
    }
}
