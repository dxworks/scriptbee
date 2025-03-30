using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace ScriptBee.Persistence.File.Plugin.Yaml;

public class ParsingEventBuffer(LinkedList<ParsingEvent> events) : IParser
{
    private LinkedListNode<ParsingEvent>? _current = events.First;

    public ParsingEvent? Current => _current?.Value;

    public bool MoveNext()
    {
        _current = _current?.Next;
        return _current is not null;
    }

    public void Reset()
    {
        _current = events.First;
    }
}
