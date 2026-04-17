namespace ScriptBee.Plugins.Yaml;

public interface ITypeDiscriminator
{
    Type BaseType { get; }

    bool TryResolve(ParsingEventBuffer buffer, out Type? suggestedType);
}
