using Microsoft.Extensions.Logging;

namespace commitizen.Cli.Services;

public class ServiceImplementation(ILogger<ServiceImplementation> logger) : IService
{
    public void DoSomething()
    {
        logger.LogCritical("I am doing something");
    }
}