using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMTest.DBContext;
using EMTest.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryOrdersController : ControllerBase
    {
        private readonly DeliveryContext _context;
        private readonly ILogger<DeliveryOrdersController> _logger;

        public DeliveryOrdersController(DeliveryContext context, ILogger<DeliveryOrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredOrders(string cityDistrict, DateTime firstDeliveryDateTime)
        {
            if (string.IsNullOrWhiteSpace(cityDistrict))
                return BadRequest("Необходимо указать район.");

            var firstDeliveryDateTimeUtc = DateTime.SpecifyKind(firstDeliveryDateTime, DateTimeKind.Utc);
            var halfHourLater = firstDeliveryDateTimeUtc.AddMinutes(30);

            var filteredOrders = await _context.Orders
                .Where(o => o.District == cityDistrict && o.DeliveryTime >= firstDeliveryDateTimeUtc && o.DeliveryTime <= halfHourLater)
                .ToListAsync();

            if (filteredOrders.Count == 0)
            {
                _logger.LogInformation($"Заказы не найдены для района: {cityDistrict} в указанный промежуток времени.");
                return NotFound("Заказы не найдены.");
            }

            _logger.LogInformation($"Отфильтровано {filteredOrders.Count} заказов для района: {cityDistrict} с {firstDeliveryDateTimeUtc} до {halfHourLater}.");
            await LogOperationAsync($"Отфильтрованные заказы для района {cityDistrict} между {firstDeliveryDateTimeUtc} и {halfHourLater}.");

            return Ok(filteredOrders);
        }

        private async Task LogOperationAsync(string message)
        {
            var log = new LogEntry { Message = message, Timestamp = DateTime.UtcNow};
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
