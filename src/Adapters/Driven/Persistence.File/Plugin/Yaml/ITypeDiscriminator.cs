namespace ScriptBee.Persistence.File.Plugin.Yaml;

public interface ITypeDiscriminator
{
    Type BaseType { get; }

    bool TryResolve(ParsingEventBuffer buffer, out Type? suggestedType);
}
