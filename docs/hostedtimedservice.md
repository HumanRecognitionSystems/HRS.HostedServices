# Hosted Timed Service
The service provides an abstract base class that runs an async executing an a regular interval. Also the service pauses the timer while the task is executing. So the interval is the time between the executions.

## HostedTimedService
The service itself is an abstract class.

When inheriting the service there is only the one function to implement *ProcessItem*.

An example is shown below
```c#
public class ExampleHostedTimedService : HostedTimedService
{
    public ExampleHostedTimedService(HostedTimedOptions options, ILogger<ExampleHostedTimedService> logger) 
        : base(options, logger)
    {}

    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        Console.WriteLine($"Task was executed");
        return Task.CompletedTask;
    }
}
```

## HostedTimedOptions
The options for the hosted timed service has two properties.

* InitialDelay - The first time delay from the start of the service. default 1 second
* Interval - The interval between executions of the main function. default 1 second

The example below shows how the default options would look in the appsettings.json
```json
{
    //...

    "ExampleHostedTimed": {
        "InitialDelay": "00:00:01",
        "Interval": "00:00:01"
    }

    //...
}
```

## Adding to your application
Your implemtation of HostedTimedService, and the options should be added to your application in the startup. Logging should also be added, which is the default when using CreateDefaultBuilder.

```c#
public class Startup
{
    //...

    public void ConfigureServices(IServiceCollection services)
    {
        //...
        var timedOptions = Configuration.GetSection("ExampleHostedTimed").Get<HostedTimedOptions>();
        services,AddSingleton(timedOptions);
        services.AddHostedService<ExampleHostedTimedService>();
        //...
    }

    //...
}