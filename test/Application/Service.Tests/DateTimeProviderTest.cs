using Shouldly;

namespace ScriptBee.Domain.Service.Tests;

public class DateTimeProviderTest
{
    private readonly DateTimeProvider _dateTimeProvider = new();

    [Fact]
    public void UtcNow()
    {
        var dateTime = _dateTimeProvider.UtcNow();

        dateTime.ShouldBe(DateTime.UtcNow, TimeSpan.FromMilliseconds(100));
    }
}
