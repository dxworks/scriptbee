using System.Reflection;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.Api.Services;
using DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp;

public class CSharpScriptRunner : IScriptRunner
{
    public string Language => "csharp";

    public async Task RunAsync(IProject project, IHelperFunctionsContainer helperFunctionsContainer,
        IEnumerable<ScriptParameter> parameters, string scriptContent, CancellationToken cancellationToken = default)
    {
        var validScript = new ScriptGeneratorStrategy().ExtractValidScript(scriptContent);

        var compiledScript =
            await Task.Run(() => CompileScript(validScript, parameters, helperFunctionsContainer, cancellationToken),
                cancellationToken);

        await Task.Run(() => ExecuteScript(project, compiledScript, helperFunctionsContainer),
            cancellationToken);
    }

    private static void ExecuteScript(IProject project, Assembly compiledScriptAssembly,
        IHelperFunctionsContainer helperFunctionsContainer)
    {
        PopulateHelperFunctionFields(compiledScriptAssembly, helperFunctionsContainer);

        foreach (var type in compiledScriptAssembly.GetTypes())
        {
            if (type.Name != "ScriptContent")
            {
                continue;
            }

            foreach (var method in type.GetMethods())
            {
                var methodParameters = method.GetParameters();

                if (method.Name == "ExecuteScript")
                {
                    switch (methodParameters.Length)
                    {
                        case 1 when IsProjectType(methodParameters[0].ParameterType):
                        {
                            var scriptContentObject = compiledScriptAssembly.CreateInstance(type.Name);

                            method.Invoke(scriptContentObject, new object[]
                            {
                                project
                            });

                            return;
                        }
                        case 2 when IsProjectType(methodParameters[0].ParameterType) &&
                                    methodParameters[1].ParameterType.Name == "ScriptParameters":
                        {
                            var scriptContentObject = compiledScriptAssembly.CreateInstance(type.Name);
                            var scriptParameters = CreateScriptParameters(compiledScriptAssembly);

                            method.Invoke(scriptContentObject, new[]
                            {
                                project,
                                scriptParameters
                            });

                            return;
                        }
                    }
                }
            }
        }
    }

    private static object? CreateScriptParameters(Assembly compiledScriptAssembly)
    {
        var scriptParameters = compiledScriptAssembly.GetTypes().FirstOrDefault(t => t.Name == "ScriptParameters");

        return scriptParameters is null ? null : Activator.CreateInstance(scriptParameters);
    }

    private static bool IsProjectType(MemberInfo type)
    {
        return type.Name is nameof(IProject) or nameof(Project);
    }

    // todo add tests
    private static void PopulateHelperFunctionFields(Assembly compiledScriptAssembly,
        IHelperFunctionsContainer helperFunctionsContainer)
    {
        var helperFunctionClass = compiledScriptAssembly.GetTypes()
            .FirstOrDefault(t => t.FullName == "ScriptContent");

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

    private static Assembly CompileScript(string script, IEnumerable<ScriptParameter> parameters,
        IHelperFunctionsContainer helperFunctionsContainer, CancellationToken cancellationToken)
    {
        var members = HelperFunctionsGenerator.GetMemberDeclarationSyntaxList(helperFunctionsContainer);
        var scriptParameters = ScriptParametersGenerator.GenerateScriptParameters(parameters);

        script = script.Replace("public void ExecuteScript",
            $"{scriptParameters}{members}public void ExecuteScript");

        // todo when writing script instead of class with execute method 
        // https://stackoverflow.com/questions/13601412/compilation-errors-when-dealing-with-c-sharp-script-using-roslyn
        var syntaxTree = CSharpSyntaxTree.ParseText(script, cancellationToken: cancellationToken);

        CheckErrors(syntaxTree, cancellationToken);

        var compilation = CSharpCompilation.Create("Compilation")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOverflowChecks(true)
                .WithOptimizationLevel(OptimizationLevel.Release))
            .AddReferences(FindReferences())
            .AddSyntaxTrees(syntaxTree);

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
