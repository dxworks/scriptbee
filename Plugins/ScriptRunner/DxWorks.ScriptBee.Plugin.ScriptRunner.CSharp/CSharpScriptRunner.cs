using System.Reflection;
using DxWorks.ScriptBee.Plugin.Api.HelperFunctions;
using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.Api.ScriptRunner;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ScriptBee.Scripts.ScriptRunners.Exceptions;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp;

public class CSharpScriptRunner : IScriptRunner
{
    // todo
    // private readonly IPluginPathReader _pluginPathReader;
    private readonly IHelperFunctionsFactory _helperFunctionsFactory;

    public CSharpScriptRunner(IHelperFunctionsFactory helperFunctionsFactory)
    {
        _helperFunctionsFactory = helperFunctionsFactory;
    }

    public async Task RunAsync(IProject project, string runId, string scriptContent,
        CancellationToken cancellationToken = default)
    {
        var compiledScript = await Task.Run(() => CompileScript(scriptContent, cancellationToken), cancellationToken);

        var helperFunctions = _helperFunctionsFactory.Create(project.Id, runId);

        await Task.Run(() => ExecuteScript(project, helperFunctions, compiledScript), cancellationToken);
    }

    private void ExecuteScript(IProject project, IHelperFunctions helperFunctions, Assembly compiledScript)
    {
        foreach (var type in compiledScript.GetTypes())
        {
            if (type.Name == "ScriptContent")
            {
                foreach (var method in type.GetMethods())
                {
                    var methodParameters = method.GetParameters();
                    if (method.Name == "ExecuteScript" && methodParameters.Length == 2 &&
                        methodParameters[0].ParameterType.Name == "Project" &&
                        methodParameters[1].ParameterType.Name == "IHelperFunctions")
                    {
                        var scriptContentObject = compiledScript.CreateInstance(type.Name);
                        method.Invoke(scriptContentObject, new object[]
                        {
                            project,
                            helperFunctions
                        });
                    }
                }
            }
        }
    }

    private Assembly CompileScript(string script, CancellationToken cancellationToken)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(script, cancellationToken: cancellationToken);

        var compilationUnitRoot = syntaxTree.GetCompilationUnitRoot();

        var syntacticDiagnostics = compilationUnitRoot.GetDiagnostics();

        CheckErrors(syntacticDiagnostics);

        var compilation = CSharpCompilation.Create("Compilation", new[]
        {
            syntaxTree
        }, FindReferences(), new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithOverflowChecks(true).WithOptimizationLevel(OptimizationLevel.Release));

        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var semanticDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

        CheckErrors(semanticDiagnostics);

        using var stream = new MemoryStream();
        compilation.Emit(stream, cancellationToken: cancellationToken);

        return Assembly.Load(stream.GetBuffer());
    }

    private static void CheckErrors(IEnumerable<Diagnostic> diagnostics)
    {
        IList<string> errors = new List<string>();

        foreach (var diagnostic in diagnostics)
        {
            if (diagnostic.Severity == DiagnosticSeverity.Error)
            {
                errors.Add(diagnostic.GetMessage());
            }
        }

        if (errors.Count > 0)
        {
            var errorMessage = errors.Aggregate("", (current, error) => current + error + "\n");
            throw new CompilationErrorException(errorMessage);
        }
    }

    private List<PortableExecutableReference> FindReferences()
    {
        var references = new List<PortableExecutableReference>();

        var value = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
        if (value != null)
        {
            var pathToDlls = value.Split(Path.PathSeparator);
            references.AddRange(pathToDlls.Where(pathToDll => !string.IsNullOrEmpty(pathToDll))
                .Select(pathToDll => MetadataReference.CreateFromFile(pathToDll))
            );
        }

        // todo
        // foreach (var pluginPath in _pluginPathReader.GetPluginPaths())
        // {
        //     references.Add(MetadataReference.CreateFromFile(pluginPath));
        // }

        return references;
    }
}
