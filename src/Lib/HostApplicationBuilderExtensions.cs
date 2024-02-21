// using commitizen.NET.Lib;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Options;

// public static class ConfigurationExtensions
// {
//     // public static IConfigurationBuilder AddDefaultLintingSettingsConfiguration(this ConfigurationManager configuration)
//     // {
//     //     return configuration
//     // }
//     public static IServiceCollection AddDefaultLintingSettings(this IServiceCollection services)
//     {

//         services.AddOptions<LintingSettings>()
//             .Configure<IConfiguration>((options, configuration) =>
//                 configuration.GetSection(nameof(LintingSettings)).Bind(options));
//         // .BindConfiguration(configSectionPath);

//         return services;
//     }
// }