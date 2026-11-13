using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using ScriptBee.Common.Web.Extensions;

namespace ScriptBee.Common.Tests.Extensions;

public class TelemetryExtensionsTests
{
    [Fact]
    public void AddScriptBeeTelemetry_WhenDisabled_ShouldNotRegisterTracerOrMeterProvider()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { ["ScriptBee:Telemetry:Enabled"] = "false" }
            )
            .Build();
        var services = new ServiceCollection();

        // Act
        services.AddScriptBeeTelemetry(configuration, "test.service");
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<TracerProvider>().ShouldBeNull();
        serviceProvider.GetService<MeterProvider>().ShouldBeNull();
    }

    [Fact]
    public void AddScriptBeeTelemetry_WhenEnabledWithDefaults_ShouldRegisterTracerAndMeterProvider()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();
        var services = new ServiceCollection();

        // Act
        services.AddScriptBeeTelemetry(configuration, "test.service");
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<TracerProvider>().ShouldNotBeNull();
        serviceProvider.GetService<MeterProvider>().ShouldNotBeNull();
    }

    [Fact]
    public void AddScriptBeeTelemetry_WhenConsoleExporterEnabled_ShouldRegisterSuccessfully()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ScriptBee:Telemetry:Enabled"] = "true",
                    ["ScriptBee:Telemetry:ConsoleExporterEnabled"] = "true",
                }
            )
            .Build();
        var services = new ServiceCollection();

        // Act
        services.AddScriptBeeTelemetry(configuration, "test.service");
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<TracerProvider>().ShouldNotBeNull();
        serviceProvider.GetService<MeterProvider>().ShouldNotBeNull();
    }

    [Fact]
    public void AddScriptBeeTelemetry_WhenOtlpEndpointConfigured_ShouldRegisterSuccessfully()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ScriptBee:Telemetry:Enabled"] = "true",
                    ["ScriptBee:Telemetry:OtlpEndpoint"] = "http://localhost:4318",
                }
            )
            .Build();
        var services = new ServiceCollection();

        // Act
        services.AddScriptBeeTelemetry(configuration, "test.service");
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<TracerProvider>().ShouldNotBeNull();
        serviceProvider.GetService<MeterProvider>().ShouldNotBeNull();
    }

    [Fact]
    public void TelemetryOptions_ShouldHaveExpectedDefaults()
    {
        // Arrange & Act
        var options = new TelemetryOptions();

        // Assert
        options.Enabled.ShouldBeTrue();
        options.OtlpEndpoint.ShouldBeNull();
        options.ConsoleExporterEnabled.ShouldBeFalse();
    }
}
