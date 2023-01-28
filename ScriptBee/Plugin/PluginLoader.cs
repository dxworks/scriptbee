using System.Linq;
using ScriptBee.FileManagement;
using Serilog;

namespace ScriptBee.Plugin;

public class PluginLoader : IPluginLoader
{
    private readonly IDllLoader _dllLoader;
    private readonly IFileService _fileService;
    private readonly ILogger _logger;
    private readonly IPluginRegistrationService _pluginRegistrationService;
    private readonly IPluginRepository _pluginRepository;

    public PluginLoader(ILogger logger, IFileService fileService, IDllLoader dllLoader,
        IPluginRepository pluginRepository, IPluginRegistrationService pluginRegistrationService)
    {
        _logger = logger;
        _fileService = fileService;
        _dllLoader = dllLoader;
        _pluginRepository = pluginRepository;
        _pluginRegistrationService = pluginRegistrationService;
    }

    public void Load(Models.Plugin plugin)
    {
        foreach (var extensionPoint in plugin.Manifest.ExtensionPoints)
        {
            if (_pluginRegistrationService.TryGetValue(extensionPoint.Kind, out var acceptedPluginTypes))
            {
                if (!acceptedPluginTypes!.Any())
                {
                    _pluginRepository.RegisterPlugin(plugin.Manifest);
                    return;
                }

                var path = _fileService.CombinePaths(plugin.FolderPath, extensionPoint.EntryPoint);

                var loadDllTypes = _dllLoader.LoadDllTypes(path, acceptedPluginTypes!).ToList();

                foreach (var (@interface, concrete) in loadDllTypes)
                {
                    _pluginRepository.RegisterPlugin(plugin.Manifest, @interface, concrete);
                }
            }
            else
            {
                _logger.Warning("Plugin kind {PluginKind} is not supported", extensionPoint.Kind);
            }
        }

        _logger.Information("Plugin {PluginName} loaded", plugin.Manifest.Name);
    }
}
