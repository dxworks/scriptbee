using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using PactNet.Infrastructure.Outputters;
using PactNet.Verifier;
using ScriptBee.Marketplace.Client;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using ScriptBeeWebApp.Extensions;
using ScriptBeeWebApp.Tests.Contract.Middleware;
using ScriptBeeWebApp.Tests.Contract.ProviderStates;
using ScriptBeeWebApp.Tests.Contract.XUnitHelpers;
using Xunit;
using Xunit.Abstractions;

namespace ScriptBeeWebApp.Tests.Contract;

public sealed class ProviderApiContractTests : IDisposable
{
    private readonly string _pactServiceUri;
    private readonly WebApplication _webApplication;
    private readonly ITestOutputHelper _outputHelper;

    public ProviderApiContractTests(ITestOutputHelper output)
    {
        _outputHelper = output;
        _pactServiceUri = "http://localhost:5214";

        var builder = WebApplication.CreateBuilder();
        var userFolderConfigurationSection = builder.Configuration.GetSection("UserFolder");

        // TODO: move service registration in Endpoint Definitions, must be synchronized with Program.cs  
        builder.Services
            .Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; })
            .AddSerilog()
            .AddUtilityServices()
            .AddPluginServices()
            .AddScriptBeeMarketplaceClient()
            .AddFileWatcherServices()
            .AddControllerServices(userFolderConfigurationSection)
            .AddValidatorsFromAssemblyContaining<IValidationMarker>()
            .AddCors(options => options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .AllowAnyOrigin();
                }
            ))
            .AddSignalR();
        builder.Services.AddEndpointDefinitions();

        builder.Services.RegisterProviderStateLocatorMocks();

        builder.WebHost.UseUrls(_pactServiceUri);

        _webApplication = builder.Build();

        _webApplication.UseMiddleware<ProviderStateMiddleware>();
        _webApplication.UseRouting();
        _webApplication.UseExceptionHandler("/error");
        _webApplication.UseEndpoints(_ => { });

        _webApplication.UseEndpointDefinitions();
    }

    [Fact]
    public async Task EnsureProviderApiHonoursPactWithConsumer()
    {
        var config = new PactVerifierConfig
        {
            Outputters = new List<IOutput>
            {
                new XUnitOutput(_outputHelper)
            },
        };

        var cancellationTokenSource = new CancellationTokenSource();

        await _webApplication.StartAsync(cancellationTokenSource.Token);

        var pactUri = new Uri(_pactServiceUri);

        try
        {
            new PactVerifier(config)
                .ServiceProvider("ScriptBee API", pactUri)
                .WithFileSource(new FileInfo("ScriptBee UI-ScriptBee API.json"))
                .WithProviderStateUrl(new Uri(pactUri, "/provider-states"))
                .Verify();
        }
        catch
        {
            cancellationTokenSource.Cancel();
            throw;
        }
    }

    #region IDisposable Support

    private bool _disposedValue;

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _webApplication.StopAsync().GetAwaiter().GetResult();
            _webApplication.DisposeAsync().GetAwaiter().GetResult();
        }

        _disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(true);
    }

    #endregion
}
