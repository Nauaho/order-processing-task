namespace OrderConsoleApp.Utilities;

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