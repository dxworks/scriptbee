using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HelperFunctions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ScriptBee.PluginManager;
using ScriptBee.Scripts.ScriptRunners.Exceptions;
using Project = ScriptBee.ProjectContext.Project;

namespace ScriptBee.Scripts.ScriptRunners
{
    public class CSharpScriptRunner : IScriptRunner
    {
        private readonly IPluginPathReader _pluginPathReader;
        private readonly IHelperFunctionsFactory _helperFunctionsFactory;

        public CSharpScriptRunner(IPluginPathReader pluginPathReader, IHelperFunctionsFactory helperFunctionsFactory)
        {
            _pluginPathReader = pluginPathReader;
            _helperFunctionsFactory = helperFunctionsFactory;
        }

        public Task<List<RunResult>> Run(Project project, string runId, string scriptContent)
        {
            var compiledScript = CompileScript(scriptContent);

            var helperFunctions = _helperFunctionsFactory.Create(project.Id, runId);

            ExecuteScript(project, helperFunctions, compiledScript);

            return helperFunctions.GetResults();
        }

        private void ExecuteScript(Project project, IHelperFunctions helperFunctions,
            Assembly compiledScript)
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

        private Assembly CompileScript(string script)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(script);

            var compilationUnitRoot = syntaxTree.GetCompilationUnitRoot();

            var syntacticDiagnostics = compilationUnitRoot.GetDiagnostics();

            CheckErrors(syntacticDiagnostics);

            var compilation = CSharpCompilation.Create("Compilation", new SyntaxTree[]
            {
                syntaxTree
            }, FindReferences(), new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOverflowChecks(true).WithOptimizationLevel(OptimizationLevel.Release));

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var semanticDiagnostics = semanticModel.GetDiagnostics();

            CheckErrors(semanticDiagnostics);

            using (var stream = new MemoryStream())
            {
                compilation.Emit(stream);
                Assembly assembly = Assembly.Load(stream.GetBuffer());

                return assembly;
            }
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

            foreach (var pluginPath in _pluginPathReader.GetPluginPaths())
            {
                references.Add(MetadataReference.CreateFromFile(pluginPath));
            }

            return references;
        }
    }
}