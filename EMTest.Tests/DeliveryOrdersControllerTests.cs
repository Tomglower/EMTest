using EMTest.Controllers;
using EMTest.DBContext;
using EMTest.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace EMTest.Tests;

[TestFixture]
public class DeliveryOrdersControllerTests
{
    private DeliveryContext _context;
    private Mock<ILogger<DeliveryOrdersController>> _loggerMock;
    private DeliveryOrdersController _controller;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DeliveryContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new DeliveryContext(options);

        _loggerMock = new Mock<ILogger<DeliveryOrdersController>>();
        _controller = new DeliveryOrdersController(_context, _loggerMock.Object);
    }
    
    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    
    [Test]
    public async Task GetFilteredOrders_ReturnsOkResult_WithCorrectOrders()
    {
        var district = "Центральный";
        var deliveryTime = DateTime.UtcNow;
        _context.Orders.AddRange(new List<DeliveryOrder>
        {
            new DeliveryOrder { OrderNumber = "ORD001", Weight = 1.5, District = "Центральный", DeliveryTime = deliveryTime.AddMinutes(-10) },
            new DeliveryOrder { OrderNumber = "ORD002", Weight = 2.2, District = "Центральный", DeliveryTime = deliveryTime.AddMinutes(10) },
            new DeliveryOrder { OrderNumber = "ORD003", Weight = 0.8, District = "Северный", DeliveryTime = deliveryTime.AddMinutes(5) },
            new DeliveryOrder { OrderNumber = "ORD004", Weight = 3.1, District = "Центральный", DeliveryTime = deliveryTime.AddMinutes(20) }
        });
        await _context.SaveChangesAsync();

        var result = await _controller.GetFilteredOrders(district, deliveryTime);

        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        var orders = okResult?.Value as List<DeliveryOrder>;
        Assert.AreEqual(2, orders?.Count);
    }
    [Test]
    public async Task GetFilteredOrders_ReturnsNotFound_WhenNoOrdersMatch()
    {
        var district = "Южный";
        var deliveryTime = DateTime.UtcNow;

        var result = await _controller.GetFilteredOrders(district, deliveryTime);

        Assert.IsInstanceOf<NotFoundObjectResult>(result);
        _loggerMock.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Заказы не найдены для района: {district}")),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
    }
}