namespace ScriptBee.Persistence.File.Yaml;

public interface ITypeDiscriminator
{
    Type BaseType { get; }

    bool TryResolve(ParsingEventBuffer buffer, out Type? suggestedType);
}
