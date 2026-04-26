using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Tests.Common;

namespace ScriptBee.Plugins.Installer.Tests;

public class PluginUninstallerTests : IClassFixture<TempDirFixture>
{
    private readonly ILogger<PluginUninstaller> _logger = Substitute.For<
        ILogger<PluginUninstaller>
    >();

    private readonly PluginUninstaller _pluginUninstaller;
    private readonly TempDirFixture _fixture;

    public PluginUninstallerTests(TempDirFixture fixture)
    {
        _fixture = fixture;
        _pluginUninstaller = new PluginUninstaller(_logger);
    }

    [Fact]
    public void GivenPath_WhenUninstall_TheDirectoryIsDeleted()
    {
        var pluginPath = _fixture.CreateSubFolder("plugin_to_uninstall");

        _pluginUninstaller.Uninstall(pluginPath);

        Directory.Exists(pluginPath).ShouldBeFalse();
    }
}
