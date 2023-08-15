using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProcessingService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessingController : ControllerBase
    {
        private readonly IMongoCollection<Order> _collection;

        public ProcessingController()
        {
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("orderdb");
            _collection = database.GetCollection<Order>("order");
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderToProcess()
        {
           
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: "order_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                string? message = null;
                var messages = new List<string>();
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    message = Encoding.UTF8.GetString(body);
                    messages.Add(message);

                };


                channel.BasicConsume(queue: "order_queue", autoAck: true, consumer: consumer);

                await Task.Delay(100);

                var result = "";

                if (messages.Count > 1)
                {
                    foreach (var item in messages)
                    {
                        result += item + " , ";

                    }
                }
                else
                {
                    foreach (var item in messages)
                    {
                        result += item;

                    }
                }

               

                return Ok($"Commande(s) à traiter: Commande n° {result}");




            }
            catch (Exception e)
            {
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
            }
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> ProcessOrder(string orderId, [FromBody] OrderDto orderDto)
        {
            try
            {
                var product = new Product { ProductId = orderDto.ProductId, ProductName = orderDto.ProductName };


                var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderId);
                var update = Builders<Order>.Update
                    .Set(o => o.IsProcesing, orderDto.IsProcesing)
                    .Set(o => o.OrderProduct, product)
                    .Set(o => o.Quantity, orderDto.Quantity)
                    .Set(o => o.TotalPrice, orderDto.Quantity * orderDto.Price)
                    .Set(o => o.Price, orderDto.Price)
                    .Set(o => o.OrderStatus, orderDto.OrderStatus);

                var options = new FindOneAndUpdateOptions<Order>
                {
                    ReturnDocument = ReturnDocument.After
                };

                var updatedDocument = await _collection.FindOneAndUpdateAsync(filter, update, options);

                if (updatedDocument != null)
                {
                    return Ok("Commande traiter avec succès");
                }
                else
                {
                    return NotFound("Numéro de la commande est incorrecte");
                }
            }
            catch (Exception e)
            {
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
            }
        }
    }
}
