using ScriptBee.Ports.Driving.UseCases;

namespace ScriptBee.Domain.Service;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow() => DateTimeOffset.UtcNow;
}
