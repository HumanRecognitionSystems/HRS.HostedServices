# Hosted Queue Service
The service provides an abstract base class for one at a time, FIFO, processing queue as a hosted service.


## HostedQueueService
The service itself is an abstract class.

When inheriting the service there is only the one function to implement *ProcessItem*.

An example is shown below
```c#
public class ExampleHostedQueueService : HostedQueueService<string>
{
    public ExampleHostedQueueService(IServiceQueue<string> queue, ILogger<ExampleHostedQueueService> logger, HostedQueueOptions options) 
        : base(queue, logger, options)
    {}

    protected override Task ProcessItem(string item) {
        Console.WriteLine($"Queued Item:{item}");
        return Task.CompletedTask;
    }
}
```

when processing an item, there should be an expectation that a default item maybe returned. This maybe through a problem with the queue.

## IServiceQueue and ServiceQueue
The IServiceQueue is the inteface for the queue that will be used by the service. The ServiceQueue is a standard implmentation of the interface. 

Any implemtation of the Dequeue function should not return until an item is available from the queue. if a problem occurs while getting an item from the queue it should return a default item, that then can be handled by the implemtation of ProcessItem in the implentation of the HostedQueueService.

## HostedQueueOptions
The options for the hosted queue service has three properties.

* RequeueFailedItems - if a ProcessItem call throws an exception should the item be requeued or throw away, the default is false (thrown away)
* RequeueDelay - If RequeueFailedItems is enabled what if any delay should be applied before readding the item. default 100ms
* NextItemDelay - Should there be a delay between processing the options. default is 0 no delay.

The example below shows how the default options would look in the appsettings.json
```json
{
    //...

    "ExampleHostedQueue": {
        "RequeueFailedItems": false,
        "RequeueDelay": "00:00:00.1",
        "NextItemDelay": "00:00:00"
    }

    //...
}
```

## Adding to your application
Your implemtation of HostedQueueService, the implmentation of IHostedQueue, and the options should be added to your application in the startup. Logging should also be added, which is the default when using CreateDefaultBuilder.

```c#
public class Startup
{
    //...

    public void ConfigureServices(IServiceCollection services)
    {
        //...
        var queueOptions = Configuration.GetSection("ExampleHostedQueue").Get<HostedQueueOptions>();
        services,AddSingleton(queueOptions);
        services.AddSingleton<IServiceQueue<string>, ServiceQueue<string>>();
        services.AddHostedService<ExampleHostedQueueService>();
        //...
    }

    //...
}
```