using ScriptBee.Ports.Driving.UseCases;

namespace ScriptBee.Domain.Service;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow() => DateTime.UtcNow;
}
