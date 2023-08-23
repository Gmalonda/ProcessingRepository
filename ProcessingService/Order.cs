using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProcessingService
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? OrderId { get; set; }
        public string? OrderStatus { get; set; }
        public string? OrderDate { get; set; }
        public Product? OrderProduct { get; set; }
        public Client? OrderClient { get; set; }
        public string? Quantity { get; set; }
        public string? Price { get; set; }
        public string? TotalPrice { get; set; }
        public string? IsProcesing { get; set; }
    }
}
