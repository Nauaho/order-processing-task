namespace OrderConsoleApp.Repositories;

public interface IOrderRepository
{
    string GetOrder(int orderId);
}