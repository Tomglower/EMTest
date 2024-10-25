using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMTest.DBContext;
using EMTest.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MockDataController : ControllerBase
    {
        private readonly DeliveryContext _context;
        private readonly ILogger<MockDataController> _logger;

        public MockDataController(DeliveryContext context, ILogger<MockDataController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("add-mock-orders")]
        public async Task<IActionResult> AddMockOrders()
        {
            var mockOrders = new List<DeliveryOrder>
            {
                new DeliveryOrder { OrderNumber = "ORD001", Weight = 1.5, District = "Центральный", DeliveryTime = DateTime.UtcNow.AddMinutes(-10) },
                new DeliveryOrder { OrderNumber = "ORD002", Weight = 2.2, District = "Центральный", DeliveryTime = DateTime.UtcNow.AddMinutes(10) },
                new DeliveryOrder { OrderNumber = "ORD003", Weight = 0.8, District = "Северный", DeliveryTime = DateTime.UtcNow.AddMinutes(5) },
                new DeliveryOrder { OrderNumber = "ORD004", Weight = 3.1, District = "Центральный", DeliveryTime = DateTime.UtcNow.AddMinutes(20) },
                new DeliveryOrder { OrderNumber = "ORD005", Weight = 1.9, District = "Южный", DeliveryTime = DateTime.UtcNow.AddMinutes(-15) }
            };

            await _context.Orders.AddRangeAsync(mockOrders);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Мок заказы добавлены в базу данных.");
            return Ok("Мок заказы добавлены в базу данных.");
        }

        [HttpPost("add-order")]
        public async Task<IActionResult> AddOrders([FromBody] DeliveryOrder order)
        {
            if (order == null)
            {
                _logger.LogWarning("Получен null объект для добавления заказа.");
                return BadRequest("Заказ не может быть пустым.");
            }

            if (string.IsNullOrWhiteSpace(order.OrderNumber) || string.IsNullOrWhiteSpace(order.District) || order.Weight <= 0)
            {
                _logger.LogWarning("Некорректные данные заказа: {Order}", order);
                return BadRequest("Некорректные данные заказа.");
            }

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Заказ добавлен в базу данных: {Order}", order);
            return Ok("Заказ успешно добавлен.");
        }
    }
}