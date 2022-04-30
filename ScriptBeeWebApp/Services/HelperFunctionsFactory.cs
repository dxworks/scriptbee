using HelperFunctions;

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

    public IHelperFunctionsWithResults Create(string projectId, string runId)
    {
        return new HelperFunctions(projectId, runId, _fileModelService, _fileNameGenerator);
    }
}