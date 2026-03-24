using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class HelperFunctionsContainerTest
{
    [Fact]
    public void GetFunctionsDictionary_NoHelperFunctions_ReturnsEmptyDictionary()
    {
        var helperFunctionsContainer = new HelperFunctionsContainer([]);

        var result = helperFunctionsContainer.GetFunctionsDictionary();

        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetFunctionsDictionary_SingleHelperFunction_ReturnsDictionaryWithMethods()
    {
        var helperFunction = new TestHelperFunction();
        var helperFunctionsContainer = new HelperFunctionsContainer(
            new List<IHelperFunctions> { helperFunction }
        );

        var result = helperFunctionsContainer.GetFunctionsDictionary();

        result.Count.ShouldBe(2);
        Assert.True(result.ContainsKey(nameof(helperFunction.TestFunction)));
        Assert.True(result.ContainsKey(nameof(helperFunction.OverloadedFunction)));
        Assert.NotNull(result[nameof(helperFunction.TestFunction)]);
        Assert.NotNull(result[nameof(helperFunction.OverloadedFunction)]);
    }

    [Fact]
    public void GetFunctionsDictionary_MultipleHelperFunctions_ReturnsCombinedDictionary()
    {
        var helperFunction1 = new TestHelperFunction();
        var helperFunction2 = new TestHelperFunction();
        var helperFunctionsContainer = new HelperFunctionsContainer(
            new List<IHelperFunctions> { helperFunction1, helperFunction2 }
        );

        var result = helperFunctionsContainer.GetFunctionsDictionary();

        Assert.Equal(2, result.Count);
        Assert.True(result.ContainsKey(nameof(helperFunction1.TestFunction)));
        Assert.True(result.ContainsKey(nameof(helperFunction2.TestFunction)));
    }

    [Fact]
    public void GetFunctionsDictionary_OverloadedMethods_SelectsCorrectMethod()
    {
        var helperFunction = new OverloadedTestHelperFunction();
        var methodInfo = helperFunction
            .GetType()
            .GetMethod(nameof(helperFunction.OverloadedFunction), [typeof(string)]);

        var helperFunctionsContainer = new HelperFunctionsContainer(
            new List<IHelperFunctions> { helperFunction }
        );

        var result = helperFunctionsContainer.GetFunctionsDictionary();

        Assert.Single(result);
        Assert.True(result.ContainsKey(nameof(helperFunction.OverloadedFunction)));
        Assert.Equal(methodInfo, result[nameof(helperFunction.OverloadedFunction)].Method);
    }

    [Fact]
    public void GetFunctions_ReturnsHelperFunctions()
    {
        var helperFunctionsList = new List<IHelperFunctions>
        {
            new TestHelperFunction(),
            new TestHelperFunction(),
        };
        var helperFunctionsContainer = new HelperFunctionsContainer(helperFunctionsList);

        var result = helperFunctionsContainer.GetFunctions();

        Assert.Equal(helperFunctionsList, result);
    }
}
