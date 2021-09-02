using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ScriptBee.Plugins;
using ScriptBee.Scripts.ScriptRunners.Exceptions;
using DummyPlugin;

namespace ScriptBee.Scripts.ScriptRunners
{
    public class CSharpDummyScriptRunner : DummyScriptRunner
    {
        private readonly IPluginLoader _pluginLoader;

        public CSharpDummyScriptRunner(IPluginLoader pluginLoader)
        {
            _pluginLoader = pluginLoader;
        }

        public override void RunScript(DummyModel dummyModel, string script)
        {
            var compiledScript = CompileScript(script);

            ExecuteScript(dummyModel, compiledScript);
        }

        private void ExecuteScript(DummyModel dummyModel, Assembly compiledScript)
        {
            foreach (var type in compiledScript.GetTypes())
            {
                if (type.Name == "ScriptContent")
                {
                    foreach (var method in type.GetMethods())
                    {
                        if (method.Name == "ExecuteScript" && method.GetParameters()
                            .SingleOrDefault(param => param.ParameterType.Name == "DummyModel") != null)
                        {
                            var scriptContentObject = compiledScript.CreateInstance(type.Name);
                            method.Invoke(scriptContentObject, new object[]
                            {
                                dummyModel
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
                var errorMessage = errors.ToString();
                throw new CompilationErrorException(errorMessage);
            }
        }

        private List<PortableExecutableReference> FindReferences()
        {
            var references = new List<PortableExecutableReference>();

            var value = (string) AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
            if (value != null)
            {
                var pathToDlls = value.Split(Path.PathSeparator);
                references.AddRange(pathToDlls.Where(pathToDll => !string.IsNullOrEmpty(pathToDll))
                    .Select(pathToDll => MetadataReference.CreateFromFile(pathToDll))
                );
            }

            foreach (var pluginPath in _pluginLoader.GetPluginPaths())
            {
                references.Add(MetadataReference.CreateFromFile(pluginPath));
            }

            return references;
        }
    }
}