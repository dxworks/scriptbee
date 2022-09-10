using DxWorks.ScriptBee.Plugin.Api.HelperFunctions;
using DxWorks.ScriptBee.Plugin.Api.ScriptRunner;

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

    public IHelperFunctions Create(string projectId, string runId)
    {
        throw new System.NotImplementedException();
    }
}
