namespace OrderConsoleApp.Repositories;

public interface IOrderRepository
{
    Task<string> GetOrderAsync(int orderId);
}