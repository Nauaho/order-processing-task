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
        var orders =
        new List<Order>(){
            new() { Id = 1, Description = "Laptop" },
            new() { Id = 2, Description = "Phone" }
        };
        orders.ForEach(o => database.TryAdd(o.Id, o));

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
            tasks[0] = Task.Run( async () =>
            {
                await orderService.ProcessOrderAsync(1);
            });
            tasks[1] = Task.Run(async () =>
            {
                await orderService.ProcessOrderAsync(2);
            });
            tasks[2] = Task.Run(async () =>
            {
                await orderService.ProcessOrderAsync(-1);
            });
            Task.WaitAll(tasks);

            Console.WriteLine("Processing complete.");
        }
    }
}