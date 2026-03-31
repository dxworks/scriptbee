using ScriptBee.Application.Model.Sorting;

namespace ScriptBee.Application.Model.Tests.Parsers;

public class SortParserTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyInputs_ReturnEmpty(string? input)
    {
        var result = SortParser.Parse<TestUserSortField>(input);

        Assert.Empty(result);
    }

    [Theory]
    [InlineData("Name", TestUserSortField.Name, SortOrder.Ascending)]
    [InlineData("-CreatedAt", TestUserSortField.CreatedAt, SortOrder.Descending)]
    [InlineData("+Email", TestUserSortField.Email, SortOrder.Ascending)]
    [InlineData("name", TestUserSortField.Name, SortOrder.Ascending)]
    [InlineData("cReAtEdAt", TestUserSortField.CreatedAt, SortOrder.Ascending)]
    public void SingleField(string input, TestUserSortField expectedField, SortOrder expectedOrder)
    {
        var result = SortParser.Parse<TestUserSortField>(input);

        Assert.Single(result);
        Assert.Equal(expectedField, result[0].Field);
        Assert.Equal(expectedOrder, result[0].Direction);
    }

    [Theory]
    [InlineData("Name,-CreatedAt")]
    [InlineData("Name, -CreatedAt")]
    [InlineData(" Name , -CreatedAt ")]
    public void MultipleFields(string input)
    {
        var result = SortParser.Parse<TestUserSortField>(input);

        Assert.Equal(2, result.Count);

        Assert.Equal(TestUserSortField.Name, result[0].Field);
        Assert.Equal(SortOrder.Ascending, result[0].Direction);

        Assert.Equal(TestUserSortField.CreatedAt, result[1].Field);
        Assert.Equal(SortOrder.Descending, result[1].Direction);
    }

    [Theory]
    [InlineData("Name,")]
    [InlineData("Name,,")]
    [InlineData("Name,,Email")]
    public void IgnoresEmptyEntries(string input)
    {
        var result = SortParser.Parse<TestUserSortField>(input);

        Assert.Contains(result, x => x.Field == TestUserSortField.Name);
    }

    [Fact]
    public void IgnoresEmptyEntriesAndKeepsNonEmptyOnes()
    {
        var result = SortParser.Parse<TestUserSortField>("Name,,Email");

        Assert.Contains(result, x => x.Field == TestUserSortField.Name);
        Assert.Contains(result, x => x.Field == TestUserSortField.Email);
    }

    [Fact]
    public void IgnoresInvalidFieldsAndKeepsKnownOnes()
    {
        var result = SortParser.Parse<TestUserSortField>("Name,InvalidField");

        Assert.Single(result);
        Assert.Equal(TestUserSortField.Name, result[0].Field);
    }

    [Theory]
    [InlineData("InvalidField")]
    [InlineData("Invalid1,Invalid2")]
    public void IgnoresInvalidFields(string input)
    {
        var result = SortParser.Parse<TestUserSortField>(input);

        Assert.Empty(result);
    }
}

public enum TestUserSortField
{
    Name,
    CreatedAt,
    Email,
}
