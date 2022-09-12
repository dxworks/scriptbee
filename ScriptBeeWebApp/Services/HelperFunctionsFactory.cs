using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Services;

namespace ScriptBeeWebApp.Services;

public class HelperFunctionsFactory : IHelperFunctionsFactory
{
    private readonly IFileModelService _fileModelService;
    private readonly IFileNameGenerator _fileNameGenerator;

    public HelperFunctionsFactory(IFileModelService fileModelService, IFileNameGenerator fileNameGenerator)
    {
        _fileModelService = fileModelService;
        _fileNameGenerator = fileNameGenerator;
    }

    public IHelperFunctionsContainer Create(string projectId, string runId)
    {
        // todo use dependency injection to create the helper functions (include IHelperFunctionsResultService)
        throw new System.NotImplementedException();
    }
}
