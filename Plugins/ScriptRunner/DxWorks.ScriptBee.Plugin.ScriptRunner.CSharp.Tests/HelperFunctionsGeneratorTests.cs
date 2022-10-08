using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;
using Xunit;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests;

public class HelperFunctionsGeneratorTests
{
    [Fact]
    public void GivenNonGenericVoidMethod_WhenCreateSyntaxTree_ThenCorrectSyntaxTreeIsGenerated()
    {
        var helperFunctionsContainer = new HelperFunctionsContainer(new List<IHelperFunctions>
        {
            new HelperFunctionWithoutGenericMethodThatReturnsVoid()
        });

        var syntaxTree = HelperFunctionsGenerator.CreateSyntaxTree(helperFunctionsContainer);

        Assert.Equal(@"using System;

namespace DxWorks.ScriptBee.Plugin.Api
{
    static partial class HelperFunctions
    {
        public static DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests.HelperFunctionWithoutGenericMethodThatReturnsVoid HelperFunctionWithoutGenericMethodThatReturnsVoid;
        public static void Method1(string a, string b)
        {
            HelperFunctionWithoutGenericMethodThatReturnsVoid.Method1(a, b);
        }
    }
}", syntaxTree.ToString());
    }

    [Fact]
    public void GivenNonGenericMethodThatReturnsSomething_WhenCreateSyntaxTree_ThenCorrectSyntaxTreeIsGenerated()
    {
        var helperFunctionsContainer = new HelperFunctionsContainer(new List<IHelperFunctions>
        {
            new HelperFunctionWithoutGenericMethodsThatReturnSomething()
        });

        var syntaxTree = HelperFunctionsGenerator.CreateSyntaxTree(helperFunctionsContainer);

        Assert.Equal(@"using System;

namespace DxWorks.ScriptBee.Plugin.Api
{
    static partial class HelperFunctions
    {
        public static DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests.HelperFunctionWithoutGenericMethodsThatReturnSomething HelperFunctionWithoutGenericMethodsThatReturnSomething;
        public static string GetSomething()
        {
            return HelperFunctionWithoutGenericMethodsThatReturnSomething.GetSomething();
        }

        public static DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests.Something GetSomething(string a, int b, char c)
        {
            return HelperFunctionWithoutGenericMethodsThatReturnSomething.GetSomething(a, b, c);
        }
    }
}", syntaxTree.ToString());
    }

    [Fact]
    public void GivenGenericMethods_WhenCreateSyntaxTree_ThenCorrectSyntaxTreeIsGenerated()
    {
        var helperFunctionsContainer = new HelperFunctionsContainer(new List<IHelperFunctions>
        {
            new HelperFunctionsWithGenericMethods()
        });

        var syntaxTree = HelperFunctionsGenerator.CreateSyntaxTree(helperFunctionsContainer);

        Assert.Equal(@"using System;

namespace DxWorks.ScriptBee.Plugin.Api
{
    static partial class HelperFunctions
    {
        public static DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests.HelperFunctionsWithGenericMethods HelperFunctionsWithGenericMethods;
        public static T? Method<T>()
        {
            return HelperFunctionsWithGenericMethods.Method<T>();
        }

        public static DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests.Something Method<T1, T2, T3>(T1? arg1, T2 arg2, T3 arg3)
        {
            return HelperFunctionsWithGenericMethods.Method<T1, T2, T3>(arg1, arg2, arg3);
        }
    }
}", syntaxTree.ToString());
    }

    [Fact]
    public void GivenGenericMethodWithConstrains_WhenCreateSyntaxTree_ThenCorrectSyntaxTreeIsGenerated()
    {
        var helperFunctionsContainer = new HelperFunctionsContainer(new List<IHelperFunctions>
        {
            new HelperFunctionsWithGenericMethodWithConstrains()
        });

        var syntaxTree = HelperFunctionsGenerator.CreateSyntaxTree(helperFunctionsContainer);

        Assert.Equal(@"using System;

namespace DxWorks.ScriptBee.Plugin.Api
{
    static partial class HelperFunctions
    {
        public static DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests.HelperFunctionsWithGenericMethodWithConstrains HelperFunctionsWithGenericMethodWithConstrains;
        public static void Method<T1, T2, T3, T4>()
            where T1 : class, new() where T2 : struct where T3 : DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests.Something
        {
            HelperFunctionsWithGenericMethodWithConstrains.Method<T1, T2, T3, T4>();
        }

        public static void Method<T1, T2, T3>()
            where T1 : class where T2 : T1 where T3 : struct
        {
            HelperFunctionsWithGenericMethodWithConstrains.Method<T1, T2, T3>();
        }

        public static void Method<T, TR>()
            where T : DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests.Something where TR : DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests.Something, DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests.ISomething
        {
            HelperFunctionsWithGenericMethodWithConstrains.Method<T, TR>();
        }
    }
}", syntaxTree.ToString());
    }
    
    [Fact]
    public void GivenMethodWithCollections_WhenCreateSyntaxTree_ThenCorrectSyntaxTreeIsGenerated()
    {
        var helperFunctionsContainer = new HelperFunctionsContainer(new List<IHelperFunctions>
        {
            new HelperFunctionWithCollections()
        });

        var syntaxTree = HelperFunctionsGenerator.CreateSyntaxTree(helperFunctionsContainer);

        Assert.Equal(@"using System;

namespace DxWorks.ScriptBee.Plugin.Api
{
    static partial class HelperFunctions
    {
        public static DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests.HelperFunctionWithCollections HelperFunctionWithCollections;
        public static void Method<T>(System.Collections.Generic.List<T>? list)
        {
            HelperFunctionWithCollections.Method<T>(list);
        }

        public static System.Collections.Generic.IDictionary<string, DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests.Something> Method()
        {
            return HelperFunctionWithCollections.Method();
        }

        public static void Method(System.Collections.Generic.List<System.Collections.Generic.HashSet<string>> values)
        {
            HelperFunctionWithCollections.Method(values);
        }
    }
}", syntaxTree.ToString());
    }
}
