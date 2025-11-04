using Moq;
using OrderConsoleApp.Repositories;
using OrderConsoleApp.Services;
using OrderConsoleApp.Utilities;

namespace OrderPieceTests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepository;
    private readonly Mock<ILogger> _logger;
    
    public OrderServiceTests()
    {
        _orderRepository = new Mock<IOrderRepository>();
        _logger = new Mock<ILogger>();
    }
    [Fact]
    public async Task ProcessOrderSync_ShouldLogSuccessMessage_WhenOrderIsFound()
    {
        //Arrange
        var logs = new List<string>();
        const int orderId = 1;
        var msg1 = $"Start of processing order {orderId}";
        var msg2 = $"Order {orderId} has been successfully processed";
        _orderRepository
            .Setup(r => r.GetOrderAsync(It.Is<int>(n => n >= 1)))
            .ReturnsAsync( It.IsAny<string>());
        var repo = _orderRepository.Object;
        
        _logger
            .Setup(l => l.LogInfo(It.IsAny<string>()))
            .Callback<string>(msg => logs.Add(msg));
        var logger = _logger.Object;
        
        var orderService = new OrderService(repo, logger);
        
        //Act
        await orderService.ProcessOrderAsync(orderId);
        
        //Assert
        Assert.Contains(msg1, logs);
        Assert.Contains(msg2, logs);
        Assert.True(logs.IndexOf(msg1) < logs.IndexOf(msg2));
    }
    
    [Fact]
    public async Task ProcessOrderSync_ShouldLogNotFoundErrorMessage_WhenOrderIsNotFound()
    {
        //Arrange
        var logs = new List<string>();
        var exceptions = new List<Exception>();
        const int orderId = 1;
        var msg1 = $"Start of processing order {orderId}";
        var msg2 = $"No such order with index of {orderId}";
        _orderRepository
            .Setup(r => r.GetOrderAsync(It.Is<int>(n => n >= 1)))
            .ThrowsAsync(new KeyNotFoundException());
        var repo = _orderRepository.Object;
        
        _logger
            .Setup(l => l.LogInfo(It.IsAny<string>()))
            .Callback<string>(msg => logs.Add(msg));
        _logger
            .Setup(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()))
            .Callback<string, Exception>((msg, ex) =>
            {
                logs.Add(msg);
                exceptions.Add(ex);
            });
        var logger = _logger.Object;
        
        var orderService = new OrderService(repo, logger);
        
        //Act
        await orderService.ProcessOrderAsync(orderId);
        
        //Assert
        Assert.Contains(msg1, logs);
        Assert.Contains(msg2, logs);
        Assert.True(logs.IndexOf(msg1) < logs.IndexOf(msg2));
        var ex = Assert.Single(exceptions);
        Assert.IsType<KeyNotFoundException>(ex);
    }
    
    [Fact]
    public async Task ProcessOrderSync_ShouldLogIllegalArgumentErrorMessage_WhenInputIsIllegal()
    {
        //Arrange
        var logs = new List<string>();
        var exceptions = new List<Exception>();
        const int orderId = -1;
        var msg1 = $"Start of processing order {orderId}";
        var msg2 = $"OrderId mustn't be less than 1. {orderId} < 1";
        _orderRepository
            .Setup(r => r.GetOrderAsync(It.Is<int>(n => n < 1)))
            .ThrowsAsync(new ArgumentException());
        var repo = _orderRepository.Object;
        
        _logger
            .Setup(l => l.LogInfo(It.IsAny<string>()))
            .Callback<string>(msg => logs.Add(msg));
        _logger
            .Setup(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()))
            .Callback<string, Exception>((msg, ex) =>
            {
                logs.Add(msg);
                exceptions.Add(ex);
            });
        var logger = _logger.Object;
        
        var orderService = new OrderService(repo, logger);
        
        //Act
        await orderService.ProcessOrderAsync(orderId);
        
        //Assert
        Assert.Contains(msg1, logs);
        Assert.Contains(msg2, logs);
        Assert.True(logs.IndexOf(msg1) < logs.IndexOf(msg2));
        var ex = Assert.Single(exceptions);
        Assert.IsType<ArgumentException>(ex);
    }
}