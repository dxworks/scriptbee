using System;
using System.Collections.Generic;
using System.Linq;
using ScriptBee.Plugin.Manifest;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace ScriptBee.FileManagement.Yaml;

public class PluginManifestSpecTypeDiscriminator : ITypeDiscriminator
{
    private const string TargetKey = nameof(PluginExtensionPoint.Kind);
    private readonly string _targetKey;
    private readonly Dictionary<string, Type> _typeLookup;

    public PluginManifestSpecTypeDiscriminator(INamingConvention namingConvention, Dictionary<string, Type> typeLookup)
    {
        _typeLookup = typeLookup;
        _targetKey = namingConvention.Apply(TargetKey);
    }

    public Type BaseType => typeof(PluginExtensionPoint);

    public bool TryResolve(ParsingEventBuffer buffer, out Type? suggestedType)
    {
        suggestedType = null;
        if (buffer.TryFindMappingEntry(scalar => _targetKey == scalar.Value, out var key, out var value))
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
        throw new Exception($"Could not determine expectation type, {_targetKey} has an empty value");
    }

    private Type CheckName(string value)
    {
        if (_typeLookup.TryGetValue(value, out var childType))
        {
            return childType;
        }

        var enumerable = _typeLookup.Keys.Aggregate("", (s, s1) => s + "," + s1);
        throw new Exception(
            $"Could not match `{_targetKey}: {value} to a known expectation. Expecting one of: {enumerable}");
    }
}
