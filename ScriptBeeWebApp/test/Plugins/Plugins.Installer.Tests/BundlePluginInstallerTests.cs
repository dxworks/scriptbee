using System.Collections;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Plugins.Marketplace;
using ScriptBee.Plugins.Marketplace.Errors;
using ScriptBee.Tests.Common;
using ScriptBee.Tests.Common.Plugins;
using static ScriptBee.Tests.Common.Plugins.PluginUtils;

namespace ScriptBee.Plugins.Installer.Tests;

public class BundlePluginInstallerTests : IClassFixture<TempDirFixture>
{
    private readonly IPluginReader _pluginReader = Substitute.For<IPluginReader>();

    private readonly ISimplePluginInstaller _simplePluginInstaller =
        Substitute.For<ISimplePluginInstaller>();

    private readonly IPluginUrlFetcher _pluginUrlFetcher = Substitute.For<IPluginUrlFetcher>();

    private readonly IPluginPathProvider _pluginPathProvider =
        Substitute.For<IPluginPathProvider>();

    private readonly IPluginZipProcessor _pluginZipProcessor =
        Substitute.For<IPluginZipProcessor>();

    private readonly ILogger<BundlePluginInstaller> _logger = Substitute.For<
        ILogger<BundlePluginInstaller>
    >();

    private readonly TempDirFixture _tempDirFixture;

    private readonly ProjectId _projectId = ProjectId.FromValue("project-id");

    private readonly BundlePluginInstaller _bundlePluginInstaller;

    public BundlePluginInstallerTests(TempDirFixture tempDirFixture)
    {
        _tempDirFixture = tempDirFixture;
        _bundlePluginInstaller = new BundlePluginInstaller(
            _pluginReader,
            _simplePluginInstaller,
            _pluginUrlFetcher,
            _pluginPathProvider,
            _pluginZipProcessor,
            _logger
        );
    }

    #region Simple Plugin Tests

    [Theory]
    [ClassData(typeof(BundlePluginInstallerSimpleTestData))]
    public async Task GivenSimplePlugin_WhenInstall_ThenPluginIsInstalledSuccessfully(
        PluginList pluginList
    )
    {
        var pluginId = new PluginId("pluginId", new Version("1.0.0"));
        _pluginReader.ReadPlugins("plugin/path").Returns(pluginList.Plugins);
        _pluginUrlFetcher.GetPluginUrl(pluginId, Arg.Any<CancellationToken>()).Returns("url");
        _simplePluginInstaller
            .Install("url", pluginId, Arg.Any<CancellationToken>())
            .Returns("pluginId@1.0.0");

        var result = await _bundlePluginInstaller.Install(
            pluginId,
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        result.AsT0.Single().ShouldBe(pluginId);
    }

    [Theory]
    [ClassData(typeof(BundlePluginInstallerSimpleTestData))]
    public async Task GivenSimplePluginAndUrlFetchFailsWithPluginNotFound_WhenInstall_ThenReturnsError(
        PluginList pluginList
    )
    {
        var pluginId = new PluginId("nonExistentPlugin", new Version("1.0.0"));
        _pluginReader.ReadPlugins("plugin/path").Returns(pluginList.Plugins);
        _pluginUrlFetcher
            .GetPluginUrl(pluginId, Arg.Any<CancellationToken>())
            .Returns(_ => new PluginNotFoundError(pluginId));

        var result = await _bundlePluginInstaller.Install(
            pluginId,
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBeEquivalentTo(new PluginInstallationError(pluginId, []));
    }

    [Theory]
    [ClassData(typeof(BundlePluginInstallerSimpleTestData))]
    public async Task GivenSimplePluginAndUrlFetchFailsWithPluginVersionNotFoundError_WhenInstall_ThenReturnsError(
        PluginList pluginList
    )
    {
        var pluginId = new PluginId("plugin", new Version("1.0.0"));
        _pluginReader.ReadPlugins("plugin/path").Returns(pluginList.Plugins);
        _pluginUrlFetcher
            .GetPluginUrl(pluginId, Arg.Any<CancellationToken>())
            .Returns(_ => new PluginVersionNotFoundError(pluginId));

        var result = await _bundlePluginInstaller.Install(
            pluginId,
            TestContext.Current.CancellationToken
        );

        result.AsT1.Id.ShouldBe(pluginId);
        result.AsT1.NestedPluginsThatCouldNotBeInstalled.ShouldBe([]);
    }

    [Theory]
    [ClassData(typeof(BundlePluginInstallerSimpleTestData))]
    public async Task GivenSimplePluginAndInstallFails_WhenInstall_ThenReturnsError(
        PluginList pluginList
    )
    {
        var pluginId = new PluginId("plugin", new Version("1.0.0"));
        _pluginReader.ReadPlugins("plugin/path").Returns(pluginList.Plugins);
        _pluginUrlFetcher.GetPluginUrl(pluginId, Arg.Any<CancellationToken>()).Returns("url");
        _simplePluginInstaller
            .Install(Arg.Any<string>(), Arg.Any<PluginId>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromException<OneOf<string, PluginInstallationError>>(
                    new Exception("Installation failed")
                )
            );

        var result = await _bundlePluginInstaller.Install(
            pluginId,
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBeEquivalentTo(new PluginInstallationError(pluginId, []));
    }

    public sealed record PluginList(List<Plugin> Plugins);

    private class BundlePluginInstallerSimpleTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [new PluginList([])];
            yield return
            [
                new PluginList([new TestPlugin(new PluginId("pluginName", new Version(1, 0, 0)))]),
            ];
            yield return
            [
                new PluginList([
                    new TestPlugin(new PluginId("pluginName", new Version(1, 0, 0))),
                    new TestPlugin(new PluginId("pluginName", new Version(1, 0, 1))),
                ]),
            ];
            yield return
            [
                new PluginList([
                    new TestPlugin(new PluginId("pluginName2", new Version(1, 0, 0))),
                    new TestPlugin(new PluginId("pluginName3", new Version(1, 0, 0))),
                    new TestPlugin(new PluginId("pluginName3", new Version(1, 0, 5))),
                ]),
            ];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    #endregion

    #region Bundle Plugin Tests

    [Fact]
    public async Task GivenBundlePluginThatHasNoPluginInFolder_WhenInstall_ThenOnlyFolderIsInstalled()
    {
        // Arrange
        var bundleId = new PluginId("bundle", new Version("1.0.0"));

        _pluginUrlFetcher
            .GetPluginUrl(bundleId, TestContext.Current.CancellationToken)
            .Returns("url");
        _simplePluginInstaller
            .Install("url", bundleId, TestContext.Current.CancellationToken)
            .Returns("bundle@1.0.0");

        _pluginReader.ReadPlugin("bundle@1.0.0").Returns((Plugin?)null);

        // Act
        var result = await _bundlePluginInstaller.Install(
            bundleId,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT0.ShouldBeTrue();
        result.AsT0.Single().ShouldBe(bundleId);
    }

    [Fact]
    public async Task GivenBundlePluginWithOnePlugin_WhenInstall_ThenPluginIsInstalled()
    {
        // Arrange
        var bundleId = new PluginId("bundle", new Version("1.0.0"));
        var childId = new PluginId("child", new Version("2.0.0"));

        _pluginUrlFetcher
            .GetPluginUrl(bundleId, TestContext.Current.CancellationToken)
            .Returns("url-bundle");
        _simplePluginInstaller
            .Install("url-bundle", bundleId, TestContext.Current.CancellationToken)
            .Returns("bundle@1.0.0");

        _pluginUrlFetcher
            .GetPluginUrl(childId, TestContext.Current.CancellationToken)
            .Returns("url-child");
        _simplePluginInstaller
            .Install("url-child", childId, TestContext.Current.CancellationToken)
            .Returns("child@2.0.0");

        _pluginReader
            .ReadPlugin("bundle@1.0.0")
            .Returns(CreatePluginWithDependencies(bundleId, childId));
        _pluginReader.ReadPlugin("child@2.0.0").Returns((Plugin?)null);

        // Act
        var result = await _bundlePluginInstaller.Install(
            bundleId,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT0.ShouldBeTrue();
        result.AsT0.Count.ShouldBe(2);
        result.AsT0.ShouldContain(bundleId);
        result.AsT0.ShouldContain(childId);
    }

    [Fact]
    public async Task GivenBundleWithDeepNestedBundles_WhenInstall_ThenAllPluginsAreInstalled()
    {
        // Arrange
        var rootId = new PluginId("root", new Version("1.0.0"));
        var middleId = new PluginId("middle", new Version("1.0.0"));
        var leafId = new PluginId("leaf", new Version("1.0.0"));

        SetupMockInstall(rootId, "root@1.0.0", [middleId]);
        SetupMockInstall(middleId, "middle@1.0.0", [leafId]);
        SetupMockInstall(leafId, "leaf@1.0.0", []);

        // Act
        var result = await _bundlePluginInstaller.Install(
            rootId,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT0.ShouldBeTrue();
        result.AsT0.Count.ShouldBe(3);
        result.AsT0.ShouldContain(rootId);
        result.AsT0.ShouldContain(middleId);
        result.AsT0.ShouldContain(leafId);
    }

    [Fact]
    public async Task GivenBundleWhereChildFails_WhenInstall_ThenReturnsError()
    {
        // Arrange
        var bundleId = new PluginId("bundle", new Version("1.0.0"));
        var failingChildId = new PluginId("broken", new Version("1.0.0"));

        _pluginUrlFetcher
            .GetPluginUrl(bundleId, TestContext.Current.CancellationToken)
            .Returns("url-bundle");
        _simplePluginInstaller
            .Install("url-bundle", bundleId, TestContext.Current.CancellationToken)
            .Returns("bundle@1.0.0");

        _pluginUrlFetcher
            .GetPluginUrl(failingChildId, TestContext.Current.CancellationToken)
            .Returns(new PluginNotFoundError(failingChildId));

        _pluginReader
            .ReadPlugin("bundle@1.0.0")
            .Returns(CreatePluginWithDependencies(bundleId, failingChildId));

        // Act
        var result = await _bundlePluginInstaller.Install(
            bundleId,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT1.ShouldBeTrue();
        result.AsT1.Id.ShouldBe(bundleId);
        result.AsT1.NestedPluginsThatCouldNotBeInstalled.ShouldContain(failingChildId);
    }

    private void SetupMockInstall(PluginId id, string path, List<PluginId> dependencies)
    {
        _pluginUrlFetcher
            .GetPluginUrl(id, TestContext.Current.CancellationToken)
            .Returns($"url-{id.Name}");
        _simplePluginInstaller
            .Install($"url-{id.Name}", id, TestContext.Current.CancellationToken)
            .Returns(path);

        var testBundlePlugins = dependencies
            .Select(d => new TestBundlePlugin(PluginKind.Plugin, d.Name, d.Version.ToString()))
            .ToArray();

        var plugin = CreateBundlePlugin(id.Name, id.Version.ToString(), testBundlePlugins);

        _pluginReader.ReadPlugin(path).Returns(plugin);
    }

    private static Plugin CreatePluginWithDependencies(PluginId id, params PluginId[] childIds)
    {
        var testBundlePlugins = childIds
            .Select(c => new TestBundlePlugin(PluginKind.Plugin, c.Name, c.Version.ToString()))
            .ToArray();

        return CreateBundlePlugin(id.Name, id.Version.ToString(), testBundlePlugins);
    }

    #endregion

    #region Zip Stream Tests

    [Fact]
    public async Task GivenZipStream_WhenInstall_AndManifestNotFound_ThenReturnsManifestNotFoundError()
    {
        using var stream = new MemoryStream([1, 2, 3]);

        _pluginZipProcessor
            .ProcessZipStream(_projectId, stream, Arg.Any<CancellationToken>())
            .Returns(new PluginManifestNotFoundError());

        var result = await _bundlePluginInstaller.Install(
            _projectId,
            stream,
            TestContext.Current.CancellationToken
        );

        result.IsT1.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenZipStream_WhenInstall_AndProcessorFails_ThenReturnsInstallationError()
    {
        var pluginId = new PluginId("failedPlugin", new Version("1.0.0"));
        using var stream = new MemoryStream([1, 2, 3]);

        _pluginZipProcessor
            .ProcessZipStream(_projectId, stream, Arg.Any<CancellationToken>())
            .Returns(new PluginInstallationError(pluginId, []));

        var result = await _bundlePluginInstaller.Install(
            _projectId,
            stream,
            TestContext.Current.CancellationToken
        );

        result.IsT2.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenZipStream_WhenInstall_AndNoDependencies_ThenReturnsInstalledPlugin()
    {
        var pluginId = new PluginId("streamPlugin", new Version("1.0.0"));
        var projectPluginsPath = _tempDirFixture.CreateSubFolder($"project_{Guid.NewGuid()}");
        var pluginPath = Path.Combine(projectPluginsPath, pluginId.GetFullyQualifiedName());
        Directory.CreateDirectory(pluginPath);

        using var stream = new MemoryStream([1, 2, 3]);

        _pluginZipProcessor
            .ProcessZipStream(_projectId, stream, Arg.Any<CancellationToken>())
            .Returns(pluginId);

        _pluginPathProvider.GetPathToPlugins(_projectId).Returns(projectPluginsPath);
        _pluginPathProvider
            .GetPathToPlugins()
            .Returns(_tempDirFixture.CreateSubFolder($"global_{Guid.NewGuid()}"));

        _pluginReader.ReadPlugin(pluginPath).Returns((Plugin?)null);

        var result = await _bundlePluginInstaller.Install(
            _projectId,
            stream,
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        result.AsT0.Single().ShouldBe(pluginId);
    }

    [Fact]
    public async Task GivenZipStream_WhenInstall_AndDependencyFails_ThenLogsErrorAndReturnsInstallationError()
    {
        var bundleId = new PluginId("bundlePlugin", new Version("1.0.0"));
        var depId = new PluginId("failedDep", new Version("2.0.0"));
        var projectPluginsPath = _tempDirFixture.CreateSubFolder($"project_{Guid.NewGuid()}");
        var bundlePath = Path.Combine(projectPluginsPath, bundleId.GetFullyQualifiedName());
        Directory.CreateDirectory(bundlePath);

        using var stream = new MemoryStream([1, 2, 3]);

        _pluginZipProcessor
            .ProcessZipStream(_projectId, stream, Arg.Any<CancellationToken>())
            .Returns(bundleId);

        _pluginPathProvider.GetPathToPlugins(_projectId).Returns(projectPluginsPath);
        _pluginPathProvider
            .GetPathToPlugins()
            .Returns(_tempDirFixture.CreateSubFolder($"global_{Guid.NewGuid()}"));

        _pluginReader.ReadPlugin(bundlePath).Returns(CreatePluginWithDependencies(bundleId, depId));

        _pluginUrlFetcher
            .GetPluginUrl(depId, Arg.Any<CancellationToken>())
            .Returns(new PluginNotFoundError(depId));

        var result = await _bundlePluginInstaller.Install(
            _projectId,
            stream,
            TestContext.Current.CancellationToken
        );

        result.IsT2.ShouldBeTrue();
        result.AsT2.NestedPluginsThatCouldNotBeInstalled.ShouldContain(depId);

        _logger
            .Received()
            .Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString()!.Contains(depId.Name)),
                null,
                Arg.Any<Func<object, Exception?, string>>()
            );
    }

    #endregion
}
