using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

// Interface for order service
public interface IOrderService
{
    void ProcessOrder(int orderId);
}

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly ILogger _logger;

    public OrderService(IOrderRepository repository, ILogger logger)
    {
        _repository = repository;
        _logger = logger;
    }
 
    public void ProcessOrder(int orderId)
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
        
    }

}

// Interface for order repository
public interface IOrderRepository
{
    string GetOrder(int orderId);
}

public class OrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<int, Order> _database;
    
    public OrderRepository(ConcurrentDictionary<int, Order> database)
    {
        _database = database;
        _database.TryAdd(1, new Order(){Id = 1, Description = "Laptop"});
        _database.TryAdd(2, new Order(){Id = 2, Description = "Phone"});
    }
    
    public string GetOrder(int orderId)
    {
        if (orderId < 1)
            throw new ArgumentException("Order's Id cannot be less than 1");
        _database.TryGetValue(orderId, out var order);
        if (order is null)
            throw new KeyNotFoundException($"There is no Order with Id {orderId}");
        return order.Description;
    }
}

// Interface for logger
public interface ILogger
{
    void LogInfo(string message);
    void LogError(string message, Exception ex);
}

public class ConsoleLogger : ILogger
{
    
    public void LogInfo(string message)
    {
        Console.WriteLine($"{DateTime.Now} {message}");
    }

    public void LogError(string message, Exception ex)
    {
        Console.Error.WriteLine($"{DateTime.Now} {message}\n{ex.StackTrace}");
    }
}

// Sample Order class
public class Order
{
    public int Id { get; set; }
    public string Description { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        // TODO: Initialize DI container, services, and repository
        // TODO: Demonstrate multi-threaded order processing
        var services = new ServiceCollection();
        services.AddSingleton<ILogger, ConsoleLogger>();
        services.AddSingleton(new ConcurrentDictionary<int, Order>());
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddTransient<IOrderService, OrderService>();
        
        using var provider = services.BuildServiceProvider();
        using (var scope = provider.CreateScope())
        {
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

            Console.WriteLine("Order Processing System");

            // Example: Simulate multiple threads processing orders
            Task[] tasks = new Task[3];
            tasks[0] = Task.Run(() =>
            {
                orderService.ProcessOrder(1);
            });
            tasks[1] = Task.Run(() =>
            {
                orderService.ProcessOrder(2);
            });
            tasks[2] = Task.Run(() =>
            {
                orderService.ProcessOrder(-1);
            });
            Task.WaitAll(tasks);

            Console.WriteLine("Processing complete.");
        }
    }
}