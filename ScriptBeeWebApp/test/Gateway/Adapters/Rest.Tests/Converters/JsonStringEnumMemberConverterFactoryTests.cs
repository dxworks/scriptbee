using System.Runtime.Serialization;
using System.Text.Json;
using ScriptBee.Rest.Converters;

namespace ScriptBee.Rest.Tests.Converters;

public class JsonStringEnumMemberConverterFactoryTest
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new JsonStringEnumMemberConverterFactory() },
    };

    [Fact]
    public void ShouldDeserializeUsingEnumMember()
    {
        var value = JsonSerializer.Deserialize<TestEnum>(
            """
            "top-navigation-bar"
            """,
            Options
        );

        value.ShouldBe(TestEnum.TopNavigationBar);
    }

    [Fact]
    public void ShouldDeserializeSecondEnumMember()
    {
        var value = JsonSerializer.Deserialize<TestEnum>(
            """
            "side-panel"
            """,
            Options
        );

        value.ShouldBe(TestEnum.SidePanel);
    }

    [Fact]
    public void ShouldDeserializeUsingNameWhenEnumMemberIsMissing()
    {
        var value = JsonSerializer.Deserialize<TestEnum>(@"""NoAttribute""", Options);

        value.ShouldBe(TestEnum.NoAttribute);
    }

    [Fact]
    public void ShouldSerializeUsingEnumMember()
    {
        var json = JsonSerializer.Serialize(TestEnum.TopNavigationBar, Options);

        json.ShouldBe(
            """
            "top-navigation-bar"
            """
        );
    }

    [Fact]
    public void ShouldSerializeUsingNameWhenEnumMemberIsMissing()
    {
        var json = JsonSerializer.Serialize(TestEnum.NoAttribute, Options);

        json.ShouldBe(
            """
            "NoAttribute"
            """
        );
    }

    [Fact]
    public void ShouldThrowForUnknownValue()
    {
        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<TestEnum>(
                """
                "does-not-exist"
                """,
                Options
            )
        );
    }
}

public enum TestEnum
{
    [EnumMember(Value = "top-navigation-bar")]
    TopNavigationBar,

    [EnumMember(Value = "side-panel")]
    SidePanel,

    NoAttribute,
}
