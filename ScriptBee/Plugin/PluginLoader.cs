using System.Linq;
using ScriptBee.FileManagement;
using Serilog;

namespace ScriptBee.Plugin;

public class PluginLoader : IPluginLoader
{
    private readonly ILogger _logger;
    private readonly IFileService _fileService;
    private readonly IDllLoader _dllLoader;
    private readonly IPluginRepository _pluginRepository;
    private readonly IPluginRegistrationService _pluginRegistrationService;

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
        if (_pluginRegistrationService.TryGetValue(plugin.Manifest.Kind, out var acceptedPluginTypes))
        {
            if (!acceptedPluginTypes!.Any())
            {
                _pluginRepository.RegisterPlugin(plugin.Manifest);
                return;
            }

            var path = _fileService.CombinePaths(plugin.FolderPath, plugin.Manifest.Metadata.EntryPoint);

            var loadDllTypes = _dllLoader.LoadDllTypes(path, acceptedPluginTypes!).ToList();

            foreach (var (@interface, concrete) in loadDllTypes)
            {
                _pluginRepository.RegisterPlugin(plugin.Manifest, @interface, concrete);
            }

            _logger.Information("Plugin {pluginName} loaded", plugin.Manifest.Metadata.Name);
        }
        else
        {
            _logger.Warning("Plugin kind {pluginKind} is not supported", plugin.Manifest.Kind);
        }
    }
}
