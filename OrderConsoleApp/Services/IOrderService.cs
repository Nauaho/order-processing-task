namespace OrderConsoleApp.Services;

public interface IOrderService
{
    Task ProcessOrderAsync(int orderId);
}