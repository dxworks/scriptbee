namespace ScriptBee.Service.Plugin.Tests;

public class PluginNameGeneratorTests
{
    [Fact]
    public void GivenPluginIdAndVersionString_WhenGetPluginName_ThenPluginNameIsReturned()
    {
        var pluginName = PluginNameGenerator.GetPluginName("pluginId", "1.0.0");

        Assert.Equal("pluginId@1.0.0", pluginName);
    }

    [Fact]
    public void GivenPluginIdAndVersion_WhenGetPluginName_ThenPluginNameIsReturned()
    {
        var pluginName = PluginNameGenerator.GetPluginName("pluginId", new Version(1, 0, 0));

        Assert.Equal("pluginId@1.0.0", pluginName);
    }

    [Theory]
    [InlineData("pluginId")]
    [InlineData("pluginId@")]
    [InlineData("pluginId@xxaa")]
    [InlineData("@1.0.0")]
    [InlineData("@")]
    [InlineData("  @1.0.0")]
    [InlineData("\t@\t")]
    [InlineData("\t@4.2.0.1")]
    public void GivenInvalidFolderNane_WhenGetPluginNameAndVersion_ThenNullIsReturned(
        string folderName
    )
    {
        var (id, version) = PluginNameGenerator.GetPluginNameAndVersion(folderName);

        Assert.Null(id);
        Assert.Null(version);
    }

    [Fact]
    public void GivenFolderNameWithDelimiterAndVersion_WhenGetPluginNameAndVersion_ThenPluginNameAndVersionAreReturned()
    {
        var (id, version) = PluginNameGenerator.GetPluginNameAndVersion("plugin@1.0.0");

        Assert.Equal("plugin", id);
        Assert.Equal(new Version(1, 0, 0), version);
    }

    [Fact]
    public void GivenFolderNameWithDelimiterAtTheBeginningInPluginId_WhenGetPluginNameAndVersion_ThenPluginNameAndVersionAreReturned()
    {
        var (id, version) = PluginNameGenerator.GetPluginNameAndVersion("@plugin/plugin@1.0.5");

        Assert.Equal("@plugin/plugin", id);
        Assert.Equal(new Version(1, 0, 5), version);
    }

    [Fact]
    public void GivenFolderNameWithDelimiterInTheMiddleOfPluginId_WhenGetPluginNameAndVersion_ThenPluginNameAndVersionAreReturned()
    {
        var (id, version) = PluginNameGenerator.GetPluginNameAndVersion("plugin@1.2.2@1.0.5");

        Assert.Equal("plugin@1.2.2", id);
        Assert.Equal(new Version(1, 0, 5), version);
    }

    [Fact]
    public void GivenFolderNameWithVersionWith4Parts_WhenGetPluginNameAndVersion_ThenPluginNameAndVersionAreReturned()
    {
        var (id, version) = PluginNameGenerator.GetPluginNameAndVersion("plugin@5.2.66.3");

        Assert.Equal("plugin", id);
        Assert.Equal(new Version(5, 2, 66, 3), version);
    }
}
