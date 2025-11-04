using System.Collections.Concurrent;
using OrderConsoleApp.Models;

namespace OrderConsoleApp.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<int, Order> _database;
    
    public OrderRepository(ConcurrentDictionary<int, Order> database)
    {
        _database = database;
    }
    
    public async Task<string> GetOrderAsync(int orderId)
    {
        if (orderId < 1)
            throw new ArgumentException("Order's Id cannot be less than 1");
        await Task.Delay(100);
        _database.TryGetValue(orderId, out var order);
        if (order is null)
            throw new KeyNotFoundException($"There is no Order with Id {orderId}");
        return order.Description;
    }
}