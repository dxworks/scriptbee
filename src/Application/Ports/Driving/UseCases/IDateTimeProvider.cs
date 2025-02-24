namespace ScriptBee.Ports.Driving.UseCases;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow();
}
