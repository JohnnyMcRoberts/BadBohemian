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
