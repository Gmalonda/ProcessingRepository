using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProcessingService
{
    public class OrderDto
    {
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public bool? IsProcesing { get; set; }
        public string? OrderStatus { get; set; }
        public string? ProductId { get; set; }
        public int? Price { get; set; }
    }
}
