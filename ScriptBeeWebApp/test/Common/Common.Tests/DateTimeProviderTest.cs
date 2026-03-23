namespace ScriptBee.Common.Tests;

public class DateTimeProviderTest
{
    private readonly DateTimeProvider _dateTimeProvider = new();

    [Fact]
    public void UtcNow()
    {
        var dateTime = _dateTimeProvider.UtcNow();

        dateTime.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(100));
    }
}
