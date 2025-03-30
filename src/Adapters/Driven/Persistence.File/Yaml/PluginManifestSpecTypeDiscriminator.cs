using ScriptBee.Domain.Model.Plugin.Manifest;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace ScriptBee.Persistence.File.Yaml;

public class PluginManifestSpecTypeDiscriminator(
    INamingConvention namingConvention,
    Dictionary<string, Type> typeLookup
) : ITypeDiscriminator
{
    private const string TargetKey = nameof(PluginExtensionPoint.Kind);
    private readonly string _targetKey = namingConvention.Apply(TargetKey);

    public Type BaseType => typeof(PluginExtensionPoint);

    public bool TryResolve(ParsingEventBuffer buffer, out Type? suggestedType)
    {
        suggestedType = null;
        if (
            buffer.TryFindMappingEntry(
                scalar => _targetKey == scalar.Value,
                out var key,
                out var value
            )
        )
        {
            // read the value of the kind key
            if (value is Scalar valueScalar)
            {
                suggestedType = CheckName(valueScalar.Value);

                return true;
            }

            FailEmpty();
        }

        // we could not find our key, thus we could not determine correct child type

        return false;
    }

    private void FailEmpty()
    {
        throw new Exception(
            $"Could not determine expectation type, {_targetKey} has an empty value"
        );
    }

    private Type CheckName(string value)
    {
        if (typeLookup.TryGetValue(value, out var childType))
        {
            return childType;
        }

        var enumerable = typeLookup.Keys.Aggregate("", (s, s1) => s + "," + s1);
        throw new Exception(
            $"Could not match `{_targetKey}: {value} to a known expectation. Expecting one of: {enumerable}"
        );
    }
}
