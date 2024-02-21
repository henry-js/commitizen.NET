using commitizen.NET.Lib;
using Microsoft.Extensions.Configuration;

namespace commitizen.NET.Tests;

public static class TestConfigHelper
{
    private static IConfigurationRoot? configurationRoot;

    public static IConfigurationRoot GetIConfigurationRoot()
    {
        configurationRoot = new ConfigurationBuilder()
            .AddJsonFile("settings.test.json", optional: false, reloadOnChange: true)
            .Build();

        return configurationRoot;
    }
}