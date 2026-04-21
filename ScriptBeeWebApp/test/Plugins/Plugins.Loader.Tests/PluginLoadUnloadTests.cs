using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;
using ScriptBee.Tests.Common;

namespace ScriptBee.Plugins.Loader.Tests;

public class PluginLoadUnloadTests : IClassFixture<TempDirFixture>
{
    private readonly TempDirFixture _tempDirFixture;
    private readonly ILogger<PluginLoader> _logger = Substitute.For<ILogger<PluginLoader>>();
    private readonly PluginRepository _pluginRepository = new();

    private readonly IPluginRegistrationService _pluginRegistrationService =
        Substitute.For<IPluginRegistrationService>();

    private readonly IDllLoader _dllLoader = new DllLoader();
    private readonly PluginLoader _pluginLoader;

    public PluginLoadUnloadTests(TempDirFixture tempDirFixture)
    {
        _tempDirFixture = tempDirFixture;
        _pluginLoader = new PluginLoader(
            _logger,
            _dllLoader,
            _pluginRepository,
            _pluginRegistrationService
        );
    }

    [Fact]
    public void GivenValidPlugin_WhenLoadAndUnload_ThenAssemblyIsUnloaded()
    {
        // Arrange
        const string pluginName = "TestPlugin";
        const string pluginVersion = "1.0.0";
        var pluginFolder = _tempDirFixture.CreateSubFolder($"{pluginName}@{pluginVersion}");

        var originalDllPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "DxWorks.ScriptBee.Plugin.Api.dll"
        );
        var pluginDllPath = Path.Combine(pluginFolder, "plugin.dll");
        File.Copy(originalDllPath, pluginDllPath);

        var plugin = new Plugin(
            pluginFolder,
            new PluginId(pluginName, Version.Parse(pluginVersion)),
            new PluginManifest
            {
                Name = pluginName,
                ExtensionPoints =
                [
                    new ScriptGeneratorPluginExtensionPoint
                    {
                        Kind = "ScriptGenerator",
                        EntryPoint = "plugin.dll",
                    },
                ],
            }
        );

        _pluginRegistrationService
            .TryGetValue("ScriptGenerator", out Arg.Any<HashSet<Type>?>())
            .Returns(x =>
            {
                x[1] = new HashSet<Type> { typeof(object) };
                return true;
            });

        // Act
        var weakRef = ExecuteLoadAndUnloadInIsolatedScope(plugin);

        TryCollectLoadContextMemory(weakRef);

        // Assert
        Assert.False(weakRef.IsAlive, "AssemblyLoadContext should have been collected");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private WeakReference ExecuteLoadAndUnloadInIsolatedScope(Plugin plugin)
    {
        _pluginLoader.Load(plugin);

        var loadedPlugins = _pluginRepository.GetLoadedPlugins().ToList();
        Assert.Single(loadedPlugins);
        Assert.Equal(plugin.Id.Name, loadedPlugins[0].Id.Name);

        var alc = AssemblyLoadContext.All.FirstOrDefault(x =>
            x.Name == null && x.GetType() == typeof(PluginAssemblyLoadContext)
        );
        var weakRef = new WeakReference(alc);

        _pluginLoader.Unload(plugin.Id);

        return weakRef;
    }

    private static void TryCollectLoadContextMemory(WeakReference weakRef)
    {
        for (var i = 0; i < 10 && weakRef.IsAlive; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
