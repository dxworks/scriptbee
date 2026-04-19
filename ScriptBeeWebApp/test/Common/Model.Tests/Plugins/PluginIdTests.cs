using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Domain.Model.Tests.Plugins;

public class PluginIdTests
{
    [Fact]
    public void GivenPluginIdAndVersionString_WhenGetFullyQualifiedName_ThenPluginNameIsReturned()
    {
        var pluginName = new PluginId("pluginId", new Version("1.0.0"));

        Assert.Equal("pluginId@1.0.0", pluginName.GetFullyQualifiedName());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("pluginId")]
    [InlineData("pluginId@")]
    [InlineData("pluginId@xxaa")]
    [InlineData("@1.0.0")]
    [InlineData("@")]
    [InlineData("  @1.0.0")]
    [InlineData("\t@\t")]
    [InlineData("\t@4.2.0.1")]
    public void GivenInvalidFolderNane_WhenTryParse_ThenNullIsReturned(string? folderName)
    {
        var result = PluginId.TryParse(folderName, out var pluginInfo);

        result.ShouldBeFalse();
        pluginInfo.ShouldBeNull();
    }

    [Fact]
    public void GivenFolderNameWithDelimiterAndVersion_WhenTryParse_ThenPluginNameAndVersionAreReturned()
    {
        var result = PluginId.TryParse("plugin@1.0.0", out var pluginInfo);

        result.ShouldBeTrue();
        pluginInfo.ShouldBe(new PluginId("plugin", new Version(1, 0, 0)));
    }

    [Fact]
    public void GivenFolderNameWithDelimiterAtTheBeginningInPluginId_WhenTryParse_ThenPluginNameAndVersionAreReturned()
    {
        var result = PluginId.TryParse("@plugin/plugin@1.0.5", out var pluginInfo);

        result.ShouldBeTrue();
        pluginInfo.ShouldBe(new PluginId("@plugin/plugin", new Version(1, 0, 5)));
    }

    [Fact]
    public void GivenFolderNameWithDelimiterInTheMiddleOfPluginId_WhenTryParse_ThenPluginNameAndVersionAreReturned()
    {
        var result = PluginId.TryParse("plugin@1.2.2@1.0.5", out var pluginInfo);

        result.ShouldBeTrue();
        pluginInfo.ShouldBe(new PluginId("plugin@1.2.2", new Version(1, 0, 5)));
    }

    [Fact]
    public void GivenFolderNameWithVersionWith4Parts_WhenTryParse_ThenPluginNameAndVersionAreReturned()
    {
        var result = PluginId.TryParse("plugin@5.2.66.3", out var pluginInfo);

        result.ShouldBeTrue();
        pluginInfo.ShouldBe(new PluginId("plugin", new Version(5, 2, 66, 3)));
    }
}
