using OrderConsoleApp.Repositories;
using OrderConsoleApp.Utilities;

namespace OrderConsoleApp.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly ILogger _logger;

    public OrderService(IOrderRepository repository, ILogger logger)
    {
        _repository = repository;
        _logger = logger;
    }
 
    public Task ProcessOrderAsync(int orderId)
    {
        _logger.LogInfo($"Start of processing order {orderId}");
        try
        {
            _repository.GetOrder(orderId);
            _logger.LogInfo($"Order {orderId} has been successfully processed");
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"Error has been encountered: no such order with index of {orderId}", e);
        }
        catch (ArgumentException e)
        {
            _logger.LogError($"Error has been encountered: orderId mustn't be less than 1", e);
        }
        return Task.CompletedTask;
    }

}