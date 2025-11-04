namespace OrderConsoleApp.Utilities;

public class ConsoleLogger : ILogger
{
    
    public void LogInfo(string message)
    {
        Console.WriteLine($"{DateTime.UtcNow} INFO: {message}");
    }

    public void LogError(string message, Exception ex)
    {
        Console.Error.WriteLine($"""
                                 {DateTime.UtcNow} ERROR : {message}
                                    Exception Type: {ex.GetType()}
                                    Exception Message: {ex.Message}
                                    Stacktrace: {ex.StackTrace}
                                 """);
    }
}