using DxWorks.ScriptBee.Analysis.Sdk.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace DxWorks.ScriptBee.Analysis.Sdk.Tests.Extensions;

public class TelemetrySdkExtensionsTests
{
    [Fact]
    public void AddAnalysisSdkTelemetry_WhenDisabled_ShouldNotRegisterTracerOrMeterProvider()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { ["ScriptBee:Telemetry:Enabled"] = "false" }
            )
            .Build();
        var services = new ServiceCollection();

        // Act
        services.AddAnalysisSdkTelemetry(configuration, "custom.analysis.service");
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<TracerProvider>().ShouldBeNull();
        serviceProvider.GetService<MeterProvider>().ShouldBeNull();
    }

    [Fact]
    public void AddAnalysisSdkTelemetry_WhenEnabledWithDefaults_ShouldRegisterTracerAndMeterProvider()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();
        var services = new ServiceCollection();

        // Act
        services.AddAnalysisSdkTelemetry(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<TracerProvider>().ShouldNotBeNull();
        serviceProvider.GetService<MeterProvider>().ShouldNotBeNull();
    }

    [Fact]
    public void AddAnalysisSdkTelemetry_WhenOtlpEndpointConfigured_ShouldRegisterSuccessfully()
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
        services.AddAnalysisSdkTelemetry(configuration, "custom.analysis.service");
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<TracerProvider>().ShouldNotBeNull();
        serviceProvider.GetService<MeterProvider>().ShouldNotBeNull();
    }
}
