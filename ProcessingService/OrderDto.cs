using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProcessingService
{
    public class OrderDto
    {
        public string? IsProcesing { get; set; }
        public string? OrderStatus { get; set; }
    }
}
