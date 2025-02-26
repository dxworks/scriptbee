namespace ScriptBee.Common;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow() => DateTimeOffset.UtcNow;
}
