using System.Reflection;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.Api.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ScriptBee.Scripts.ScriptRunners.Exceptions;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp;

public class CSharpScriptRunner : IScriptRunner
{
    public string Language => "csharp";

    public async Task RunAsync(IProject project, IHelperFunctionsContainer helperFunctionsContainer,
        string scriptContent, CancellationToken cancellationToken = default)
    {
        var compiledScript =
            await Task.Run(() => CompileScript(scriptContent, helperFunctionsContainer, cancellationToken),
                cancellationToken);

        await Task.Run(() => ExecuteScript(project, compiledScript, helperFunctionsContainer), cancellationToken);
    }

    private static void ExecuteScript(IProject project, Assembly compiledScriptAssembly,
        IHelperFunctionsContainer helperFunctionsContainer)
    {
        PopulateHelperFunctionFields(compiledScriptAssembly, helperFunctionsContainer);

        foreach (var type in compiledScriptAssembly.GetTypes())
        {
            if (type.Name == "ScriptContent")
            {
                foreach (var method in type.GetMethods())
                {
                    var methodParameters = method.GetParameters();
                    if (method.Name == "ExecuteScript" && methodParameters.Length == 1 &&
                        methodParameters[0].ParameterType.Name is nameof(IProject) or nameof(Project))
                    {
                        var scriptContentObject = compiledScriptAssembly.CreateInstance(type.Name);

                        method.Invoke(scriptContentObject, new object[]
                        {
                            project
                        });
                    }
                }
            }
        }
    }

    // todo add tests
    private static void PopulateHelperFunctionFields(Assembly compiledScriptAssembly,
        IHelperFunctionsContainer helperFunctionsContainer)
    {
        var helperFunctionClass = compiledScriptAssembly.GetTypes()
            .FirstOrDefault(t => t.FullName == typeof(HelperFunctions).FullName);

        if (helperFunctionClass is null)
        {
            return;
        }

        foreach (var fieldInfo in helperFunctionClass.GetFields())
        {
            var helperFunction = helperFunctionsContainer.GetFunctions()
                .FirstOrDefault(h => h.GetType().FullName == fieldInfo.FieldType.FullName);

            fieldInfo.SetValue(null, helperFunction);
        }
    }

    private static Assembly CompileScript(string script, IHelperFunctionsContainer helperFunctionsContainer,
        CancellationToken cancellationToken)
    {
        // todo when writing script instead of class with execute method 
        // https://stackoverflow.com/questions/13601412/compilation-errors-when-dealing-with-c-sharp-script-using-roslyn
        var syntaxTree = CSharpSyntaxTree.ParseText(script, cancellationToken: cancellationToken);

        CheckErrors(syntaxTree, cancellationToken);

        var helperFunctionsSyntaxTree = CSharpSyntaxTree.ParseText(
            HelperFunctionsGenerator.CreateSyntaxTree(helperFunctionsContainer).ToString(),
            cancellationToken: cancellationToken);

        CheckErrors(helperFunctionsSyntaxTree, cancellationToken);

        var compilation = CSharpCompilation.Create("Compilation")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOverflowChecks(true)
                .WithOptimizationLevel(OptimizationLevel.Release))
            .AddReferences(FindReferences())
            .AddSyntaxTrees(helperFunctionsSyntaxTree, syntaxTree);

        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream, cancellationToken: cancellationToken);

        if (!emitResult.Success)
        {
            var errorMessage = emitResult.Diagnostics.Select(d => d.GetMessage())
                .Aggregate("", (current, error) => current + error + "\n");
            throw new CompilationErrorException(errorMessage);
        }

        return Assembly.Load(stream.ToArray());
    }

    private static void CheckErrors(SyntaxTree syntaxTree, CancellationToken cancellationToken)
    {
        var compilationUnitRoot = syntaxTree.GetCompilationUnitRoot(cancellationToken: cancellationToken);

        var diagnostics = compilationUnitRoot.GetDiagnostics();

        var errors = diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Select(d => d.GetMessage())
            .ToList();

        if (errors.Count > 0)
        {
            var errorMessage = errors.Aggregate("", (current, error) => current + error + "\n");
            throw new CompilationErrorException(errorMessage);
        }
    }

    private static IEnumerable<PortableExecutableReference> FindReferences()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Where(a => !string.IsNullOrEmpty(a.Location))
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location));
    }
}
