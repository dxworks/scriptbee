namespace ScriptBee.Common;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow();
}
