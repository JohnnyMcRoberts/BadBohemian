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

    public class BaseMongoEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
