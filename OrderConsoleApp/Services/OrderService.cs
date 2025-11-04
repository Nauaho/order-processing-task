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
 
    public async Task ProcessOrderAsync(int orderId)
    {
        _logger.LogInfo($"Start of processing order {orderId}");
        try
        {
            await _repository.GetOrderAsync(orderId);
            _logger.LogInfo($"Order {orderId} has been successfully processed");
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"No such order with index of {orderId}", e);
        }
        catch (ArgumentException e)
        {
            _logger.LogError($"OrderId mustn't be less than 1. {orderId} < 1", e);
        }
    }

}