using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoDbBooks.Models
{
    public class BaseMongoEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
