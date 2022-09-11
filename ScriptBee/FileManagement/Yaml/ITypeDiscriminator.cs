using System;

namespace ScriptBee.FileManagement.Yaml;

public interface ITypeDiscriminator
{
    Type BaseType { get; }

    bool TryResolve(ParsingEventBuffer buffer, out Type? suggestedType);
}
