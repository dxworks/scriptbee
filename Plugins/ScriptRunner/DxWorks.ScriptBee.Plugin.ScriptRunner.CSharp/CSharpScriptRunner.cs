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

        await Task.Run(() => ExecuteScript(project, helperFunctionsContainer, compiledScript), cancellationToken);
    }

    private void ExecuteScript(IProject project, IHelperFunctionsContainer helperFunctionsContainer,
        Assembly compiledScript)
    {
        // todo populate helper functions static wrapper with helper functions from helper functions container

        foreach (var type in compiledScript.GetTypes())
        {
            if (type.Name == "ScriptContent")
            {
                foreach (var method in type.GetMethods())
                {
                    var methodParameters = method.GetParameters();
                    if (method.Name == "ExecuteScript" && methodParameters.Length == 1 &&
                        methodParameters[0].ParameterType.Name == nameof(IProject))
                    {
                        var scriptContentObject = compiledScript.CreateInstance(type.Name);
                        method.Invoke(scriptContentObject, new object[]
                        {
                            project
                        });
                    }
                }
            }
        }
    }

    private Assembly CompileScript(string script, IHelperFunctionsContainer helperFunctionsContainer,
        CancellationToken cancellationToken)
    {
        // todo when writing script instead of class with execute method 
        // https://stackoverflow.com/questions/13601412/compilation-errors-when-dealing-with-c-sharp-script-using-roslyn
        var syntaxTree = CSharpSyntaxTree.ParseText(script, cancellationToken: cancellationToken);

        CheckErrors(syntaxTree, cancellationToken);

        var helperFunctionsSyntaxTree =
            HelperFunctionsGenerator.CreateSyntaxTree(helperFunctionsContainer);

        CheckErrors(helperFunctionsSyntaxTree, cancellationToken);

        var compilation = CSharpCompilation.Create("Compilation")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOverflowChecks(true))
            // .WithOptimizationLevel(OptimizationLevel.Release))
            .WithReferences(FindReferences(helperFunctionsContainer))
            // .AddSyntaxTrees(helperFunctionsSyntaxTree, syntaxTree);
            .AddSyntaxTrees(helperFunctionsSyntaxTree);

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

    private List<PortableExecutableReference> FindReferences(IHelperFunctionsContainer helperFunctionsContainer)
    {
        var references = new List<PortableExecutableReference>();

        // foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic))
        // {
        //     references.Add(MetadataReference.CreateFromFile(assembly.Location));
        // }

        // references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        //
        // var portableExecutableReferences = helperFunctionsContainer
        //     .GetFunctions().SelectMany(x => x.GetType().Assembly.GetReferencedAssemblies())
        //     .Select(a => MetadataReference.CreateFromFile(Assembly.Load(a).Location));
        // foreach (var portableExecutableReference in portableExecutableReferences)
        // {
        //     references.Add(portableExecutableReference);
        // }

        // foreach (var reference in helperFunctionsContainer.GetType().Assembly.GetReferencedAssemblies()
        //              .Select(a => MetadataReference.CreateFromFile(Assembly.Load(a).Location)))
        // {
        //     references.Add(reference);
        // }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()
                     .Where(a => !a.IsDynamic)
                     .Where(a => !string.IsNullOrEmpty(a.Location)))
        {
            references.Add(MetadataReference.CreateFromFile(assembly.Location));
        }
        // Assembly.GetEntryAssembly().GetReferencedAssemblies()
        //     .Select(a => MetadataReference.CreateFromFile(Assembly.Load(a).Location))

        // var value = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
        // if (value != null)
        // {
        //     var pathToDlls = value.Split(Path.PathSeparator);
        //     references.AddRange(pathToDlls.Where(pathToDll => !string.IsNullOrEmpty(pathToDll))
        //         .Select(pathToDll => MetadataReference.CreateFromFile(pathToDll))
        //     );
        // }

        return references;
    }
}
