using Microsoft.Extensions.Configuration;

namespace commitizen.NET.Tests;

public static class TestConfigHelper
{
    public static IConfigurationRoot GetIConfigurationRoot()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("settings.json", optional: false, reloadOnChange: true)
            .Build();
    }
}