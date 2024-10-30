namespace OrderFiltering;

public class Program
{
    public static Task Main(string[] args) => 
        WebApplication
            .CreateBuilder(args)
            .SetupBuilder()
            .Build()
            .SetupApplication()
            .InitializeMigrations()
            .RunAsync();
}