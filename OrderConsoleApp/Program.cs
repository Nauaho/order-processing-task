using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OrderConsoleApp.Models;
using OrderConsoleApp.Repositories;
using OrderConsoleApp.Services;
using OrderConsoleApp.Utilities;

class Program
{
    static void Main(string[] args)
    {
        var database = new ConcurrentDictionary<int, Order>();
        Order[] orders = { new Order() { Id = 1, Description = "Laptop" }, new Order() { Id = 2, Description = "Phone" } };
        var services = new ServiceCollection();
        services.AddSingleton<ILogger, ConsoleLogger>();
        services.AddSingleton(database);
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