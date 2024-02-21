using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace commitizen.NET.Cli.Common;

public sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly HostApplicationBuilder _builder;

    public TypeRegistrar(HostApplicationBuilder builder)
    {
        _builder = builder;
    }

    public ITypeResolver Build()
    {
        return new TypeResolver(_builder.Build());
    }

    public void Register(Type service, /* [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] */ Type implementation)
    {
        _builder.Services.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        _builder.Services.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> func)
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        _builder.Services.AddSingleton(service, _ => func());
    }
}