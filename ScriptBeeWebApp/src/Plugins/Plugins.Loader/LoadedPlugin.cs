using System.Runtime.Loader;

namespace ScriptBee.Plugins.Loader;

internal record LoadedPlugin(
    AssemblyLoadContext LoadContext,
    IEnumerable<(Type @interface, Type concrete)> LoadedTypes
);
