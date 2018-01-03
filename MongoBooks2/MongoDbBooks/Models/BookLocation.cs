namespace MongoDbBooks.Models
{
    public class BookLocation
    {
        public BookRead Book { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
