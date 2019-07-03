# Hosted Warm Web Service
This service provides a way to keep endpoints warmed up and active by "getting" the endpoints at regular intervals.

## HostedWarmWebOptions
The options for the hosted warm web service has four properties.

* BaseUri - which will be used as the base for the endpoints, if empty the endpoints should be complete - default empty
* WarmupEndpoints - The enumerable endpoints to add to the base - default empty
* InitialDelay - The first time delay before calling the endpoitns. default 1 second
* Interval - The interval between calls to the endpoints. default 1 second

The example below shows how the options would look in your appsettings.json
```json
{
    //...

    "HostedWarmWeb": {
        "BaseUri": "http://localhost:5000/",
        "WarmupEndpoints": [
            "api/endpoint1/check",
            "api/endpoint2/check"
        ],
        "InitialDelay": "00:30:00",
        "Interval": "00:02:00"
    }

    //...
}
```

## Adding to your application
HostedWarmWebService, and the options should be added to your application in the startup. Logging should also be added, which is the default when using CreateDefaultBuilder.

There is an extension provided which expects the section "HostedWarmWeb" to be set in your configuration, and adds everything needed.
```c#
public class Startup
{
    //...

    public void ConfigureServices(IServiceCollection services)
    {
        //...
        services.AddHostedWarmWebService();
        //...
    }

    //...
}
```

Or if you wish to do this manually an example is give below:
```c#
public class Startup
{
    //...

    public void ConfigureServices(IServiceCollection services)
    {
        //...
        var warmWebOptions = Configuration.GetSection("HostedWarmWeb").Get<HostedWarmWebOptions>();
        services,AddSingleton(warmWebOptions);
        services.AddHostedService<HostedWarmWebService>();
        //...
    }

    //...
}
```

There is an alternative to providing the baseUri or complete endpoints which is to provide a Func<HttpClient> as part of the startup services with the baseaddress already set. This way you can add headers, cookies etc to all the requests made.

```c#
    services.AddSingletion<Func<HttpClient>>(sp => () => new HttpClient { BaseAddress = new Uri("http://someserver:port/") });

```
