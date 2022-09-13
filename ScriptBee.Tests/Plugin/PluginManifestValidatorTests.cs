using ScriptBee.Plugin;
using ScriptBee.Tests.Plugin.Internals;
using Xunit;

namespace ScriptBee.Tests.Plugin;

public class PluginManifestValidatorTests
{
    private readonly PluginManifestValidator _pluginManifestValidator;

    public PluginManifestValidatorTests()
    {
        _pluginManifestValidator = new PluginManifestValidator();
    }

    [Fact]
    public void GivenMissingEntryPoint_WhenValidate_ThenFalseIsReturned()
    {
        var testPluginManifest = new TestPluginManifest();

        Assert.False(_pluginManifestValidator.Validate(testPluginManifest));
    }

    [Fact]
    public void GivenExistingEntryPoint_WhenValidate_ThenTrueIsReturned()
    {
        var testPluginManifest = new TestPluginManifest
        {
            Metadata =
            {
                EntryPoint = "value"
            }
        };

        Assert.True(_pluginManifestValidator.Validate(testPluginManifest));
    }
}
